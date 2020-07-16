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
            txtExRate.Text = Properties.Settings.Default.ExRate.ToString();
            txtQty.SelectAll();
        }

        private string GetSpreadsheet()
        {
            if (String.IsNullOrEmpty(txtExRate.Text) || string.IsNullOrEmpty(txtQty.Text))
            {
                MessageBox.Show("Please enter exchange rate and quantity.");
                return string.Empty;
            }
            ExcelWriter exWrite = new ExcelWriter();
            double cost = currentProd.CostRmb * int.Parse(txtQty.Text);
            partsNeeded = SortParts(partsNeeded);

            string name = Guid.NewGuid().ToString() + ".xlsx";

            string path = (Properties.Settings.Default.SpreadsheetLocation == "Documents") ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) : _IO.Path.GetFullPath(Properties.Settings.Default.SpreadsheetLocation);
            path = _IO.Path.Combine(path, name);

            exWrite.WriteToExcel(partsNeeded, path, int.Parse(txtQty.Text), double.Parse(txtExRate.Text), cost);
            return path;
        }

        private void RunAndWait(string path)
        {
            var proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = "explorer.exe";
            proc.StartInfo.Arguments = string.Format(@"/select,""{0}""", path);
            proc.EnableRaisingEvents = true;
            proc.Start();
            proc.Close();
        }


        private List<PartQty> SortParts(List<PartQty> partList)
        {
            return partList.OrderBy(o => o.TypeId).ToList();
        }

        private void btnGetSs_Click(object sender, RoutedEventArgs e)
        {
            var path = GetSpreadsheet();
            if (path != string.Empty)
            {
                RunAndWait(path);
            }
        }

        private void txtExRate_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Verifier.HasIllegalChars(true, e);
        }

        private void txtQty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Verifier.HasIllegalChars(false, e);
        }

        private void txtExRate_TextChanged(object sender, TextChangedEventArgs e)
        {
            Properties.Settings.Default.ExRate = decimal.Parse(txtExRate.Text);
            Properties.Settings.Default.Save();
        }

        private void txtQty_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
