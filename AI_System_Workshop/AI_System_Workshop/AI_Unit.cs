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
        private Archetype archetype;
        public Archetype Archetype
        {
            get { return archetype; }
            set { archetype = value; }
        }

        private Order currentOrder;
        public Order CurrentOrder
        {
            get { return currentOrder; }
            set { currentOrder = value; }
        }

        public AI_Unit()
        {
            Console.WriteLine("AI_Unit created");
        }

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
