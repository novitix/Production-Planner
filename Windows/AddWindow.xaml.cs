﻿using System;
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
            partTypes = new ObservableCollection<PartType>(DatabaseHandler.GetAllPartTypes());
            cbPartType.ItemsSource = partTypes;
        }

        private void UpdatePartList()
        {
            partList = new ObservableCollection<Part>(DatabaseHandler.GetParts());
            cbPartsList.ItemsSource = partList;
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

            int ptQty;
            if (int.TryParse(txtPtQty.Text, out ptQty) == false)
            {
                MessageBox.Show("Please enter part quantity.");
                return;
            }
            prodPtList.Add(new PartQty(selItem.Id, selItem.Name, selItem.TypeId, ptQty));

            cbPartsList.SelectedIndex = -1;
            txtPtQty.Clear();
            cbPartsList.Focus();
            SetStatus("Part added to Product successfully");
        }

        private void AddProduct()
        {
            string sqlStr = string.Format(@"INSERT INTO products (name, cost_rmb) VALUES ('{0}', '{1}')", txtProductName.Text, txtProdCost.Text);
            DatabaseHandler.ExecuteSql(sqlStr);
        }

        private void btnAddProd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProdCost.Text) || string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                MessageBox.Show("Please enter Product Name and Product Cost");
                return;
            }
            AddProduct();
            Product lastProd = DatabaseHandler.GetLastProduct();
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
                DatabaseHandler.ExecuteSql(sqlStr);
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
            DatabaseHandler.ExecuteSql(sqlStr);
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
    }
}