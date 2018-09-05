using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.IWS2000.Ai.DolanyAI
{
    [AI(
        Name = nameof(AutoVoiceAI),
        Description = "AI for auto repeating voice/msg.",
        IsAvailable = true,
        PriorityLevel = 10
    )]
    public class AutoVoiceAI : AIBase
    {
        public AutoVoiceAI()
        {
            RuntimeLogger.Log("AutoVoiceAI started.");
        }

        public override void Work()
        {
        }
    }
}