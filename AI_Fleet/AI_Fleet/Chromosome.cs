using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Chromosome()
        {
            incrementIDs();
        }

        public Chromosome(Gene _gene)
        {
            initChromosome(_gene);
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
            Console.WriteLine("Chromosome " + ID + " has " + alleles.Count +  " genes");
            Console.WriteLine("~~~~~~~~~~~~~~~~~");
            foreach (Gene gene in alleles)
            {
                gene.DebugDisplay();
            }
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine();
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
