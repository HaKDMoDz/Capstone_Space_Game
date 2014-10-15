using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneticAlgorithm;

namespace GeneticAlgorithm
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime start = DateTime.Now;
            Population pop1 = new Population(300);
            DateTime now = DateTime.Now;
            Console.WriteLine("Time taken was: " + (now-start).TotalSeconds + " seconds");
            Console.Out.WriteLine("Press Enter to Continue...");
            Console.In.ReadLine();
        }
    }
}
