using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI_Fleet
{
    class RandomManager
    {
        private static System.Random random = new System.Random();
        private static int seed;
        private static System.Random veryRandom;

        public static void InitializeManager()
        {
            seed = (int)(random.NextDouble() * int.MaxValue);
            veryRandom = new System.Random(seed);
            Debug.Log("RandomManager instance created");
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
