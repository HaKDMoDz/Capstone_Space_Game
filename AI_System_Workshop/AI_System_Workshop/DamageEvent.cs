using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_System_Workshop
{
    class DamageEvent: BattleEvent
    {
        private Unit u_source;
        public Unit u_Source
        {
            get { return u_source; }
            set { u_source = value; }
        }
        private Unit u_destination;
        public Unit u_Destination
        {
            get { return u_destination; }
            set { u_destination = value; }
        }
        private Component c_source;
        public Component c_Source
        {
            get { return c_source; }
            set { c_source = value; }
        }
        private Component c_destination;
        public Component c_Destination
        {
            get { return c_destination; }
            set { c_destination = value; }
        }
        private float amount;
        public float Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        public DamageEvent()
        {
            Console.WriteLine("DamageEvent Created");
        }
    }
}
