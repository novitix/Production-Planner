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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            PushProductsToDb();
            PushPartsToDb();
            PushPartTypesToDb();
        }

        #region ProductTab
        private ObservableCollection<Part> partList;
        private ObservableCollection<Product> prodList;
        private ObservableCollection<PartQty> prodPtList;
        
        private void UpdatePartList()
        {
            partList = new ObservableCollection<Part>(DBHandler.GetParts());
            cbPartsList.ItemsSource = partList;
            cbModPartList.ItemsSource = partList;
        }

        private void UpdateProdsList()
        {
            prodList = new ObservableCollection<Product>(DBHandler.GetAllProducts());
            cbProdList.ItemsSource = prodList;
        }

        private void UpdateProdPtList()
        {
            if (cbProdList.SelectedItem == null) return;
            prodPtList = new ObservableCollection<PartQty>(DBHandler.GetParts(cbProdList.SelectedItem as Product));
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

            double ptQty;
            if (double.TryParse(txtPtQty.Text, out ptQty) == false)
            {
                MessageBox.Show("Please enter part quantity.");
                return;
            }

            if (cbProdList.SelectedIndex == -1)
            {
                MessageBox.Show("Please choose product.");
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
            DBHandler.PushProdPtsToDb(cbProdList.SelectedItem as Product, new List<PartQty>(prodPtList));
        }
        private void txtPtQty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Verifier.HasIllegalChars(true, e);
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
                DBHandler.ExSql(sql);
            }
            
        }

        private void lbPartsList_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsQtyBoxFocused || e.Key != Key.Delete || lbPartsList.SelectedIndex == -1) return;

            prodPtList.RemoveAt(lbPartsList.SelectedIndex);
            DBHandler.PushProdPtsToDb(cbProdList.SelectedItem as Product, new List<PartQty>(prodPtList));
        }

        private void btnDelProd_Click(object sender, RoutedEventArgs e)
        {
            if (cbProdList.SelectedIndex == -1) return;
            if (MessageBox.Show("Are you sure you want to delete this product?", "Confirm Delete", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
            int prodId = (cbProdList.SelectedItem as Product).Id;
            List<string> sql = new List<string>();
            sql.Add(string.Format("DELETE FROM products WHERE id={0}", prodId));
            sql.Add(string.Format("DELETE FROM part_find WHERE product_id={0}", prodId));
            sql.Add(string.Format("DELETE FROM order_list WHERE product_id={0}", prodId));
            DBHandler.ExSql(sql);

            cbProdList.SelectedIndex = -1;
            UpdateProdsList();
        }
        #endregion
        #region PartTab

        private ObservableCollection<PartType> partTypeList;
        private void UpdatePartTypeList()
        {

            // sortLst is needed as ObservableCollections cannot be sorted this way.
            var sortLst = DBHandler.GetAllPartTypes();
            sortLst.Sort((i1, i2) => i1.TypeName.CompareTo(i2.TypeName));
            partTypeList = new ObservableCollection<PartType>(sortLst);
            cbPartType.ItemsSource = partTypeList;
            cbModPartTypeList.ItemsSource = partTypeList;
        }

        private void cbModPartList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbPartType.SelectedIndex == -1) return;
            (cbModPartList.SelectedItem as Part).PartType.InvokeIdChanged();
        }

        private void PushPartsToDb()
        {
            string sql;
            foreach (Part item in partList)
            {
                sql = string.Format("UPDATE parts SET name='{0}', type={1} WHERE id={2}", item.Name, item.PartType.Id, item.Id);
                DBHandler.ExSql(sql);
            }
        }
        #endregion

        #region PartTypeTab
        private void PushPartTypesToDb()
        {
            foreach (PartType item in partTypeList)
            {
                string sql = string.Format("UPDATE part_type SET type_name='{0}' WHERE id={1}", item.TypeName, item.Id);
                DBHandler.ExSql(sql);
            }
        }
        #endregion

        private void btnDelPartType_Click(object sender, RoutedEventArgs e)
        {
            if (cbModPartTypeList.SelectedIndex == -1) return;
            if (MessageBox.Show("This will delete all parts that have this part type and remove these parts from existing products. Are you sure you wish to continue?", "Confirmation", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
            int partTypeId = (cbModPartTypeList.SelectedItem as PartType).Id;

            // delete part type
            DBHandler.ExSql(string.Format("DELETE FROM part_type WHERE id={0}", partTypeId));

            // find parts with this part type
            List<int> partIds = new List<int>();
            using (var reader = DBHandler.GetReader(string.Format("SELECT id FROM parts WHERE type={0}", partTypeId)))
            {
                while (reader.Read())
                {
                    int partId = reader.GetInt32(0);
                    partIds.Add(partId);
                }
            }

            // delete parts with this part type
            DBHandler.ExSql(string.Format("DELETE FROM parts WHERE type={0}", partTypeId));

            // delete these parts from products
            DBHandler.ExSql(string.Format("DELETE FROM part_find WHERE part_id IN({0})", ConvertIdListForDb(partIds)));

            cbModPartTypeList.SelectedIndex = -1;
            UpdatePartTypeList();
        }

        private string ConvertIdListForDb(List<int> lst)
        {
            List<string> strLst = lst.ConvertAll(delegate (int i) { return i.ToString(); });
            return "'" + string.Join("', '", strLst) + "'";
        }

        private void btnDelPt_Click(object sender, RoutedEventArgs e)
        {
            if (cbModPartList.SelectedIndex == -1) return;
            if (MessageBox.Show("This will delete the part and remove it from all existing products. Are you sure you wish to continue?", "Confirmation", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;

            int ptId = (cbModPartList.SelectedItem as Part).Id;
            List<string> sql = new List<string>();
            sql.Add(string.Format("DELETE FROM parts WHERE id={0}", ptId));
            sql.Add(string.Format("DELETE FROM part_find WHERE part_id={0}", ptId));
            DBHandler.ExSql(sql);

            cbModPartList.SelectedIndex = -1;
            UpdatePartList();
        }

        private void txtItemQty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Verifier.HasIllegalChars(true, e);
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

        private void txtItemQty_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ( string.IsNullOrWhiteSpace((sender as TextBox).Text) ) return;

            int selProdId = (cbProdList.SelectedItem as Product).Id;
            foreach (PartQty pt in prodPtList)
            {
                string sql = string.Format("UPDATE part_find SET qty={0} WHERE product_id={1} AND part_id={2}", pt.OrderQty, selProdId, pt.Id);
                DBHandler.ExSql(sql);
            }
        }
    }
}