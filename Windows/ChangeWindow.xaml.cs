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

namespace Production_Planner.Windows
{
    /// <summary>
    /// Interaction logic for ChangeWindow.xaml
    /// </summary>
    public partial class ChangeWindow : Window
    {
        public ChangeWindow()
        {
            InitializeComponent();
            UpdateProdsList();
            UpdatePartList();
            UpdatePartTypeList();
        }

        #region ProductTab
        private ObservableCollection<Part> partList;
        private ObservableCollection<Product> prodList;
        private ObservableCollection<PartQty> prodPtList;
        
        private void UpdatePartList()
        {
            partList = new ObservableCollection<Part>(DatabaseHandler.GetParts());
            cbPartsList.ItemsSource = partList;
            cbModPartList.ItemsSource = partList;
        }

        private void UpdateProdsList()
        {
            prodList = new ObservableCollection<Product>(DatabaseHandler.GetAllProducts());
            cbProdList.ItemsSource = prodList;
        }

        private void UpdateProdPtList()
        {
            if (cbProdList.SelectedItem == null) return;
            prodPtList = new ObservableCollection<PartQty>(DatabaseHandler.GetParts(cbProdList.SelectedItem as Product));
            lbPartsList.ItemsSource = prodPtList;
        }

        private void cbProdList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProdPtList();
        }

        private void btnPtToProd_Click(object sender, RoutedEventArgs e)
        {
            Part selItem = (Part)cbPartsList.SelectedItem;
            if (selItem == null)
            {
                MessageBox.Show("Part not Found!");
                return;
            }

            int ptQty;
            if (int.TryParse(txtPtQty.Text, out ptQty) == false)
            {
                MessageBox.Show("Please enter part quantity.");
                return;
            }

            if (prodPtList.Any(o => o.Id == selItem.Id))
            {
                prodPtList.First(o => o.Id == selItem.Id).OrderQty += ptQty;
            }
            else
            {
                prodPtList.Add(new PartQty(selItem, ptQty));
            }
            PushProdPtsToDb(cbProdList.SelectedItem as Product, new List<PartQty>(prodPtList));
        }
        private void txtPtQty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Verifier.HasIllegalChars(false, e);
        }

        private void txtProdCost_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Verifier.HasIllegalChars(true, e);
        }

        private void PushProductsToDb()
        {
            string sql;
            foreach (var item in prodList)
            {
                sql = string.Format("UPDATE products SET name='{0}', cost_rmb='{1}' WHERE id={2}", item.Name, item.CostRmb, item.Id);
                DatabaseHandler.ExecuteSql(sql);
            }
            
        }

        private void PushProdPtsToDb(Product prod, List<PartQty> parts)
        {
            // clear product parts first
            DatabaseHandler.ExecuteSql(string.Format("DELETE FROM part_find WHERE product_id={0}", prod.Id));

            // re-add updated product's parts
            foreach (PartQty part in parts)
            {
                string sql = string.Format("INSERT INTO part_find (product_id, part_id, qty) VALUES({0}, {1}, {2})", prod.Id, part.Id, part.OrderQty);
                DatabaseHandler.ExecuteSql(sql);
            }
        }

        private void lbPartsList_KeyDown(object sender, KeyEventArgs e)
        {
            if ( (e.Key == Key.Delete) && (lbPartsList.SelectedIndex != -1) )
            {
                prodPtList.RemoveAt(lbPartsList.SelectedIndex);
                PushProdPtsToDb(cbProdList.SelectedItem as Product, new List<PartQty>(prodPtList));
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            PushProductsToDb();
        }

        private void btnDelProd_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this product?", "Confirm Delete", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
            int prodId = (cbProdList.SelectedItem as Product).Id;
            List<string> sql = new List<string>();
            sql.Add(string.Format("DELETE FROM products WHERE id={0}", prodId));
            sql.Add(string.Format("DELETE FROM part_find WHERE product_id={0}", prodId));
            sql.Add(string.Format("DELETE FROM order_list WHERE product_id={0}", prodId));
            DatabaseHandler.ExecuteSql(sql);

            cbProdList.SelectedIndex = -1;
            UpdateProdsList();
        }
        #endregion
        #region PartTab

        private ObservableCollection<PartType> partTypeList;
        private void UpdatePartTypeList()
        {
            partTypeList = new ObservableCollection<PartType>(DatabaseHandler.GetAllPartTypes());
            cbPartType.ItemsSource = partTypeList;
        }
        #endregion
    }
}
