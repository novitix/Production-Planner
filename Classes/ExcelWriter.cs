using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _IO = System.IO;
using Microsoft.Office.Interop.Excel;
using _Excel = Microsoft.Office.Interop.Excel;


namespace Production_Planner
{
    class ExcelWriter
    {
        _Application excel = new _Excel.Application();
        Workbook wb;
        Worksheet ws;
        public void WriteToExcel(List<PartQty> parts, string path, int multiplier, double exRate, double cost)
        {
            this.wb = excel.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            this.ws = wb.Worksheets[1];
            WriteColumns();
            WriteParts(parts, multiplier);

            writeCost(parts.Count + 2, exRate, cost);
            wb.SaveAs(path);
            this.wb.Close();
        }

        private void WriteColumns()
        {
            string[] columnHeaders = { "", "Arrived", "Ordered", "Products", "Order Qty", "Product Type", "Notes", "Comments" };
            int[] columnWidth = { 5, 10, 10, 47, 15, 15, 35, 52 };
            for (int i = 0; i < columnHeaders.Length; i++)
            {
                this.ws.Cells[1, i+1].Value2 = columnHeaders[i];
                this.ws.Cells[1, i + 1].Font.Bold = true;
                string columnId = intToChar(i);
                this.ws.Columns[columnId + ":" + columnId].ColumnWidth = columnWidth[i];
            }
        }
        private string intToChar(int number)
        {
            // Converts integer to corresponding letter e.g. 0 => A, 1 => B. This is needed as columns in Excel are named using uppercase characters.
            number = number + 65;
            return ((char)number).ToString();
        }

        private void WriteParts(List<PartQty> parts, int multiplier)
        {
            for (int i = 0; i < parts.Count; i++)
            {
                this.ws.Cells[i + 2, 4].Value2 = parts[i].Name;
                this.ws.Cells[i + 2, 5].Value2 = parts[i].OrderQty * multiplier;
                this.ws.Cells[i + 2, 6].Value2 = DatabaseHandler.GetPartTypeName(parts[i].TypeId);
            }
        }

        private void writeCost(int lastFilledRow, double exRate, double rmbCost)
        {
            double audCost = Math.Round(rmbCost / exRate, 2);
            rmbCost = Math.Round(rmbCost, 2);
            this.ws.Cells[lastFilledRow + 1, 4].Value2 = string.Format("Total Cost: {0} AUD ({1} RMB)", audCost, rmbCost);
        }
    }
}
