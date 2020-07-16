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
            RefreshProductList();
            SetOrderList();
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
            OpenProductWindow();
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

        public void SetOrderList()
        {
            //lbOrderProds.ItemsSource = test;
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
    }

}
