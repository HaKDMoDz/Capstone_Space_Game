using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI_Fleet
{
    class Population
    {
        List<Organism> genePool = new List<Organism>();
        public List<Organism> GenePool
        {
            get { return genePool; }
            set { genePool = value; }
        }

        public Population(AIManager aiManager)
        {
            initPopulation(RandomManager.randomInt(5, 9), aiManager);
        }

        public Population(int _populaiton, AIManager aiManager)
        {
            initPopulation(_populaiton, aiManager);
        }

        private void initPopulation(int _population, AIManager aiManager)
        {
            Debug.Log("Generating Population\n~~~~~~~~~~~~~~~~~~~~~");
            for (int i = 0; i < _population; i++)
            {
                genePool.Add(new Organism(aiManager));
            }
        }

        public void DebugDisplay()
        {
            foreach (Organism lilSquisher in genePool)
            {
                lilSquisher.DebugDisplay();
            }
        }

        public List<ShipBlueprint> GenerateBluePrints()
        {
            List<ShipBlueprint> bluePrintList = new List<ShipBlueprint>();

            foreach (Organism squishy in genePool)
            {
                bluePrintList.Add(squishy.GenerateBluePrint());
            }

            return bluePrintList;
        }
    }
}
