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
        }
        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            OpenProductWindow();
        }

        private void OpenProductWindow()
        {
            var selProd = (Product)disp_products.SelectedItem;
            ProductWindow wnd = new ProductWindow(selProd);
            wnd.Owner = this;
            wnd.ShowDialog();
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
            foreach (ProductQty orderItem in lbOrderProds.Items)
            {
                string sql = string.Format("INSERT OR REPLACE INTO order_list (product_id, qty) VALUES({0}, {1})", orderItem.Id, orderItem.Qty);
                DatabaseHandler.ExecuteSql(sql);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            PushOrderList();
        }
    }

}
