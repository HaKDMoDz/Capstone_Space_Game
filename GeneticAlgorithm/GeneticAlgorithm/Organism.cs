using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class Organism
    {
        private static int id = 0;
        private int organismID;
        public int OrganismID
        {
            get { return organismID; }
            set { organismID = value; }
        }
        private float fitness;
        public float Fitness
        {
            get { return fitness; }
            set { fitness = value; }
        }

        private Chromosome chromosome;
        public Chromosome ChromosomeData
        {
            get { return chromosome; }
            set { chromosome = value; }
        }

        public Organism()
        {
            //setup and print the organism's ID
            setupIDandFitness();

            //give the organism some genetic data
            chromosome = new Chromosome();
            
            //print out the genetic data
            DebugGeneticInfoDump();

        }
        public Organism(Chromosome _geneticInfo)
        {
            setupIDandFitness();
            chromosome = _geneticInfo;
            DebugGeneticInfoDump();
        }

        public Organism Breed(Organism _mate)
        {
            int geneSwapThreshold = chromosome.TestGenetics.Count / 2;
            Chromosome babyDNA = new Chromosome();
            babyDNA.TestGenetics.RemoveRange(0, 1);
            babyDNA.TestGenetics.Add(string.Concat(
                chromosome.TestGenetics.First<string>().Substring(0, geneSwapThreshold),
                _mate.ChromosomeData.TestGenetics.First<string>().Substring(geneSwapThreshold, _mate.ChromosomeData.TestGenetics.First<string>().Length - geneSwapThreshold)));
            
            Console.WriteLine("--------------------------");
            Console.WriteLine("Daddy DNA: " + _mate.ChromosomeData.TestGenetics.First<string>());
            Console.WriteLine("Mommy DNA: " + chromosome.TestGenetics.First<string>());
            Console.WriteLine("--------------------------");
            Console.WriteLine("Baby DNA: " + babyDNA.TestGenetics.First<string>());
            Console.WriteLine("--------------------------");
            return new Organism(babyDNA);
        }

        private void setupIDandFitness()
        {
            organismID = ++id;
            Console.WriteLine("organism " + organismID + " has been born");
            fitness = 0;
        }
        public void DebugData()
        {
            Console.Write(OrganismID + ": ");
            DebugGeneticInfoDump();
        }

        private void DebugGeneticInfoDump()
        {
            string chromosomeData = "";
            foreach (string item in chromosome.TestGenetics)
            {
                chromosomeData = string.Concat(chromosomeData, item);
            }
            Console.WriteLine("with genetic info: " + chromosomeData);
        }

        public void testFitness(Organism _ideal)
        {
            Chromosome idealDNA = _ideal.ChromosomeData;
            string encodedDNA = idealDNA.TestGenetics.First<string>();//TODO replace with something that goes through each genome in the chromosome

            float total = encodedDNA.Length;
            int numMatching = 0;
            int index = 0;
            foreach (char item in encodedDNA)
            {
                
                if (item == chromosome.TestGenetics.First<string>().ToCharArray()[index])
                {
                    numMatching++;
                }
            }

            fitness = (float)numMatching / total;
        }
    }
}

public class CompareByFitness : IComparer<GeneticAlgorithm.Organism>
{
    public int Compare(GeneticAlgorithm.Organism x, GeneticAlgorithm.Organism y)
    {
        int compareFitness = x.Fitness.CompareTo(y.Fitness);

        return compareFitness;
    }
}
