using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.Ice.Ai.DolanyAI
{
    [AI(
        Name = "KancoleVoiceAI",
        Description = "AI for response random kancole girl voice.",
        IsAvailable = true,
        PriorityLevel = 10
        )]
    public class KancoleVoiceAI : AIBase
    {
        public KancoleVoiceAI()
            : base()
        {
            RuntimeLogger.Log("KancoleVoiceAI started.");
        }

        public override void Work()
        {
        }
    }
}