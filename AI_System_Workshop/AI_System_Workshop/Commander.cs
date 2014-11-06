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
        private List<BattleEvent> predictedEvents;
        private Dictionary<BattleEvent, Order> possibleEventResponse;

        public void setObjectives(List<AI_Objective> objectives)
        {

        }

    }
}
