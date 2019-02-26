using System;
using System.Collections.Generic;
using System.Text;
using Dolany.Ai.Core.Base;

namespace Dolany.Ai.Core.Ai.Game.AIAgainst
{
    [AI(Name = "AI对决",
        Description = "AI for Fight Against ai.",
        Enable = true,
        PriorityLevel = 10,
        NeedManulOpen = true)]
    public class AIAgainstAI : AIBase
    {
    }
}
