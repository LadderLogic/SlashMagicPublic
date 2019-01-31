using SlashMagic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlashMagic.Interfaces
{
    public interface ISlashCommandImplementer
    {
        SlashResponse ProcessCommand(SlashCommand arg);
    }
}
