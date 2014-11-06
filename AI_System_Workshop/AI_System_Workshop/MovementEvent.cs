using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_System_Workshop
{
    class MovementEvent: BattleEvent
    {
        private Unit source;
        public AI_System_Workshop.Unit Source
        {
            get { return source; }
            set { source = value; }
        }
        private Vector3 destination;
        public AI_System_Workshop.Vector3 Destination
        {
            get { return destination; }
            set { destination = value; }
        }
        private float angle;
        public float Angle
        {
            get { return angle; }
            set { angle = value; }
        }

        public MovementEvent()
        {
            Console.WriteLine("MovementEvent Created");
        }
    }
}
