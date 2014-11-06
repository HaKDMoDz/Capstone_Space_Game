using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_System_Workshop
{
    public enum Archetype { TANK, SUPPORT, SCOUT, SNIPER, SHOTGUN, DISABLER, BROADSIDER }
    class AI_Unit
    {
        Archetype archetype;
        Order currentOrder;

        public float DetermineOrderExecutionViability(Order o)
        {
            return 0.0f;
        }

        public void AssignOrder(Order o)
        {

        }

        public void CalculateNextTurn()
        {

        }
    }
}
