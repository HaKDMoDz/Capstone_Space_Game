using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    class Population
    {
        private static int _populationSize;
        public static int PopulationSize
        {
            get { return _populationSize; }
            set { _populationSize = value; }
        }

        private static Random _random = new Random();
        private bool idealFound = false;
        private int numGenerations = 0;

        private Organism ideal;
        private List<Organism> currentGen, nextGen;

        public Population(int _numOrganisms)
        {
            _populationSize = _numOrganisms;
            
            //setup
            InitializePopulation();

            //run generations
            while (!idealFound)
            {
                
                NextGeneration();
                currentGen = nextGen;
            }

            //display result

        }

        /// <summary>
        /// GetChampion searches through the current generation and 
        /// returns the highest fitness Organism via a linear search
        /// </summary>
        /// <returns>the Organism with the highest fitness</returns>
		public Organism GetChampion()
        {
            Organism _champion;
            IComparer<Organism> sortByFitness = new CompareByFitness();
            currentGen.Sort(sortByFitness);
            if (currentGen.Count == 0)
            {
                _champion = null;
            }
            else
            {
                _champion = currentGen.First<Organism>();
            }
            return _champion;
        }

        /// <summary>
        /// Sets up the population to a state before generation 1
        /// </summary>
        public void InitializePopulation()
        {
            Console.WriteLine("Population initialized");
            //make lists
            currentGen = new List<Organism>();
           

            //setup ideal
            //TODO make this accessible to the topmost level. i.e. population constructor takes in the ideal and allow population to externally set a new ideal
            Chromosome idealGeneticInfo = new Chromosome();
            ideal = new Organism(idealGeneticInfo);

            //fill the first list with _popSize elements
            for (int i = 0; i < _populationSize; i++)
            {
                Organism organism = new Organism();
                organism.testFitness(ideal);
                currentGen.Add(organism);
            }
        }

        /// <summary>
        /// Decide who breeds and who dies and replace the current Generation with the new one
        /// </summary>
		public void NextGeneration()
        {
            
            nextGen = new List<Organism>();

            //Console.Clear();
            /*
            Console.WriteLine("//////////////////////////////////////");
            Console.Write("//Generation: " + ++numGenerations+"//");
            Console.WriteLine("ideal genetic info: " + ideal.ChromosomeData.TestGenetics.First<string>());
            Console.WriteLine("current champ: " + GetChampion().ChromosomeData.TestGenetics.First<string>());
            Console.WriteLine("//////////////////////////////////////");
            */
            //GeneticAlgorithmStats.CurrGenerationNumber = numGenerations;
            numGenerations++;
            Console.Write(".");

            //Console.Out.WriteLine("Press Enter to Continue...");
            //Console.In.ReadLine();


            //breed it with the top 3 and add children to next population
            IComparer<Organism> sortByFitness = new CompareByFitness();
            currentGen.Sort(sortByFitness);


            //select a champion
            Organism champion = GetChampion();

            int numToTransfer = 3;
            int numTransferred;

            
            while (currentGen.Count > 3)
            {
                numTransferred = 0;
                for (int j = (currentGen.Count - 1); j >= 0; j--)
                {
                    
                    Organism item = currentGen[j];
                    item.testFitness(ideal);
                    champion = GetChampion();

                    if (item.Fitness < 0.1f)
                    {
                        nextGen.Add(item.Mutate(ideal));
                        currentGen.Remove(item);
                    }
                    else
                    {
                        if (item.OrganismID != champion.OrganismID)
                        {
                            if (numTransferred < numToTransfer)
                            {
                                //add the top 3 to next population
                                numTransferred++;

                                if (nextGen.Count < _populationSize)
                                {
                                    if (!nextGen.Contains(champion))
                                    {
                                        nextGen.Add(champion);
                                        //currentGen.Remove(champion);
                                    }
                                    nextGen.Add(item);
                                    nextGen.Add(item.Breed(champion, ideal));
                                    currentGen.Remove(item);
                                }
                            }
                        }
                    }
                }
          
                //check for ideal else kill champ
                if (champion.Fitness >= 0.8f)
                {
                    idealFound = true;
                    Console.WriteLine("A champion has risen to ideal status");
                    champion.DebugData();
                    Console.Write("ideal: ");
                    ideal.DebugData();
                    Console.WriteLine("generation reached: " + numGenerations);

                    //GeneticAlgorithmStats.debugStats();
                    break;
                }
                else
                {
                    currentGen.Remove(champion);
                }
            }

            
        }
    }
}
