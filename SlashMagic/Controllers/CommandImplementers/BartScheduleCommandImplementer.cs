using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SlashMagic.Models;
using SlashMagic.Interfaces;

namespace SlashMagic.Controllers.CommandImplementers
{
    public class BartScheduleCommandImplementer : ISlashCommandImplementer
    {
        private static readonly HttpClient _webClient = new HttpClient();

        private Dictionary<string, string> _bartStations;
        private SlashResponse _response;
        private const string _webRequestUri = @"http://api.bart.gov/api/etd.aspx?cmd=etd&orig={0}&key=MW9S-E7SL-26DU-VV8V&Json=y";
        public BartScheduleCommandImplementer(ICommandProcessor commandProcessor)
        {
            InitializeBartStations();
            commandProcessor.RegisterForCommand("/bart", this);
            
        }

        private void InitializeBartStations()
        {
            _bartStations = File.ReadLines(@"Controllers\CommandImplementers\BartStations.csv")
                                .Select(station => station.Split(','))
                               .ToDictionary(parsed => parsed[0], parsed => string.Concat(parsed.Skip(1)));

        }

        public SlashResponse ProcessCommand(SlashCommand arg)
        {
            if (string.IsNullOrEmpty(arg.text) || !_bartStations.ContainsKey(arg.text.Trim()))
            {
                return SendHelpResponse();
            }
            else
            {
                // timeout is 3 seconds for slash commands. If bart runs late, we need to respond, delegate, and reply async
                return GetSlashResponseForBartStation(arg.text.Trim()).Result;
            }
        }

        private async Task<SlashResponse> GetSlashResponseForBartStation(string stationCode)
        {
            BartResponse bartResponse = await GetBartResponseForStation(stationCode);
            var response = new SlashResponse()
            {
                response_type = SlashResponseType.ephemeral,
                text = string.Concat(
                bartResponse.Root.Station.First().Etd.Select(dest => $"|To *{dest.Destination}*  {GetMinutes(dest.Estimate.First().Minutes.ToString())}|\n"))
            };
            return response;
        }

        private string GetMinutes(string input)
        {
            // need some formating sauce here, since the beta json from Bart is sending strings for "leaving" trains
            if (Int64.TryParse(input, out long minutes))
            {
                return (minutes <= 1)?$"in *{minutes.ToString()}* minute": $"in *{minutes.ToString()}* minutes";
            }
            else
            {
                return $"is *{input}* "; // if not a minute (like leaving), send it as is
            }

        }

        private async Task<BartResponse> GetBartResponseForStation(string stationCode)
        {
            var httpResponse = await _webClient.GetAsync(string.Format(_webRequestUri, stationCode));
            httpResponse.EnsureSuccessStatusCode();
            var rawString = await httpResponse.Content.ReadAsStringAsync();
            
            var jsonResponse = BartResponse.FromJson(rawString);
            return jsonResponse;
        }

        private SlashResponse SendHelpResponse()
        {
            if (_response == null)
            {
                var helpText = new StringBuilder();
                helpText.AppendLine("To see bart schedule at a stop, use the station code. Following are the station codes for reference");
                var stationCodes = string.Concat(_bartStations.Select(kvp => $"||Code: *{kvp.Key}*, Station: *{kvp.Value}*||"));
                helpText.AppendLine(stationCodes);
                _response = new SlashResponse()
                {
                    response_type = SlashResponseType.ephemeral,
                    text = helpText.ToString()
                };
            }
            return _response;
        }
    }
}
