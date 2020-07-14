using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;


namespace Production_Planner
{
    static class Verifier
    {
        public static bool HasIllegalChars(bool allowDecimal, TextCompositionEventArgs e)
        {

            Regex reg;
            if (allowDecimal)
            {
                reg = new Regex("[^0-9.]+");
            }
            else
            {
                reg = new Regex("[^0-9]+");
            }
            return reg.IsMatch(e.Text);
        }
    }
}
