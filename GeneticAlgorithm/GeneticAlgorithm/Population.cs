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
            Console.WriteLine("//////////////////////////////////////");
            Console.WriteLine("//Generation: " + ++numGenerations+"//");
            Console.WriteLine("//////////////////////////////////////");

            //select a champion
            Organism champion = GetChampion();

            //breed it with the top 3 and add children to next population
            IComparer<Organism> sortByFitness = new CompareByFitness();
            currentGen.Sort(sortByFitness);
            int numToTransfer = 3;
            int numTransferred;

            while (currentGen.Count > 3)
            {
                numTransferred = 0;
                for (int j = (currentGen.Count - 1); j >= 0; j--)
                {
                    
                    Organism item = currentGen[j];
                    if (item.OrganismID != champion.OrganismID)
                    {
                        
                        if (numTransferred < numToTransfer)
                        {
                            
                            //add the top 3 to next population
                            numTransferred++;

                            if (nextGen.Count < _populationSize)
                            {
                                nextGen.Add(item);
                                nextGen.Add(item.Breed(champion));
                            }

                            currentGen.Remove(item);
                        }
                    }
                }

                //kill the champion
                currentGen.Remove(champion);
                
            }

            //repeat until current gen pop count < 4
            
            currentGen = nextGen;
            //test code to make sure we don't get an infinite loop 
            if (numGenerations > (10 - 1))
            {
                idealFound = true;
            }
            if (currentGen.Count > 0)
            {
                GetChampion().DebugData();
            }
            
        }
    }
}
/*


        
 * 
 * 
 * 
 * 
 private static Random _random = new Random();

		/// <summary>
		/// The number of bits in the gene.
		/// </summary>
        private int _geneSize;

		/// <summary>
		/// The number of genomes in the world - should be an even number.
		/// </summary>
        private int _populationSize;

		/// <summary>
		/// The percentage chance of a cross over (genes being swapped in two genomes) each generation.
		/// </summary>
        private int _crossOverChance;

		/// <summary>
		/// The percentage chance of a mutation (a bit being flipped in a random position) over each generation.
		/// </summary>
        private int _mutationChance;
		
		/// <summary>
		/// This World's population of genomes.
		/// </summary>
		public IList<Genome> Population { get; set; }

		/// <summary>
		/// Display console logging when mutations and cross overs don't occur.
		/// </summary>
	    public bool ShowDebugMessages { get; set; }

		/// <summary>
		/// Creates a new instance of a GA world.
		/// </summary>
		/// <param name="geneSize">The number of genes in a genome</param>
		/// <param name="populationSize">The size of the population</param>
		/// <param name="crossOverChance">The percentage chance (1-100) of a cross-over occuring (a set
		/// of genes being swapped) between two genomes that mate.</param>
		/// <param name="mutationChance">The percentage chance (1-100) of a mutation occuring (a single
		/// gene being swapped) between two genomes that mate.</param>
        public World(int geneSize, int populationSize, int crossOverChance, int mutationChance)
        {
            _geneSize = geneSize;
            _populationSize = populationSize;
            _crossOverChance = crossOverChance;
            _mutationChance = mutationChance;
			Population = new List<Genome>();
			InitializePopulation();
        }

		public Genome GetChampion()
		{
			for (int i = 0; i < Population.Count; i++)
			{
				if (FitnessFunction(Population[i]) == 14)
					return Population[i];
			}

			return null;
		}

        public virtual int FitnessFunction(Genome genome)
        {
			return genome.Total;
        }

        public void InitializePopulation()
        {
            Population = new List<Genome>();

            for (int i = 0; i < _populationSize; i++)
            {
                Genome genome = new Genome(_geneSize);
                genome.RandomizeGeneValues();

                Population.Add(genome);
            }
        }

		public void NextGeneration()
		{
			EnsurePopulationIsCreated();

			List<Genome> nextGeneration = new List<Genome>();

			for (int i = 0; i < Population.Count; i++)
			{
				Genome genome = SpinBiasedRouletteWheel();
				nextGeneration.Add(genome);
			}

			Population = nextGeneration;
		}

		public Genome SpinBiasedRouletteWheel(Random random = null)
        {
			EnsurePopulationIsCreated();

			if (random == null)
				random = _random;

            // Get the total fitness value of all genomes
			int populationTotal = Population.Sum(x => x.Total);

			for (int i = 0; i < Population.Count; i++)
	        {
				Genome genome = Population[i];

				// Weighted % value of each genome: genome fitness value/ total
				// This % represents the chance the genome is picked.
				decimal percentage = ((decimal) genome.Total / populationTotal) * 100;

				// Roll 1-100. If the % lies within in this number, return it.
				// For example: 
				//	percentage is 60%
				//	random number is 75
				//  = doesn't get picked
				int randomNumber = random.Next(1, 100);
				if (percentage <= 0 || randomNumber <= percentage)
				{
					return genome;
				}
	        }

			return Population.First();
        }

        public void CrossOver(Random random = null)
        {
			EnsurePopulationIsCreated();

			if (random == null)
				random = _random;

            // Figure out if crossover should occur for this generation, based on a roll of a random number
			decimal percentage = _crossOverChance;
			int randomNumber = random.Next(1, 100);

			if (percentage > 0 && randomNumber <= percentage)
			{
				// Loop through all genome pairs
				for (int i = 0; i < Population.Count; i += 2)
				{
					if (i > Population.Count)
						break;

					Genome genome1 = Population[i];
					Genome genome2 = Population[i + 1];

					// Pick a random position to swap at
					int position = random.Next(0, _geneSize);

					// Create 2 new genomes with the two parts swapped
					Genome newGenome1 = genome1.Clone();
					Genome newGenome2 = genome2.Clone();

					newGenome1.SwapWith(genome2, position);
					newGenome2.SwapWith(genome1, position);

					Population[i] = newGenome1;
					Population[i + 1] = newGenome2;
				}
			}
			else
			{
				// (No cross over, return)
				if (ShowDebugMessages)
					Console.WriteLine("No crossover performed - the random {0}% was over the {1}% threshold.", randomNumber, percentage);
			}
        }

		public void Mutate(Random random = null)
		{
			EnsurePopulationIsCreated();

			if (random == null)
				random = _random;

			// Figure out if mutation should occur for this generation, based on a roll of a random number
			decimal percentage = _mutationChance;
			int randomNumber = random.Next(1, 100);

			if (percentage > 0 && randomNumber <= percentage)
			{
				// Loop through all genome pairs
				for (int i = 0; i < Population.Count; i += 2)
				{
					if (i > Population.Count)
						break;

					Genome genome = Population[i];

					// Pick two random positions to swap at
					int position1 = random.Next(0, _geneSize);
					int position2 = random.Next(0, _geneSize);
					genome.SwapGenes(position1, position2);
				}
			}
			else
			{
				// (No mutation, return)
				if (ShowDebugMessages)
					Console.WriteLine("No mutation performed - the random {0}% was over the {1}% threshold.", randomNumber, percentage);
			}
		}

        public override string ToString()
        {
            string result = "";
            
            // Total up all genomes (the entire population)
            for (int i = 0; i < Population.Count; i++)
            {
                result += Population[i].ToString() + Environment.NewLine;
            }

            return result;
        }

		private void EnsurePopulationIsCreated()
		{
			if (Population.Count == 0)
				throw new InvalidOperationException("The population is empty! Use InitializePopulation() first");
		}
*/