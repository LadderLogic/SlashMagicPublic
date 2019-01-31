using SlashMagic.Controllers.CommandImplementers;
using SlashMagic.Interfaces;
using SlashMagic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlashMagic.Controllers
{
    /// <summary>
    /// Lightweight command pattern implementations. Processes all the slash commands supported through it's implementers
    /// </summary>
    public class SlashCommandProcessor : ICommandProcessor
    {
        private Dictionary<string, ISlashCommandImplementer> _commandImplementers;
        private List<ISlashCommandImplementer> _implementations;

        public SlashCommandProcessor()
        {
            _commandImplementers = new Dictionary<string, ISlashCommandImplementer>();
           
            BuildImplemenations();
        }

        private void BuildImplemenations()
        {
            // Build all the implementers here. An implementer can implement multiple commands. It is upto the implementer to register for the commands
            _implementations = new List<ISlashCommandImplementer>();
            _implementations.Add(new WikiCommandImplementer(this));
            _implementations.Add(new BartScheduleCommandImplementer(this));
            _implementations.Add(new SimpleCommandImplementer(this, "/allthethings", GetAllTheCommands));
        }

       

        public SlashResponse ProcessCommand(SlashCommand incoming)
        {
            if (_commandImplementers.TryGetValue(incoming.command, out ISlashCommandImplementer implementer))
            {
                return implementer.ProcessCommand(incoming);
            }
            else
            {
                var error = new SlashResponse()
                {
                    response_type = SlashResponseType.ephemeral,
                    text = "Something is amiss. I am not ready to accept that command"
                };
                return error;
            }
        }

        /// <summary>
        /// returns all the commands registered as a slash response
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private SlashResponse GetAllTheCommands(SlashCommand arg)
        {
            var response = new SlashResponse()
            {
                response_type = SlashResponseType.in_channel,
                text = $"Hello Everyone! The app Allthings supports these commands: {string.Concat(_commandImplementers.Keys.Select(k => k + "\n"))}"
            };
            return response;
        }

        /// <summary>
        /// Previousy implemented command, but intentionally omitted to test failures
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private SlashResponse ProcessBasicCommand(SlashCommand arg)
        {
            var retVal = new SlashResponse()
            {
                response_type = SlashResponseType.in_channel,
                text = "Beginners All Purpose Symbolic Instruction Code"
            };

            return retVal;
        }

        void ICommandProcessor.RegisterForCommand(string commandName, ISlashCommandImplementer implementer)
        {
            // overwrites old implementation if any by design
            _commandImplementers[commandName] = implementer;
        }

    }
}
