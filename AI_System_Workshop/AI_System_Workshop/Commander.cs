using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_System_Workshop
{
    class Commander
    {
        private Dictionary<PlayerUnit, Archetype> enemyArchetype_table;
        public Dictionary<PlayerUnit, Archetype> EnemyArchetype_table
        {
            get { return enemyArchetype_table; }
            set { enemyArchetype_table = value; }
        }

        private List<BattleEvent> predictedEvents;
        public List<BattleEvent> PredictedEvents
        {
            get { return predictedEvents; }
            set { predictedEvents = value; }
        }

        private Dictionary<BattleEvent, Order> possibleEventResponse;
        public Dictionary<BattleEvent, Order> PossibleEventResponse
        {
            get { return possibleEventResponse; }
            set { possibleEventResponse = value; }
        }

        public Commander()
        {
            Console.WriteLine("Commander created");
        }

        public void setObjectives(List<AI_Objective> objectives)
        {

        }

    }
}
