using Dolany.Ai.Core.Base;

namespace Dolany.Ai.Core.Ai.Assistance
{
    [AI(Name = nameof(SilenceAI),
        Description = "Ai for silence someone",
        Enable = false,
        PriorityLevel = 15)]
    public class SilenceAI : AIBase
    {
    }
}
