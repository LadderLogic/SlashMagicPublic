using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlashMagic.Interfaces
{
    public interface ICommandProcessor
    {
        void RegisterForCommand(string commandName, ISlashCommandImplementer implementer);
    }
}
