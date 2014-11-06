using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_System_Workshop
{
    class General
    {
        private List<ShipBlueprint> availableShips;
        public List<ShipBlueprint> AvailableShips
        {
            get { return availableShips; }
            set { availableShips = value; }
        }
        private List<ShipBlueprint> currentFleet;
        public List<ShipBlueprint> CurrentFleet
        {
            get { return currentFleet; }
            set { currentFleet = value; }
        }
        public General()
        {
            Console.WriteLine("General Created");
        }
    }
}
