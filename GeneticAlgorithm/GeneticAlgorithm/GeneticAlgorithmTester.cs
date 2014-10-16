using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    class GeneticAlgorithmTester
    {
        public GeneticAlgorithmTester()
        {
            /*
            DateTime start = DateTime.Now;
            Population pop1 = new Population(300);
            DateTime now = DateTime.Now;
            Console.WriteLine("Time taken was: " + (now - start).TotalSeconds + " seconds");
            Console.Out.WriteLine("Press Enter to Continue...");
            Console.In.ReadLine();
            */

            //Setup the List
            List<Test> tests = new List<Test>();
            
            // Add Tests for Chromosome
            TestChromosome testChromosome = new TestChromosome();
            tests.Add(testChromosome);

            //Add Tests for Organism
            TestOrganism testOrganism = new TestOrganism();
            tests.Add(testOrganism);

            //run tests
            foreach (Test item in tests)
            {
                item.RunTest();
            }

            Console.Out.WriteLine("Press Enter to Continue...");
            Console.In.ReadLine();
        }
    }

    class Test
    {
        private static int numTests;
        private int TestID;

        public Test()
        {
            TestID = ++numTests;
        }

        public virtual void RunTest() { } // STUB
    }

    class TestChromosome : Test
    {
        public TestChromosome() { } // STUB

        public override void RunTest()
        {
            Console.WriteLine("Chromosome Tests");
            Console.WriteLine("~~~~~~~~~~~~~~~~");
            Console.WriteLine("Created a Chromosome (random alphanumeric string)");
            Chromosome chr1 = new Chromosome();
            Console.WriteLine(chr1.TestGenetics.First<string>());
            Console.WriteLine();
            Console.WriteLine("randomize its genetic information");
            chr1.RandomizeGenome();
            Console.WriteLine(chr1.TestGenetics.First<string>());
            Console.WriteLine();
            //TODO: set this up so you any added string truncates after the number of chars in the string before you set it
            Console.WriteLine("change to specific genetic info: \"Abnormality\"");
            string geneticInfo = "Abnormality";
            chr1.TestGenetics.RemoveRange(0, 1);
            chr1.TestGenetics.Add(geneticInfo);
            Console.WriteLine(chr1.TestGenetics.First<string>());
            Console.WriteLine();
            Console.WriteLine();
        }
    }

    class TestOrganism : Test
    {
        public TestOrganism() {} // STUB

        public override void RunTest()
        {
            Console.WriteLine("Organism Tests");
            Console.WriteLine("~~~~~~~~~~~~~~");
            Console.WriteLine("Created a new organism with default values (random chromosome)");
            
            //generic organism
            Organism org1 = new Organism();
            org1.DebugData();
            Console.WriteLine();
            Console.WriteLine("Created a second organism");
            
            //generic organism
            Organism org2 = new Organism();
            org2.DebugData();
            Console.WriteLine();
            Console.WriteLine("Create an ideal organism (needed for breed and mutate)");
            Console.WriteLine();
            
            //ideal organism (no diff from generic but we need one)
            Organism ideal = new Organism();
            ideal.ChromosomeData.RandomizeGenome();
            Console.WriteLine("Breed them");
            
            //breed test
            Organism baby = org1.Breed(org2, ideal);
            Console.WriteLine("Daddy DNA: " + org2.ChromosomeData.TestGenetics.First<string>());
            Console.WriteLine("Mommy DNA: " + org1.ChromosomeData.TestGenetics.First<string>());
            Console.WriteLine("--------------------------");
            Console.WriteLine("Baby DNA: " + baby.ChromosomeData.TestGenetics.First<string>());
            Console.WriteLine();
            baby.DebugData();
            Console.WriteLine();

            //mutation test
            Console.WriteLine("Mutate the baby");
            baby.Mutate(ideal);
            baby.DebugData();
            Console.WriteLine();


        }
    }
}
