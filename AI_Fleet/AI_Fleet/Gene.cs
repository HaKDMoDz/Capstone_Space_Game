using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Fleet
{
    public enum GeneType { LASER,MISSILE, RAILGUN, FLACK_CANNON, FIGHTER_BAY, ARMOUR, SHIELD, POWERPLANT, THRUSTER, REPAIR_BEAM, SCANNER}
    public enum PlacementType { FORWARD, AFT, PORT, STARBOARD}

    class Gene
    {
        private GeneType type;
        public AI_Fleet.GeneType Type
        {
            get { return type; }
            set { type = value; }
        }

        private int count;
        public int Count
        {
            get { return count; }
            set { count = value; }
        }

        private PlacementType placement;
        public AI_Fleet.PlacementType Placement
        {
            get { return placement; }
            set { placement = value; }
        }

        private int compAbilityStat;
        public int ComponentAbilityStat
        {
            get { return compAbilityStat; }
            set { compAbilityStat = value; }
        }

        private static uint totalGenes = 0;
        public uint ID;

        public Gene(OrganismHull _hull)
        {
            //11 GeneTypes
            int geneType = RandomManager.rollDwhatever(11);
            int placementType = RandomManager.rollDwhatever(4);

            switch (_hull)
            {
                case OrganismHull.CORVETTE:
                    count = RandomManager.randomInt(1, 7);
                    break;
                case OrganismHull.FRIGATE:
                    count = RandomManager.randomInt(1, 9);
                    break;
                case OrganismHull.CRUISER:
                    count = RandomManager.randomInt(1,16);
                    break;
                case OrganismHull.BATTLESHIP:
                    count = RandomManager.randomInt(1, 27);
                    break;
                default:
                    break;
            }

            initGene((GeneType)(geneType), count, (PlacementType)(placementType), count * 100);
        }

        public Gene(GeneType _type, int _count, PlacementType _palcement, int _compAbilityStat)
        {
            initGene(_type, _count, _palcement, _compAbilityStat);
        }

        private void initGene(GeneType _type, int _count, PlacementType _palcement, int _compAbilityStat)
        {
            type = _type;
            count = _count;
            placement = _palcement;
            compAbilityStat = _compAbilityStat;

            IncrementGeneCounter();
        }

        private void IncrementGeneCounter()
        {
            ID = ++totalGenes;
        }

        public void DebugDisplay()
        {
            Console.WriteLine("Gene " + ID + " (type" + type + ") [count " + count + "] |placement " + placement + "| {abilityStat " + compAbilityStat + "}\n");
        }

        public void DebugDisplayVerbose()
        {
            Console.WriteLine("Gene " + ID + "\n(type" + type + ")\n[count " + count + "]\n{abilityStat " + compAbilityStat + "}\n");
        }
    }
}
