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
        public MainWindow()
        {
            InitializeComponent();
            disp_products.ItemsSource = DatabaseHandler.GetProducts();
        }
        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            OpenProductWindow();
        }

        private void OpenProductWindow()
        {
            var selProd = (Product)disp_products.SelectedItem;
            ProductWindow wnd = new ProductWindow(selProd);
            wnd.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenProductWindow();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            OpenAddWindow();
        }

        private void OpenAddWindow()
        {
            AddWindow wnd = new AddWindow();
            wnd.Show();
        }
    }
}
