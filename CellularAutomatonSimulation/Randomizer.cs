using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CellularAutomatonSimulation
{
    class Randomizer
    {
        static Random r;

        public static void Initialize()
        {
            r = new Random();
        }

        public static bool DecisionByPercent(int percents)
        {
            return r.Next(0, 100) < percents;
        }
    }
}
