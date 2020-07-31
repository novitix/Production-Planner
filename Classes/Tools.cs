using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Production_Planner.Classes
{
    public static class Tools
    {
        public static double CalcExRate(double rmbCost, double exRate)
        {
            // returns AUD amount to 2 d.p.
            return Math.Round(rmbCost / exRate, 2);
        }
    }
}
