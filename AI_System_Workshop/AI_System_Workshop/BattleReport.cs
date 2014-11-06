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
        public bool WonEngagement
        {
            get { return wonEngagement; }
            set { wonEngagement = value; }
        }

        private int battleDuration;
        public int BattleDuration
        {
            get { return battleDuration; }
            set { battleDuration = value; }
        }

        private List<MissionObjective> playerObjectives;
        public List<MissionObjective> PlayerObjectives
        {
            get { return playerObjectives; }
            set { playerObjectives = value; }
        }

        private List<AI_Objective> AI_objectives;
        public List<AI_Objective> AI_Objectives
        {
            get { return AI_objectives; }
            set { AI_objectives = value; }
        }

        private Dictionary<Unit, float> unit_damageDone_table;
        public Dictionary<Unit, float> unit_DamageDone_table
        {
            get { return unit_damageDone_table; }
            set { unit_damageDone_table = value; }
        }

        private Dictionary<Unit, float> unit_supportDone_table;
        public Dictionary<Unit, float> unit_SupportDone_table
        {
            get { return unit_supportDone_table; }
            set { unit_supportDone_table = value; }
        }

        private Dictionary<Unit, float> unit_damageTaken_table;
        public Dictionary<Unit, float> Unit_damageTaken_table
        {
            get { return unit_damageTaken_table; }
            set { unit_damageTaken_table = value; }
        }

        private Dictionary<Unit, float> unit_timeStayedAlive_table;
        public Dictionary<Unit, float> Unit_timeStayedAlive_table
        {
            get { return unit_timeStayedAlive_table; }
            set { unit_timeStayedAlive_table = value; }
        }

        private Dictionary<Unit, List<PlayerUnit>> damageSources_table;
        public Dictionary<Unit, List<PlayerUnit>> DamageSources_table
        {
            get { return damageSources_table; }
            set { damageSources_table = value; }
        }

        private Dictionary<Unit, List<Vector3>> angleOfAttack_table;
        public Dictionary<Unit, List<Vector3>> AngleOfAttack_table
        {
            get { return angleOfAttack_table; }
            set { angleOfAttack_table = value; }
        }

        private Dictionary<Unit, Queue<Vector3>> movement_table;
        public Dictionary<Unit, Queue<Vector3>> Movement_table
        {
            get { return movement_table; }
            set { movement_table = value; }
        }

        private Dictionary<Unit, Queue<Order>> order_table;
        public Dictionary<Unit, Queue<Order>> Order_table
        {
            get { return order_table; }
            set { order_table = value; }
        }

        public BattleReport()
        {
            Console.WriteLine("BattleReport created");
        }

    }
}
