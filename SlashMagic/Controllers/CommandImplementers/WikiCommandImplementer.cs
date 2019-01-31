using SlashMagic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlashMagic.Interfaces;

namespace SlashMagic.Controllers.CommandImplementers
{

    public class WikiCommandImplementer : ISlashCommandImplementer
    {
        private MediaWikiNET.MediaWiki _mediaWiki;

        public WikiCommandImplementer(ICommandProcessor commandProcessor)
        {
            _mediaWiki = new MediaWikiNET.MediaWiki(@"https://en.wikipedia.org/w/api.php");
            commandProcessor.RegisterForCommand("/wiki", this);

        }

        public SlashResponse ProcessCommand(SlashCommand arg)
        {
            var searchQuery = new MediaWikiNET.Models.SearchRequest(arg.text);
            var searchResult = _mediaWiki.Search(searchQuery);
            var returnResult = searchResult.query?.search.FirstOrDefault()?.snippet;
            if (string.IsNullOrEmpty(returnResult))
                returnResult = "Can't help you with that";
            else
                returnResult = PostProcessWikiSnippet(returnResult);

            var retVal = new SlashResponse() { response_type = SlashResponseType.ephemeral, text = returnResult };

            return retVal;
        }
        
        private const string WikiSnippetSpanIn = "<span class=\"searchmatch\">";
        private const string WikiSnippetSpanOut = "</span>";
        private const string SlackBoldStart = "*";
        private const string SlackBoldEnd = "*";

        private string PostProcessWikiSnippet(string snippet)
        {
            // Get rid of html formatting and convert into slack
            return
            snippet.Replace(WikiSnippetSpanIn, SlackBoldStart)
                   .Replace(WikiSnippetSpanOut, SlackBoldEnd);
        }
    }
}
