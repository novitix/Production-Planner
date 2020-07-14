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
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;

namespace Production_Planner
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : System.Windows.Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            txtLocation.Text = Properties.Settings.Default.SpreadsheetLocation;
        }

        private void btnChooseFolder_Click(object sender, RoutedEventArgs e)
        {
            var folDialog = new FolderBrowserDialog();

            if (Properties.Settings.Default.SpreadsheetLocation == "unset")
            {
                folDialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            DialogResult result = folDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                txtLocation.Text = folDialog.SelectedPath;
                Properties.Settings.Default.SpreadsheetLocation = folDialog.SelectedPath;
                Properties.Settings.Default.Save();
            }
        }
    }
}
