using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_System_Workshop
{
    class MovementEvent: BattleEvent
    {
        Unit source;
        Vector3 destination;
        float angle;
    }
}
