using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.Ai.Core.Ai.Game.Explore
{
    public class ExploreScene
    {
        private const int EncounterCount = 5;

        private ExploreSceneModel Model { get; }

        private List<ExploreEncounter> Encounters { get; } = new List<ExploreEncounter>();

        public ExploreScene(ExploreSceneModel Model)
        {
            this.Model = Model;

            for (var i = 0; i < EncounterCount; i++)
            {
                Encounters.Add(new ExploreEncounter(Model.Encounters.ToDictionary(p => p, p => p.Rate).RandRated()));
            }
        }

        public void StartExplore()
        {

        }
    }
}
