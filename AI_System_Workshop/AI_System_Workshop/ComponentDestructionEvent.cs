using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_System_Workshop
{
    class ComponentDestructionEvent: BattleEvent
    {
        Unit u_source;
        Unit u_destination;
        Component c_source;
        Component c_destination;
    }
}
