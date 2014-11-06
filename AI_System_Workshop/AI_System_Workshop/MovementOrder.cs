using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_System_Workshop
{
    enum MovementOrderType { HOLDPOSITION, RETREAT }

    class MovementOrder : Order
    {
        private MovementOrderType type;
        public AI_System_Workshop.MovementOrderType Type
        {
            get { return type; }
            set { type = value; }
        }

        public MovementOrder()
        {
            Console.WriteLine("MovementOrder Created");
        }
    }
}
