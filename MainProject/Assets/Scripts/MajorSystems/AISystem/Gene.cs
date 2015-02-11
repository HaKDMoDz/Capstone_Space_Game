using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI_Fleet
{
    public enum GeneType { LASER, MISSILE, RAILGUN, FLAK_CANNON, FIGHTER_BAY, REPAIR_BEAM, ARMOUR, SHIELD, POWERPLANT, THRUSTER, SCANNER }
    
    public enum PlacementType { FORWARD, AFT, PORT, STARBOARD, COUNT}

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
            private set { count = value; }
        }
        private PlacementType placement;
        public AI_Fleet.PlacementType Placement
        {
            get { return placement; }
            set { placement = value; }
        }

        private static uint totalGenes = 0;
        public uint ID;

        private float fitness;
        public float Fitness
        {
            get { return fitness; }
            set { fitness = value; }
        }

        //for a single component
        private float maxHP;
        private float powerDrain;
        private float activationCost;
        private float damage;
        private float range;
        private float hullDamagePercent;
        private float armourDmgModifier;
        private float shieldDmgModifier;
        private float shieldStrength;
        private bool unlocked;

        //for multiple components of same type in same palce
        private float m_maxHP;
        public float MaxHP
        {
            get { return m_maxHP; }
            set { m_maxHP = value; }
        }
        private float m_activationCost;
        public float ActivationCost
        {
            get { return m_activationCost; }
            set { m_activationCost = value; }
        }
        private float m_powerDrain;
        public float PowerDrain
        {
            get { return m_powerDrain; }
            set { m_powerDrain = value; }
        }
        private float m_damage;
        public float Damage
        {
            get { return m_damage; }
            set { m_damage = value; }
        }
        private float m_hullDamagePercent;
        public float HullDamagePercent
        {
            get { return m_hullDamagePercent; }
            set { m_hullDamagePercent = value; }
        }
        private float m_shieldStrength;
        public float ShieldStrength
        {
            get { return m_shieldStrength; }
            set { m_shieldStrength = value; }
        }
        

        public Gene(OrganismHull _hull)
        {
            
            //11 GeneTypes
            int geneType = RandomManager.rollDwhatever(11);
            //4 placements
            int placementType = RandomManager.rollDwhatever(4);

            switch (_hull)
            {
                case OrganismHull.CORVETTE:
                    count = RandomManager.randomInt(4,9);
                    break;
                case OrganismHull.FRIGATE:
                    count = RandomManager.randomInt(4,9);
                    break;
                case OrganismHull.CRUISER:
                    count = RandomManager.randomInt(9,16);
                    break;
                case OrganismHull.BATTLESHIP:
                    count = RandomManager.randomInt(15,27);
                    break;
                default:
                    break;
            }

            initGene((GeneType)(geneType), count, (PlacementType)(placementType));
        }

        public void ChangeCount(int _newCount)
        {
            
            count = _newCount;
            m_activationCost = activationCost * (float)count;
            m_powerDrain = powerDrain * (float)count;
            m_damage = damage * (float)count;
            m_hullDamagePercent = hullDamagePercent * (float)count;
            m_maxHP = maxHP * (float)count;
            m_shieldStrength = shieldStrength * (float)count;
        }

        //TEMP
        // replace implementation with one that gets this info from the prefabs once Unity is hooked up
        private void setStats()
        {
            switch (type)
            {
                    //weapons battery 0-5
                case GeneType.LASER:
                    activationCost = 20;
                    powerDrain = 20;
                    maxHP = 100;
                    damage = 50;
                    range = 0;
                    hullDamagePercent = 0;
                    armourDmgModifier = 0;
                    shieldDmgModifier = 0;
                    break;
                case GeneType.MISSILE:
                    activationCost = 20;
                    powerDrain = 20;
                    maxHP = 100;
                    damage = 50;
                    range = 0;
                    hullDamagePercent = 0;
                    armourDmgModifier = 0;
                    shieldDmgModifier = 0;
                    break;
                case GeneType.RAILGUN:
                    activationCost = 20;
                    powerDrain = 20;
                    maxHP = 100;
                    damage = 50;
                    range = 0;
                    hullDamagePercent = 0;
                    armourDmgModifier = 0;
                    shieldDmgModifier = 0;
                    break;
                case GeneType.FLAK_CANNON:
                    activationCost = 20;
                    powerDrain = 20;
                    maxHP = 100;
                    damage = 50;
                    range = 0;
                    hullDamagePercent = 0;
                    armourDmgModifier = 0;
                    shieldDmgModifier = 0;
                    break;
                case GeneType.FIGHTER_BAY:
                    activationCost = 20;
                    powerDrain = 20;
                    maxHP = 100;
                    damage = 50;
                    range = 0;
                    hullDamagePercent = 0;
                    armourDmgModifier = 0;
                    shieldDmgModifier = 0;
                    break;
                case GeneType.REPAIR_BEAM:
                    activationCost = 20;
                    powerDrain = 20;
                    maxHP = 100;
                    damage = 50;
                    range = 0;
                    hullDamagePercent = -100;
                    armourDmgModifier = 0;
                    shieldDmgModifier = -0.15f;
                    break;

                    // defensive 6-7
                case GeneType.ARMOUR:
                    activationCost = 0;
                    powerDrain = 60;
                    maxHP = 100;
                    shieldStrength = 0;
                    break;
                case GeneType.SHIELD:
                    activationCost = 30;
                    powerDrain = 30;
                    maxHP = 50;
                    shieldStrength = 150;
                    break;

                    //power 8
                case GeneType.POWERPLANT:
                    activationCost = 0;
                    powerDrain = -200;
                    maxHP = 100;
                    break;

                    //movement 9
                case GeneType.THRUSTER:
                    activationCost = 50;
                    powerDrain = 100;
                    maxHP = 100;
                    break;

                    //scanners 10
                case GeneType.SCANNER:
                    activationCost = 0;
                    powerDrain = 200;
                    maxHP = 50;
                    break;
                default:
                    break;
            }

        }

        public Gene(GeneType _type, int _count, PlacementType _palcement)
        {
            initGene(_type, _count, _palcement);
        }

        private void initGene(GeneType _type, int _count, PlacementType _palcement)
        {
            type = _type;
            count = _count;
            placement = _palcement;

            fitness = -1.0f; //FLAG for fitness function not run yet

            IncrementGeneCounter();

            setStats();//do this after type is set in initGene or else everything is a laser

            ChangeCount(count);
        }

        private void IncrementGeneCounter()
        {
            ID = ++totalGenes;
        }

        public void DebugDisplay()
        {
            Debug.Log("Gene " + ID + " (type " + type + ") [count " + count + "] |placement " + placement + "|");
        }

        public void DebugDisplayVerbose()
        {
            Debug.Log("Gene " + ID + "\n(type " + type + ")\n[count " + count + "]\n{Stats dmg " + m_damage + ": ac " + m_activationCost + ": powdr " + m_powerDrain + ": hp " + m_maxHP + ": shield " + m_shieldStrength + "}");
        }
    }
}
