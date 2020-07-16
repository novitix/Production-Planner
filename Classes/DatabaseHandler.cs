using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Production_Planner
{
    class DatabaseHandler
    {
        static private readonly string db_loc = "database.db";
        private static string GetConString()
        {
            return string.Format("Data Source={0}", db_loc);
        }
        public static SqliteDataReader GetReader(string sql)
        {
            string con_str = GetConString();
            var con = new SqliteConnection(con_str);
            con.Open();
            var cmd = new SqliteCommand(sql, con);
            var reader = cmd.ExecuteReader();
            return reader;
        }
        public static void ExecuteSql(string sqlStr)
        {
            string con_str = GetConString();
            var con = new SqliteConnection(con_str);
            con.Open();
            var cmd = new SqliteCommand(sqlStr, con);
            cmd.ExecuteNonQuery();
            con.Close();
        }
        public static ObservableCollection<Product> GetAllProducts()
        {
            var res = new ObservableCollection<Product>();
            string sql = @"SELECT * FROM products";
            var reader = GetReader(sql);
            while (reader.Read())
            {
                var prod = new Product(reader.GetInt32(0), reader.GetString(1), reader.GetDouble(2));
                res.Add(prod);
            }

            return res;
        }

        private class PartFindRes
        {
            public int PartId { get; set; }
            public int Qty { get; set; }

            public PartFindRes(int partId, int qty)
            {
                PartId = partId;
                Qty = qty;
            }
        }

        public static List<PartQty> GetParts(Product prod)
        {
            int id = prod.Id;
            var partFindList = new List<PartFindRes>();

            string sql = string.Format(@"SELECT part_id, qty FROM part_find WHERE product_id='{0}'", id);
            var reader = GetReader(sql);
            while (reader.Read())
            {
                partFindList.Add(new PartFindRes(reader.GetInt32(0), reader.GetInt32(1)));
            }

            // found part_ids in partFindList. Now use part id to find part details (e.g. name).
            var ids_list = partFindList.Select(o => o.PartId.ToString()).ToArray();
            string concatIds = string.Join(", ", ids_list);
            sql = string.Format(@"SELECT * FROM parts WHERE id IN ({0})", concatIds);
            var res = new List<PartQty>();
            reader = GetReader(sql);
            while (reader.Read())
            {
                var part_id = reader.GetInt32(0);
                var part_name = reader.GetString(1);
                var part_type = reader.GetInt32(2);

                var part_qty = partFindList.Where(i => i.PartId == part_id).FirstOrDefault().Qty;
                if (part_qty != default)
                {
                    res.Add(new PartQty(part_id, part_name, part_type, part_qty));
                }
                
            }
            return res;
        }

        public static List<Part> GetParts()
        {
            string sql = "SELECT * FROM parts";
            List<Part> res = new List<Part>();
            var partTypes = GetAllPartTypes();
            var reader = GetReader(sql);
            while (reader.Read())
            {
                var partId = reader.GetInt32(0);
                var partName = reader.GetString(1);
                var partTypeId = reader.GetInt32(2);
                var partTypeName = partTypes.Find(s => s.Id == partTypeId).TypeName;
                res.Add(new Part(partId, partName, partTypeId));
            }
            return res;
        }

        public static Product GetLastProduct()
        {
            string sql = "SELECT * FROM products ORDER BY ID DESC LIMIT 1";
            var reader = GetReader(sql);
            Product res = new Product();
            while (reader.Read())
            {
                res = new Product(reader.GetInt32(0), reader.GetString(1), reader.GetDouble(2));
            }

            return res;
        }

        public static string GetPartTypeName(int id)
        {
            string sql = string.Format("SELECT * FROM part_type WHERE id='{0}' LIMIT 1", id);
            var reader = GetReader(sql);
            reader.Read();
            string res = reader.GetString(1);
            return res;
        }

        public static List<PartType> GetAllPartTypes()
        {
            string sql = string.Format("SELECT * FROM part_type");
            var reader = GetReader(sql);
            var res = new List<PartType>();
            while(reader.Read())
            {
                res.Add(new PartType(reader.GetInt32(0), reader.GetString(1)));
            }
            return res;
        }

        //public static List<ProductQty> GetOrderList()
        //{

        //}

    }
}
