using Microsoft.Data.Sqlite;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using _IO = System.IO;

namespace Production_Planner
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<ProductQty> orderList;
        public MainWindow()
        {
            InitializeComponent();
            RefreshProductList();
            RefreshOrderList();
            txtExRate.Text = Properties.Settings.Default.ExRate.ToString();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selItem = disp_products.SelectedItem as Product;
            if (orderList.Any(o => o.Id == selItem.Id))
            {
                orderList.Where(o => o.Id == selItem.Id).First().Qty++;
            }
            else
            {
                orderList.Add(new ProductQty(selItem, 1));
            }
            
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            OpenAddWindow();
        }

        private void OpenAddWindow()
        {
            AddWindow wnd = new AddWindow();
            wnd.Owner = this;
            wnd.Show();
        }

        public void RefreshProductList()
        {
            disp_products.ItemsSource = DatabaseHandler.GetAllProducts();
        }

        public void RefreshOrderList()
        {
            orderList = new ObservableCollection<ProductQty>(DatabaseHandler.GetOrderList());
            lbOrderProds.ItemsSource = orderList;
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow wnd = new SettingsWindow();
            wnd.Owner = this;
            wnd.ShowDialog();
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {

        }

        private void txtItemQty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
             e.Handled = Verifier.HasIllegalChars(false, e);
        }

        private void txtItemQty_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void PushOrderList()
        {
            // clear db order list first
            string delSql = "DELETE FROM order_list";
            DatabaseHandler.ExecuteSql(delSql);

            foreach (ProductQty orderItem in lbOrderProds.Items)
            {
                string insSql = string.Format("INSERT INTO order_list (product_id, qty) VALUES({0}, {1})", orderItem.Id, orderItem.Qty);
                DatabaseHandler.ExecuteSql(insSql);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            PushOrderList();
        }

        private string GetSpreadsheet()
        {
            if (String.IsNullOrEmpty(txtExRate.Text))
            {
                MessageBox.Show("Please enter exchange rate and quantity.");
                return string.Empty;
            }
            ExcelWriter exWrite = new ExcelWriter();
            string name = Guid.NewGuid().ToString() + ".xlsx";

            string path = (Properties.Settings.Default.SpreadsheetLocation == "Documents") ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) : _IO.Path.GetFullPath(Properties.Settings.Default.SpreadsheetLocation);
            path = _IO.Path.Combine(path, name);

            exWrite.WriteToExcel(new List<ProductQty>(orderList), path, double.Parse(txtExRate.Text), GetCost());
            return path;
        }

        

        private double GetCost()
        {
            double sum = 0.0;
            foreach(var item in orderList)
            {
                sum += item.CostRmb * item.Qty;
            }

            return sum;
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

        private void txtExRate_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Verifier.HasIllegalChars(true, e);
        }

        private void txtExRate_TextChanged(object sender, TextChangedEventArgs e)
        {
            Properties.Settings.Default.ExRate = decimal.Parse(txtExRate.Text);
            Properties.Settings.Default.Save();
        }

        private void btnGetSs_Click_1(object sender, RoutedEventArgs e)
        {
            var path = GetSpreadsheet();
            if (path != string.Empty)
            {
                RunAndWait(path);
            }
        }

        private void txtItemQty_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void lbOrderProds_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsQtyBoxFocused || e.Key != Key.Delete || lbOrderProds.SelectedIndex == -1) return;
            orderList.RemoveAt(lbOrderProds.SelectedIndex);
        }

        bool IsQtyBoxFocused = false;
        private void txtItemQty_GotFocus(object sender, RoutedEventArgs e)
        {
            IsQtyBoxFocused = true;
        }

        private void txtItemQty_LostFocus(object sender, RoutedEventArgs e)
        {
            IsQtyBoxFocused = false;
        }
    }

}
