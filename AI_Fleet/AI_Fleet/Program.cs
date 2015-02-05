using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Fleet
{
    class Program
    {
        static void Main(string[] args)
        {
            RandomManager.InitializeManager();


            #region geneTests
            //Console.WriteLine("Testing Genes...");
            ////test gene
            //Gene testGene001 = new Gene(OrganismHull.CORVETTE);
            //Gene testGene002 = new Gene(GeneType.POWERPLANT, 4, PlacementType.AFT, 4000);

            //testGene001.DebugDisplay();
            //testGene002.DebugDisplay();
            #endregion

            #region chromosomeTests
            //Console.WriteLine("Testing Chromosomes...");
            ////test chromosome
            //Chromosome chromo001 = new Chromosome(OrganismHull.FRIGATE);
            //Chromosome chromo002 = new Chromosome(testGene002);

            //List<Gene> genes = new List<Gene>();
            //genes.Add(testGene001);
            //genes.Add(testGene002);
            //Chromosome chromo003 = new Chromosome(genes);

            //chromo001.DebugDeisplay();
            //chromo002.DebugDeisplay();
            //chromo003.DebugDeisplay();
            #endregion

            #region organismTests
            //Organism org001 = new Organism();
            //org001.DebugDisplay();
            #endregion

            #region populationTests
            Population pop001 = new Population();
            //pop001.DebugDisplay();
            #endregion


            Console.ReadLine();
        }
    }
}