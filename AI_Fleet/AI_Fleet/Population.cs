using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Population()
        {
            initPopulation(RandomManager.randomInt(6, 12));
        }

        public Population(int _populaiton)
        {
            initPopulation(_populaiton);
        }

        private void initPopulation(int _population)
        {
            Console.WriteLine("Generating Population\n~~~~~~~~~~~~~~~~~~~~~");
            for (int i = 0; i < _population; i++)
            {
                genePool.Add(new Organism());
            }
        }

        public void DebugDisplay()
        {
            foreach (Organism lilSquisher in genePool)
            {
                lilSquisher.DebugDisplay();
            }
        }
    }
}
