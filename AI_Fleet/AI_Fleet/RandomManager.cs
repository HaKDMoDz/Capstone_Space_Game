using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Fleet
{
    class RandomManager
    {
        private static Random random = new Random();
        private static int seed;
        private static Random veryRandom;

        public static void InitializeManager()
        {
            seed = (int)(random.NextDouble() * int.MaxValue);
            veryRandom = new Random(seed);
            Console.WriteLine("RandomManager instance created");
        }

        private RandomManager()
        {
            
        }

        public static int randomInt(int _min, int _max)
        {
            return veryRandom.Next((_max - _min) + 1) + _min;
        }

        public static int rollDwhatever(int _max)
        {
            return (int)(veryRandom.NextDouble() * _max);
        }
    }
}
