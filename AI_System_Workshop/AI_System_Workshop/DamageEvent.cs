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
        private Unit u_destination;
        private Component c_source;
        private Component c_destination;
        float amount;

    }
}
