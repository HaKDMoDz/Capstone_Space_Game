using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_System_Workshop
{
    class BattleReport
    {
        private bool wonEngagement;
        private int battleDuration;
        private List<MissionObjective> playerObjectives;
        private List<AI_Objective> AI_objectives;
        private Dictionary<Unit, float> unit_damageDone_table;
        private Dictionary<Unit, float> unit_supportDone_table;
        private Dictionary<Unit, float> unit_damageTaken_table;
        private Dictionary<Unit, float> unit_timeStayedAlive_table;
        private Dictionary<Unit, List<PlayerUnit>> damageSources_table;
        private Dictionary<Unit, List<Vector3>> angleOfAttack_table;
        private Dictionary<Unit, Queue<Vector3>> movement_table;
        private Dictionary<Unit, Queue<Order>> order_table;
    }
}
