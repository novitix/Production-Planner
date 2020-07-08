﻿using System;
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

namespace Production_Planner
{
    /// <summary>
    /// Interaction logic for ProductWindow.xaml
    /// </summary>
    public partial class ProductWindow : Window
    {
        private Product currentProd;
        private List<Part> partsNeeded;
        public ProductWindow(Product selProduct)
        {
            InitializeComponent();
            currentProd = selProduct;
            partsNeeded = DatabaseHandler.GetParts(currentProd);
        }

        private void GetSpreadsheet()
        {
            ExcelWriter exWrite = new ExcelWriter();
            exWrite.WriteToExcel(partsNeeded, @"spreadsheet.xlsx");
        }

        private void txtExRate_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txtQty_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnGetSs_Click(object sender, RoutedEventArgs e)
        {
            GetSpreadsheet();
        }
    }
}