using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_System_Workshop
{
    public enum EventType { DAMAGE, SHIP_DESTRUCTION, DISABLE, REPAIR, SHIELD_REGEN, COMPONENT_DESTRUCTION }

    class BattleEvent
    {
        EventType eventType;
    }
}
