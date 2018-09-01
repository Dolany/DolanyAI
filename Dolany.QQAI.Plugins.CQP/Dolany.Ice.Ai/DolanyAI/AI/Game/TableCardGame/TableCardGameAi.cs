using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.Ice.Ai.DolanyAI.AI.Game.TableCardGame
{
    [AI(
        Name = nameof(TableCardGameAi),
        Description = "AI for playing a table card game.",
        IsAvailable = true,
        PriorityLevel = 0
    )]
    public class TableCardGameAi : AIBase
    {
        public TableCardGameAi()
        {
            RuntimeLogger.Log("TableCardGameAi started.");
        }

        public override void Work()
        {
            LoadGames();
        }

        private static void LoadGames()
        {
        }
    }
}