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
using Microsoft.Office.Interop.Excel;
using Microsoft.Vbe.Interop;
using System.Windows.Media.Animation;

namespace Production_Planner
{
    /// <summary>
    /// Interaction logic for AddWindow.xaml
    /// </summary>
    public partial class AddWindow : System.Windows.Window
    {
        private string _statusMessage = "Ready";
        public string StatusMessage {
            get
            {
                return _statusMessage;
            }
            set
            {
                _statusMessage = (String.IsNullOrEmpty(value)) ? "Ready" : value;
            }
        }
        private ObservableCollection<Part> partList;
        private ObservableCollection<PartQty> prodPtList = new ObservableCollection<PartQty>();
        private ObservableCollection<PartType> partTypes;
        public AddWindow()
        {
            InitializeComponent();
            UpdatePartList();
            lbPartsList.ItemsSource = prodPtList;

            UpdatePartTypes();

            SetStatus("Ready");
        }

        private void UpdatePartTypes()
        {
            // sortLst is needed as ObservableCollections cannot be sorted this way.
            var sortLst = DBHandler.GetAllPartTypes();
            sortLst.Sort((i1, i2) => i1.TypeName.CompareTo(i2.TypeName));
            partTypes = new ObservableCollection<PartType>(sortLst);
            cbPartType.ItemsSource = partTypes;
        }

        private void UpdatePartList()
        {
            partList = new ObservableCollection<Part>(DBHandler.GetParts());
            cbPartsList.ItemsSource = partList;
        }

        private void txtPtQty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Verifier.HasIllegalChars(true, e);
        }

        private void txtProdCost_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Verifier.HasIllegalChars(true, e);
        }

        private void btnAddPt_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPartName.Text) || cbPartType.SelectedIndex <= -1)
            {
                MessageBox.Show("Please enter Part Name and a valid Part Type.");
                return;
            }
            PartType selItem = (PartType)cbPartType.SelectedItem;
            var selType = selItem.Id;
            string sqlStr = string.Format(@"INSERT INTO parts (name, type) VALUES ('{0}', '{1}')", txtPartName.Text, selType);
            DBHandler.ExSql(sqlStr);

            UpdatePartList();
            txtPartName.Clear();
            txtPartName.Focus(); 
            SetStatus("Part added successfully");
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

            if (prodPtList.Any(o => o.Id == selItem.Id))
            {
                prodPtList.First(o => o.Id == selItem.Id).OrderQty += ptQty;
            }
            else
            {
                prodPtList.Add(new PartQty(selItem.Id, selItem.Name, selItem.PartType, ptQty));
            }

            cbPartsList.SelectedIndex = -1;
            txtPtQty.Clear();
            cbPartsList.Focus();
            SetStatus("Part added to Product successfully");
        }

        private void AddProduct()
        {
            string sqlStr = string.Format(@"INSERT INTO products (name, cost_rmb) VALUES ('{0}', '{1}')", txtProductName.Text, txtProdCost.Text);
            DBHandler.ExSql(sqlStr);
        }

        private void btnAddProd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProdCost.Text) || string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                MessageBox.Show("Please enter Product Name and Product Cost");
                return;
            }
            AddProduct();
            Product lastProd = DBHandler.GetLastProduct();
            AddProductParts(lastProd.Id);

            txtProductName.Clear();
            txtProdCost.Clear();
            prodPtList.Clear();
            txtProductName.Focus();
            ((MainWindow)this.Owner).RefreshProductList();
            SetStatus("Product added successfully");
        }

        private void AddProductParts(int productId)
        {
            foreach (PartQty part in prodPtList)
            {
                string sqlStr = string.Format(@"INSERT INTO part_find (product_id, part_id, qty) VALUES ('{0}', '{1}', '{2}')", productId, part.Id, part.OrderQty);
                DBHandler.ExSql(sqlStr);
            }
        }

        private void btnAddPtType_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAddPtType.Text))
            {
                MessageBox.Show("Please enter Part Type");
                return;
            }
            string sqlStr = string.Format(@"INSERT INTO part_type (type_name) VALUES ('{0}')", txtAddPtType.Text);
            DBHandler.ExSql(sqlStr);
            UpdatePartTypes();
            txtAddPtType.Clear();
            txtAddPtType.Focus();
            SetStatus("Part Type added successfully");
        }

        private void SetStatus(string status)
        {
            txtStatus.Content = status;
            (this.Resources["sb"] as Storyboard).Begin();
        }

        private void cbPartType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void lbPartsList_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Delete) && (lbPartsList.SelectedIndex != -1))
            {
                prodPtList.RemoveAt(lbPartsList.SelectedIndex);
            }
        }

        private void cbPartsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
