using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI_Fleet
{
    class Chromosome
    {
       private List<Gene> alleles;
       public List<Gene> Alleles
       {
           get { return alleles; }
           set { alleles = value; }
       }

       static int numChromosomes = 0;
       public int ID;

       private float currentPowerDrain;
       public float CurrentPowerDrain
       {
           get { return currentPowerDrain; }
           set { currentPowerDrain = value; }
       }

        public Chromosome()
        {
            incrementIDs();
        }

        public Chromosome(Gene _gene)
        {
            initChromosome(_gene);
        }

        private void addOrAppend(Gene _gene, ref SlotsPerSection _remainingSlots)
        {
            if (alleles.Count > 0)
            {
                for (int i = 0; i < alleles.Count; i++)
                {
                    if (alleles[i].Placement == _gene.Placement && alleles[i].Type == _gene.Type)
                    {
                        alleles[i].ChangeCount(alleles[i].Count + _gene.Count);
                        break;
                    }
                    else
                    {
                        alleles.Add(_gene);
                        break;
                    }
                }
            }
            else
            {
                alleles.Add(_gene);
            }
        }

        public bool willItFit(Gene _gene, ref SlotsPerSection _remainingSlots)
        {
            bool placed = false;
           
            while (!placed)
            {
                if (_gene.Count <= _remainingSlots.getSlot(_gene.Placement))
                {
                    currentPowerDrain = 0;

                    foreach (Gene gene in Alleles)
                    {
                        currentPowerDrain += gene.PowerDrain;
                    }

                    if (currentPowerDrain + _gene.PowerDrain < -100)
                    {
                        addOrAppend(_gene, ref _remainingSlots);
                        _remainingSlots.setSlot(_gene.Placement, (_remainingSlots.getSlot(_gene.Placement) - _gene.Count));
                        _remainingSlots.RemainingSlots -= _gene.Count;
                        placed = true;
                        return true;
                    }
                    else
                    {
                        _gene = new Gene(GeneType.POWERPLANT, RandomManager.randomInt(1,3), _gene.Placement);
                    }
                }
                else
                {
                    _gene.ChangeCount(_gene.Count - 1);

                    if (_gene.Count <= 0)
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        public Chromosome(List<Gene> _alleles)
        {
            initChromosome(_alleles);
        }

        private void initChromosome(List<Gene> _alleles)
        {
            alleles = _alleles;
            incrementIDs();
        }

        private void initChromosome(Gene _gene)
        {
            List<Gene> _alleles = new List<Gene>();
            _alleles.Add(_gene);
            initChromosome(_alleles);
        }

        public void DebugDisplay()
        {
            float t_activationCost = 0;
            float t_powerDrain = 0;
            float t_damage = 0;
            float t_hullDamagePercent = 0;
            float t_maxHP = 0;
            float t_shieldStrength = 0;

            Debug.Log("Chromosome " + ID + " has " + alleles.Count +  " genes");
            Debug.Log("~~~~~~~~~~~~~~~~~");
            foreach (Gene gene in alleles)
            {
                //gene.DebugDisplay();

                t_activationCost += gene.ActivationCost;
                t_powerDrain += gene.PowerDrain;
                t_damage += gene.Damage;
                t_hullDamagePercent += gene.HullDamagePercent;
                t_maxHP += gene.MaxHP;
                t_shieldStrength += gene.ShieldStrength;
            }
            Debug.Log("{Total dmg " + t_damage + ": ac " + t_activationCost + ": powdr " + t_powerDrain + ": hp " + t_maxHP + ": shield " + t_shieldStrength + "}");
            Debug.Log("~~~~~~~~~~~~~~~~~~~~");
            Debug.Log("");
        }

        private void incrementIDs()
        {
            ID = ++numChromosomes;
        }

        public bool equipGene(Gene _gene)
        {
            return false;
        }
    }
}
