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
        public void WriteToExcel(List<ProductQty> prods, string path, double exRate, double cost)
        {
            ColumnDef[] partsCols = new ColumnDef[]
               {
                new ColumnDef("", 5),
                new ColumnDef("Arrived", 10),
                new ColumnDef("Ordered", 10),
                new ColumnDef("Parts", 47),
                new ColumnDef("Order Qty", 15),
                new ColumnDef("Product Type", 15),
                new ColumnDef("Notes", 35),
                new ColumnDef("Comments", 52)
               };
            ColumnDef[] prodsCols = new ColumnDef[]
            {
                new ColumnDef("Products", -1),
                new ColumnDef("Qty", -1),
                new ColumnDef("Total Cost", -1)
            };

            this.wb = excel.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            this.ws = wb.Worksheets[1];

            int lastFilledRow = 1;
            int margin = 1;
            lastFilledRow += WriteColumns(partsCols, lastFilledRow, 1);
            lastFilledRow += WriteParts(GetSortedPartsList(prods), lastFilledRow);
            lastFilledRow += margin;
            lastFilledRow += writeCost(exRate, cost, lastFilledRow);
            lastFilledRow += margin;
            lastFilledRow += WriteColumns(prodsCols, lastFilledRow, 4);
            lastFilledRow += WriteProducts(prods, lastFilledRow);
            wb.SaveAs(path);
            this.wb.Close();
        }

        private int WriteProducts(List<ProductQty> prods, int startingRow)
        {
            for (int i = 0; i < prods.Count; i++)
            {
                this.ws.Cells[startingRow + i, 4] = prods[i].Name;
                this.ws.Cells[startingRow + i, 5] = prods[i].Qty;
                this.ws.Cells[startingRow + i, 6] = prods[i].TotalCost;
            }

            Range c1 = ws.Cells[startingRow, 4];
            Range c2 = ws.Cells[startingRow + prods.Count - 1, 6];
            BorderCells(c1, c2);

            return prods.Count;
        }

        private int WriteColumns(ColumnDef[] cols, int startingRow, int startingColumn)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                this.ws.Cells[startingRow, i + startingColumn].Value2 = cols[i].Name;
                this.ws.Cells[startingRow, i + startingColumn].Font.Bold = true;
                this.ws.Cells[startingRow, i + startingColumn].Borders.Weight = _Excel.XlBorderWeight.xlThin;
                string columnId = intToChar(i);
                if (cols[i].Width != -1) this.ws.Columns[columnId + ":" + columnId].ColumnWidth = cols[i].Width;
            }
            Range c1 = ws.Cells[startingRow, startingColumn];
            Range c2 = ws.Cells[startingRow, startingColumn + cols.Length - 1];
            BorderCells(c1, c2);
            return 1;
        }

        private void BorderCells(Range c1, Range c2)
        {
            Range range = (Range)ws.get_Range(c1, c2);
            foreach (_Excel.Range cell in range.Cells)
            {
                cell.BorderAround2();
            }
        }

        private string IntsToExcelFormat(int row, int col)
        {
            // converts 1, 1 to A1;
            return intToChar(col) + row.ToString();
        }
        private string intToChar(int number)
        {
            // Converts integer to corresponding letter e.g. 0 => A, 1 => B. This is needed as columns in Excel are named using uppercase characters.
            number = number + 65;
            return ((char)number).ToString();
        }

        private int WriteParts(List<PartQty> parts, int startingRow)
        {
            for (int i = 0; i < parts.Count; i++)
            {
                this.ws.Cells[i + startingRow, 4].Value2 = parts[i].Name;
                this.ws.Cells[i + startingRow, 5].Value2 = parts[i].OrderQty;
                this.ws.Cells[i + startingRow, 6].Value2 = DatabaseHandler.GetPartTypeName(parts[i].TypeId);
            }

            Range c1 = ws.Cells[startingRow, 1];
            Range c2 = ws.Cells[startingRow + parts.Count - 1, 8];
            BorderCells(c1, c2);

            return parts.Count;
        }

        private int writeCost(double exRate, double rmbCost, int startingRow)
        {
            double audCost = Math.Round(rmbCost / exRate, 2);
            rmbCost = Math.Round(rmbCost, 2);
            this.ws.Cells[startingRow, 4].Value2 = string.Format("Total Cost: {0} AUD ({1} RMB)", audCost, rmbCost);

            Range c1 = ws.Cells[startingRow, 4];
            BorderCells(c1, c1);

            return 1; // 1 extra row is added
        }

        private List<PartQty> GetSortedPartsList(List<ProductQty> orderList)
        {
            var parts = new List<PartQty>();
            foreach (ProductQty prod in orderList)
            {
                var prodParts = DatabaseHandler.GetParts(prod);
                foreach (var part in prodParts)
                {
                    if (parts.Any(o => o.Id == part.Id))
                    {
                        parts.FindLast(o => o.Id == part.Id).OrderQty += part.OrderQty * prod.Qty;
                    }
                    else
                    {
                        parts.Add(new PartQty(part.Id, part.Name, part.TypeId, part.OrderQty * prod.Qty));
                    }
                }
            }

            parts = parts.OrderBy(o => o.TypeId).ToList();
            return parts;
        }

        private class ColumnDef
        {
            public string Name { get; set; }
            public int Width { get; set; }
            public ColumnDef(string name, int width)
            {
                Name = name;
                Width = width;
            }
        }
    }
}
