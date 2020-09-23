using Microsoft.Data.Sqlite;
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

namespace Production_Planner.Windows
{
    /// <summary>
    /// Interaction logic for OrderHistoryWindow.xaml
    /// </summary>
    public partial class OrderHistoryWindow : Window
    {
        public OrderHistoryWindow()
        {
            InitializeComponent();
            cbOrderList.ItemsSource = DBHandler.GetAllOrders();
        }

        public bool loadOrder = false;
        public List<ProductQty> selOrderList;

        private void cbOrderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbOrderList.SelectedIndex == -1) return;
            int orderId = (cbOrderList.SelectedItem as Order).Id;
            lbProdList.ItemsSource = DBHandler.GetOrderProductList(orderId);
        }

        private List<ProductQty> GetSelectedOrderList()
        {
            List<ProductQty> res = new List<ProductQty>();
            int orderId = (cbOrderList.SelectedItem as Order).Id;
            string sql = string.Format("SELECT prod_id, qty FROM order_find WHERE order_id={0}", orderId);
            using (SqliteDataReader reader = DBHandler.GetReader(sql))
            {
                while (reader.Read())
                {
                    int prodId = reader.GetInt32(0);
                    Product prod = DBHandler.GetProduct(prodId);
                    ProductQty item = new ProductQty(prod, reader.GetInt32(1));
                    res.Add(item);
                }
            }

            return res;
        }

        private void btnGetSs_Click(object sender, RoutedEventArgs e)
        {
            if (cbOrderList.SelectedIndex == -1)
            {
                MessageBox.Show("No order selected.");
                return;
            }
            var path = (Owner as MainWindow).GetSpreadsheet(GetSelectedOrderList());
            if (!string.IsNullOrEmpty(path))
            {
                (Owner as MainWindow).RunAndWait(path);
            }
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            if (cbOrderList.SelectedIndex == -1)
            {
                MessageBox.Show("No order selected.");
                return;
            }
            int orderId = (cbOrderList.SelectedItem as Order).Id;
            selOrderList = DBHandler.GetOrderProductList(orderId);
            loadOrder = true;
            this.Close();
        }
    }
}
