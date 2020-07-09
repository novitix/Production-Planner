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
        public void WriteToExcel(List<PartQty> parts, string name)
        {
            this.wb = excel.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            this.ws = wb.Worksheets[1];
            WriteColumns();
            WriteParts(parts);
            wb.SaveAs(name);
            this.wb.Close();
        }

        private void WriteColumns()
        {
            string[] columnHeaders = { "", "Arrived", "Ordered", "Products", "Order Qty", "Notes", "Comments" };
            int[] columnWidth = { 5, 10, 10, 47, 15, 35, 52 };
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

        private void WriteParts(List<PartQty> parts)
        {
            for (int i = 0; i < parts.Count; i++)
            {
                this.ws.Cells[i + 2, 4].Value2 = parts[i].Name;
                this.ws.Cells[i + 2, 5].Value2 = parts[i].Order_qty;
            }
        }
    }
}
