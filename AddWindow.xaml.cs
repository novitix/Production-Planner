using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace Production_Planner
{
    /// <summary>
    /// Interaction logic for AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        private List<Part> partList = new List<Part>();
        private ObservableCollection<Part> prodPtList = new ObservableCollection<Part>(); 
        public AddWindow()
        {
            InitializeComponent();

            partList = DatabaseHandler.GetParts();

            cbPartsList.ItemsSource = partList;
            lbPartsList.ItemsSource = prodPtList;
        }

        private void txtPtQty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = HasIllegalChars(false, e);
        }

        private bool HasIllegalChars(bool allowDecimal, TextCompositionEventArgs e)
        {

            Regex reg;
            if (allowDecimal)
            {
                reg = new Regex("[^0-9.]+");
            } else
            {
                reg = new Regex("[^0-9]+");
            }
            return reg.IsMatch(e.Text);
        }

        private void txtProdCost_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = HasIllegalChars(true, e);
        }
    }
}
