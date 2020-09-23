using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Production_Planner.Classes
{
    public static class Tools
    {
        public static int GetMaxDropHeight()
        {
            int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
            double dropDownHeight = screenHeight;
            return (int)Math.Truncate(dropDownHeight);
        }

        public static double CalcExRate(double rmbCost, double exRate)
        {
            // returns AUD amount to 2 d.p.
            return Math.Round(rmbCost / exRate, 2);
        }


    }
}
