using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlashMagic.Models
{
    public enum SlashResponseType
    {
        in_channel,
        ephemeral
    }

    public class SlashResponse
    {
        public SlashResponseType response_type { get; set; }

        public string text { get; set; }
    }
}
