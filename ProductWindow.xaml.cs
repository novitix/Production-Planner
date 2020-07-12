using System;
using System.Collections.Generic;
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
using _IO = System.IO;

namespace Production_Planner
{
    /// <summary>
    /// Interaction logic for ProductWindow.xaml
    /// </summary>
    public partial class ProductWindow : Window
    {
        private Product currentProd;
        private List<PartQty> partsNeeded;
        public ProductWindow(Product selProduct)
        {
            InitializeComponent();
            currentProd = selProduct;
            partsNeeded = DatabaseHandler.GetParts(currentProd);
        }

        private void GetSpreadsheet()
        {
            if (String.IsNullOrEmpty(txtExRate.Text) || string.IsNullOrEmpty(txtQty.Text))
            {
                MessageBox.Show("Please enter exchange rate and quantity.");
                return;
            }
            ExcelWriter exWrite = new ExcelWriter();
            string name = _IO.Path.GetRandomFileName() + ".xlsx";
            double cost = currentProd.Cost_rmb * int.Parse(txtQty.Text);
            partsNeeded = SortParts(partsNeeded);
            exWrite.WriteToExcel(partsNeeded, name, int.Parse(txtQty.Text), double.Parse(txtExRate.Text), cost);
        }

        private List<PartQty> SortParts(List<PartQty> partList)
        {
            return partList.OrderBy(o => o.TypeId).ToList();
        }

        private void btnGetSs_Click(object sender, RoutedEventArgs e)
        {
            GetSpreadsheet();
        }

        private void txtExRate_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Verifier.HasIllegalChars(true, e);
        }

        private void txtQty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Verifier.HasIllegalChars(false, e);
        }
    }
}
