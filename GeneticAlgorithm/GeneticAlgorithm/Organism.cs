using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GeneticAlgorithm
{
    public class Organism
    {
        private static Random _random = new Random();

        private float percentMaternalAlleleDominance = 0.7f;

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

        }
        public Organism(Chromosome _geneticInfo)
        {
            setupIDandFitness();
            chromosome = _geneticInfo;
        }

        public Organism Breed(Organism _mate, Organism ideal)
        {
            if (chromosome.TestGenetics.First<string>() == _mate.chromosome.TestGenetics.First<string>())
            {
                Mutate(ideal);
                return this;
            }
            else
            {
                int geneSwapThreshold = chromosome.TestGenetics.Count / 2;
                Chromosome babyDNA = new Chromosome();

                //remove the random genetic assignment every new organism gets
                babyDNA.TestGenetics.RemoveRange(0, 1);

                //make char[] of all DNA
                char[] daddyDNAChars = chromosome.TestGenetics.First<String>().ToCharArray();
                char[] mommyDNAChars = _mate.chromosome.TestGenetics.First<String>().ToCharArray();
                char[] idealDNAChars = ideal.chromosome.TestGenetics.First<String>().ToCharArray();
                char[] babyDNAChars = new char[daddyDNAChars.Length];

                //choose between mommy and daddy
                for (int l = 0; l < babyDNAChars.Length; l++)
                {
                    if (daddyDNAChars[l] == idealDNAChars[l])
                    {
                        babyDNAChars[l] = daddyDNAChars[l];
                    }
                    else if (mommyDNAChars[l] == idealDNAChars[l])
                    {
                        babyDNAChars[l] = mommyDNAChars[l];
                    }
                    else
                    {
                        if (_random.NextDouble() > percentMaternalAlleleDominance)
                        {
                            babyDNAChars[l] = mommyDNAChars[l];
                        }
                        else
                        {
                            babyDNAChars[l] = daddyDNAChars[l];
                        }
                    }
                }

                babyDNA.TestGenetics.Add(new string(babyDNAChars));
                return new Organism(babyDNA);
            }
        }

        public Organism Mutate(Organism _ideal)
        {
            
            Chromosome mutantDNA = chromosome;
            float percentChanceToChange = 0.7f;
            const float TOTAL = 100.0f;

            char[] dNAChars = mutantDNA.TestGenetics.First<string>().ToCharArray();
            char[] idealDNAChars = _ideal.chromosome.TestGenetics.First<string>().ToCharArray();

            //open up each genetic element in the chromosome (i.e. each allele)
            for (int k = 0; k < dNAChars.Length; k++)
            {
                //make a randomized choice to either change or leave the DNA alone. TODO: tweak this value
                if ((_random.Next(0, (int)TOTAL)) / (TOTAL) > (1 - percentChanceToChange))
                {
                    if (_random.NextDouble() > 0.9f)
                    {
                        dNAChars[k] = idealDNAChars[k];
                    }
                    else
                    {
                        //Console.Write("A gene was mutated from: " + dNAChars[k]);
                        dNAChars[k] = Path.GetRandomFileName().Substring(0, 1).ToCharArray()[0];
                        //Console.WriteLine(" to: " + dNAChars[k]);
                    }
                }
            }
            mutantDNA.TestGenetics.RemoveRange(0, 1);
            mutantDNA.TestGenetics.Add(new string(dNAChars));

            return new Organism(mutantDNA);
        }

        private void setupIDandFitness()
        {
            organismID = ++id;
            fitness = 0;
        }
        public void DebugData()
        {
            Console.Write("Organism with ID " + OrganismID + " ");
            DebugGeneticInfoDump();
        }

        private void DebugGeneticInfoDump()
        {
            string chromosomeData = "";
            foreach (string item in chromosome.TestGenetics)
            {
                chromosomeData = string.Concat(chromosomeData, item);
            }
            Console.WriteLine("has genetic info: " + chromosomeData);
        }

        public void testFitness(Organism _ideal)
        {

            char[] myDNAChars = chromosome.TestGenetics.First<string>().ToCharArray();
            char[] idealDNAChars = _ideal.chromosome.TestGenetics.First<string>().ToCharArray();

            float total = idealDNAChars.Length;
            int numMatching = 0;

            for (int k = 0; k < idealDNAChars.Length; k++)
            {
                if (idealDNAChars[k] == myDNAChars[k])
                {
                    numMatching++;
                }
            }

            fitness = (float)numMatching / total;
            Console.WriteLine("organism " + OrganismID + " with fitness " + Fitness);
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
