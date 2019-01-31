using SlashMagic.Interfaces;
using SlashMagic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlashMagic.Controllers.CommandImplementers
{
    /// <summary>
    /// A loop back implementer for quick and dirty actions
    /// </summary>
    public class SimpleCommandImplementer : ISlashCommandImplementer
    {
        private string _command;
        private Func<SlashCommand, SlashResponse> _simpleFunc;

        public SimpleCommandImplementer(ICommandProcessor commandProcessor, string command, Func<SlashCommand, SlashResponse> func)
        {
            _command = command;
            _simpleFunc = func;
            commandProcessor.RegisterForCommand(command, this);
        }
        public SlashResponse ProcessCommand(SlashCommand arg)
        {
            if (_command == arg.command)
            {
                return _simpleFunc(arg);
            }
            else
            {
                return new SlashResponse()
                {
                    response_type = SlashResponseType.ephemeral,
                    text = "Wasn't expecting that"
                };
            }
        }
    }
}
