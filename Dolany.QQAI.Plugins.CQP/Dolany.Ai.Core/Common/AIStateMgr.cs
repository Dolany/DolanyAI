using System.Collections.Generic;
using System.Linq;
using Dolany.Database;
using Dolany.Database.Ai;

namespace Dolany.Ai.Core.Common
{
    public class AIStateMgr
    {
        public static AIStateMgr Instance { get; } = new AIStateMgr();

        private readonly List<AIEnableState> States;

        private AIStateMgr()
        {
            States = MongoService<AIEnableState>.Get();
        }

        public bool GetState(string AiName, long GroupNum)
        {
            return States.Any(s => s.Name == AiName && s.Groups.Contains(GroupNum));
        }

        public void AddSate(string AiName, long GroupNum)
        {
            var state = States.FirstOrDefault(s => s.Name == AiName);
            if (state == null)
            {
                state = new AIEnableState()
                {
                    Name = AiName,
                    Groups = new List<long>() { GroupNum}
                };
                States.Add(state);
                MongoService<AIEnableState>.Insert(state);

                return;
            }

            state.Groups.Add(GroupNum);
            MongoService<AIEnableState>.Update(state);
        }

        public void RemoveSate(string AiName, long GroupNum)
        {
            var state = States.FirstOrDefault(s => s.Name == AiName);
            if (state == null)
            {
                return;
            }

            state.Groups.Remove(GroupNum);
            MongoService<AIEnableState>.Update(state);
        }
    }
}
