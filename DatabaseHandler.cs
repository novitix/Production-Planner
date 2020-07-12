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
        public static ObservableCollection<Product> GetAllProducts()
        {
            string con_str = GetConString();
            var res = new ObservableCollection<Product>();
            var con = new SqliteConnection(con_str);
            con.Open();

            string sql = @"SELECT * FROM products";
            var cmd = new SqliteCommand(sql, con);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var prod = new Product(reader.GetInt32(0), reader.GetString(1), reader.GetDouble(2));
                res.Add(prod);
            }

            con.Close();
            return res;
        }

        private static string GetConString()
        {
            return string.Format("Data Source={0}", db_loc);
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

            string con_str = GetConString();
            var partFindList = new List<PartFindRes>();
            var con = new SqliteConnection(con_str);
            con.Open();

            string sql = string.Format(@"SELECT part_id, qty FROM part_find WHERE product_id='{0}'", id);
            var cmd = new SqliteCommand(sql, con);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                partFindList.Add(new PartFindRes(reader.GetInt32(0), reader.GetInt32(1)));
            }
            con.Close();


            // found part_ids in partFindList. Now use part id to find part details (e.g. name).
            var partsCon = new SqliteConnection(con_str);
            con.Open();

            var ids_list = partFindList.Select(o => o.PartId.ToString()).ToArray();
            string concatIds = string.Join(", ", ids_list);
            sql = string.Format(@"SELECT * FROM parts WHERE id IN ({0})", concatIds);
            cmd = new SqliteCommand(sql, con);
            var res = new List<PartQty>();

            reader = cmd.ExecuteReader();
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
            string con_str = GetConString();
            var con = new SqliteConnection(con_str);
            string sql = "SELECT * FROM parts";
            con.Open();
            var cmd = new SqliteCommand(sql, con);
            var reader = cmd.ExecuteReader();
            List<Part> res = new List<Part>();

            while (reader.Read())
            {
                var part_id = reader.GetInt32(0);
                var part_name = reader.GetString(1);
                var part_type = reader.GetInt32(2);
                res.Add(new Part(part_id, part_name, part_type));

            }
            return res;
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

        public static Product GetLastProduct()
        {
            string con_str = GetConString();
            var con = new SqliteConnection(con_str);
            string sql = "SELECT * FROM products ORDER BY ID DESC LIMIT 1";
            con.Open();
            var cmd = new SqliteCommand(sql, con);
            var reader = cmd.ExecuteReader();
            Product res = new Product();

            while (reader.Read())
            {
                res = new Product(reader.GetInt32(0), reader.GetString(1), reader.GetDouble(2));
            }

            return res;
        }

        public static string GetPartTypeName(int id)
        {
            string con_str = GetConString();
            var con = new SqliteConnection(con_str);
            string sql = string.Format("SELECT * FROM part_type WHERE id='{0}' LIMIT 1", id);
            con.Open();
            var cmd = new SqliteCommand(sql, con);
            var reader = cmd.ExecuteReader();
            reader.Read();
            string res = reader.GetString(1);
            con.Close();
            return res;
        }

        public static List<PartType> GetAllPartTypes()
        {
            string con_str = GetConString();
            var con = new SqliteConnection(con_str);
            string sql = string.Format("SELECT * FROM part_type");
            con.Open();
            var cmd = new SqliteCommand(sql, con);
            var reader = cmd.ExecuteReader();
            var res = new List<PartType>();
            while(reader.Read())
            {
                res.Add(new PartType(reader.GetInt32(0), reader.GetString(1)));
            }
            con.Close();
            return res;
        }
    }
}
