using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SlashMagic.Models;

namespace SlashMagic.Controllers
{
    /// <summary>
    /// Main controller
    /// </summary>
    public class HomeController : Controller
    {
        private SlashCommandProcessor _slashCommandProcessor;
        public HomeController()
        {
            // need DI/IoC here to inject this instead of creating this here, this would suffice for demo purposes
            _slashCommandProcessor = new SlashCommandProcessor();
        }

        /// <summary>
        /// Entry point for all slack slash commands routed at /slack/slash
        /// </summary>
        /// <param name="incoming">Incoming slash command payload. auto parse from JSON</param>
        /// <returns></returns>
        [Route("/slack/slash")]
        [HttpPost]
        [Produces("application/json")]
        public SlashResponse SlashCommandEntry(SlashCommand incoming)
        {
            try
            {
                return _slashCommandProcessor.ProcessCommand(incoming);
            }
            catch
            {
                return new SlashResponse()
                {
                    response_type = SlashResponseType.ephemeral,
                    text = "Unable to process that command :("
                };
            }
        }






        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
