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

namespace Production_Planner
{
    /// <summary>
    /// Interaction logic for AddWindow.xaml
    /// </summary>
    public partial class AddWindow : System.Windows.Window
    {
        private ObservableCollection<Part> partList;
        private ObservableCollection<PartQty> prodPtList = new ObservableCollection<PartQty>();
        private ObservableCollection<PartType> partTypes;
        public AddWindow()
        {
            InitializeComponent();

            partList = new ObservableCollection<Part>(DatabaseHandler.GetParts());

            cbPartsList.ItemsSource = partList;
            lbPartsList.ItemsSource = prodPtList;

            partTypes = new ObservableCollection<PartType>(DatabaseHandler.GetAllPartTypes());
            cbPartType.ItemsSource = partTypes;
        }

        private void txtPtQty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Verifier.HasIllegalChars(false, e);
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
            DatabaseHandler.ExecuteSql(sqlStr);
        }

        private void btnPtToProd_Click(object sender, RoutedEventArgs e)
        {
            Part selItem = (Part)cbPartsList.SelectedItem;
            if (selItem != null)
            {
                int ptQty;
                if (int.TryParse(txtPtQty.Text, out ptQty))
                {
                    prodPtList.Add(new PartQty(selItem.Id, selItem.Name, selItem.Type, ptQty));
                }
                else
                {
                    MessageBox.Show("Please enter part quantity.");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Part not Found!");
            }
            
        }

        private void AddProduct()
        {
            string sqlStr = string.Format(@"INSERT INTO products (name, cost_rmb) VALUES ('{0}', '{1}')", txtProductName.Text, txtProdCost.Text);
            DatabaseHandler.ExecuteSql(sqlStr);
        }

        private void btnAddProd_Click(object sender, RoutedEventArgs e)
        {
            AddProduct();
            Product lastProd = DatabaseHandler.GetLastProduct();
            AddProductParts(lastProd.Id);
            ((MainWindow)this.Owner).RefreshProductList();
        }

        private void AddProductParts(int productId)
        {
            foreach (PartQty part in prodPtList)
            {
                string sqlStr = string.Format(@"INSERT INTO part_find (product_id, part_id, qty) VALUES ('{0}', '{1}', '{2}')", productId, part.Id, part.OrderQty);
                DatabaseHandler.ExecuteSql(sqlStr);
            }
        }

        private void btnAddPtType_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
