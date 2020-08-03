using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
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

    public class DbTxtConverter : IValueConverter
    {
        string _strCache;
        double _dCache;
        //Convert double to string of textbox.
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (_dCache == (double)value)
                return _strCache;
            else
                return value.ToString();
        }
        //convert string to double;
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            _strCache = (string)value;
            if(double.TryParse(_strCache, out _dCache))
            {
                return _dCache;
            }
            else
            {
                return null;
            }
        }
    }
}
