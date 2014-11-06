using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    class GeneticAlgorithmStats
    {
        private static int MAX_GENERATIONS = 1000;
        private static int[] numMutations = new int[MAX_GENERATIONS];
        public static void AddMutant()
        {
            numMutations[currGenerationNumber]++;
        }

        private static int[] numBabiesBorn = new int[MAX_GENERATIONS];
        public static void addBaby()
        {
            numBabiesBorn[currGenerationNumber]++;
        }

        private static int currGenerationNumber = 0;
        public static int CurrGenerationNumber
        {
            get { return currGenerationNumber; }
            set { currGenerationNumber = value;  }
        }

        public static void debugStats()
        {
            Console.WriteLine("generation\tbabies\tmutants");
            for (int i = 0; i < numMutations.Length; i++)
            {
                Console.Write("\t" + i + "\t" + numBabiesBorn[i] + "\t" + numMutations[i] + "\t");
                Console.WriteLine();
            }
            Console.WriteLine();

            
        }
    }
}
