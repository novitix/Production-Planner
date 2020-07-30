using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;
using _IO = System.IO;

namespace Production_Planner
{
    class DBHandler
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
            var reader = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
            return reader;
        }
        public static void ExSql(string sqlStr, bool updateBackUps = true)
        {
            string con_str = GetConString();
            var con = new SqliteConnection(con_str);
            con.Open();
            var cmd = new SqliteCommand(sqlStr, con);
            cmd.ExecuteNonQuery();
            con.Close();

            if (updateBackUps) UpdateBackups();
        }

        public static void ExSql(List<string> sqlStr)
        {
            foreach (string sql in sqlStr)
            {
                ExSql(sql, false);
            }

            UpdateBackups();
        }
        public static List<Product> GetAllProducts()
        {
            var res = new List<Product>();
            string sql = @"SELECT * FROM products";
            using (SqliteDataReader reader = GetReader(sql))
            {
                while (reader.Read())
                {
                    var prod = new Product(reader.GetInt32(0), reader.GetString(1), reader.GetDouble(2));
                    res.Add(prod);
                }
            }
                

            return res;
        }

        private class PartFindRes
        {
            public int PartId { get; set; }
            public double Qty { get; set; }

            public PartFindRes(int partId, double qty)
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
            using (SqliteDataReader reader = GetReader(sql))
            {
                while (reader.Read())
                {
                    partFindList.Add(new PartFindRes(reader.GetInt32(0), reader.GetDouble(1)));
                }
            }

            // found part_ids in partFindList. Now use part id to find part details (e.g. name).
            var ids_list = partFindList.Select(o => o.PartId.ToString()).ToArray();
            string concatIds = string.Join(", ", ids_list);
            sql = string.Format(@"SELECT * FROM parts WHERE id IN ({0})", concatIds);
            var res = new List<PartQty>();
            using (SqliteDataReader reader = GetReader(sql))
            {
                while (reader.Read())
                {
                    var part_id = reader.GetInt32(0);
                    var part_name = reader.GetString(1);
                    var part_type_id = reader.GetInt32(2);

                    var part_qty = partFindList.Where(i => i.PartId == part_id).FirstOrDefault().Qty;
                    if (part_qty != default)
                    {
                        res.Add(new PartQty(part_id, part_name, new PartType(part_type_id), part_qty));
                    }
                
                }
            }
                
            return res;
        }

        public static List<Part> GetParts()
        {
            string sql = "SELECT * FROM parts";
            List<Part> res = new List<Part>();
            var partTypes = GetAllPartTypes();
            using (SqliteDataReader reader = GetReader(sql))
            {
                while (reader.Read())
                {
                    var partId = reader.GetInt32(0);
                    var partName = reader.GetString(1);
                    var partTypeId = reader.GetInt32(2);
                    var partTypeName = partTypes.Find(s => s.Id == partTypeId).TypeName;
                    res.Add(new Part(partId, partName, new PartType(partTypeId)));
                }
            }
            return res;
        }

        public static Product GetLastProduct()
        {
            string sql = "SELECT * FROM products ORDER BY ID DESC LIMIT 1";
            Product res;
            using (SqliteDataReader reader = GetReader(sql))
            {
                reader.Read();
                res = new Product(reader.GetInt32(0), reader.GetString(1), reader.GetDouble(2));
            }
            return res;
        }

        public static string GetPartTypeName(int id)
        {
            string sql = string.Format("SELECT * FROM part_type WHERE id='{0}' LIMIT 1", id);
            string res;
            using (SqliteDataReader reader = GetReader(sql))
            {
                reader.Read();
                res = reader.GetString(1);
            }
            return res;
        }

        public static List<PartType> GetAllPartTypes()
        {
            string sql = string.Format("SELECT * FROM part_type");
            var res = new List<PartType>();
            using (SqliteDataReader reader = GetReader(sql))
            {
                while (reader.Read())
                {
                    res.Add(new PartType(reader.GetInt32(0), reader.GetString(1)));
                }
            }
            return res;
        }

        public static Product GetProduct(int id)
        {
            string sql = string.Format("SELECT * FROM products WHERE id='{0}' LIMIT 1", id);
            Product res;
            using (SqliteDataReader reader = GetReader(sql))
            {
                reader.Read();
                
                res = new Product(id, reader.GetString(1), reader.GetDouble(2));
            }
            return res;
        }

        public static List<ProductQty> GetOrderList()
        {
            var res = new List<ProductQty>();
            string sql = string.Format("SELECT * FROM order_list");
            using (var reader = GetReader(sql))
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    Product prod = GetProduct(id);
                    res.Add(new ProductQty(prod, reader.GetInt32(1)));
                }
            }
            return res;
        }

        public static void UpdateBackups()
        {
            

            string tempPath = _IO.Path.GetTempPath();
            string savePath = _IO.Path.Combine(tempPath, "ProdPlannerDBBackups");
            _IO.Directory.CreateDirectory(savePath);
            string fileName = Guid.NewGuid().ToString() + ".db";
            string saveFilePath = _IO.Path.Combine(savePath, fileName);

            // don't create backup if the db has been modified in the last 5 minutes
            string[] backupFiles = _IO.Directory.GetFiles(savePath);
            bool createBackup = true;
            foreach (string backupFile in backupFiles)
            {
                _IO.FileInfo fi = new _IO.FileInfo(backupFile);
                if (fi.CreationTime > DateTime.Now.AddMinutes(-5))
                {
                    createBackup = false;
                    break;
                }
            }
            if (!createBackup) return;

            //create backup
            _IO.File.Copy(db_loc, saveFilePath);

            // remove old backups
            backupFiles = _IO.Directory.GetFiles(savePath);
            foreach (string backupFile in backupFiles)
            {
                _IO.FileInfo fi = new _IO.FileInfo(backupFile);
                
                if (fi.CreationTime < DateTime.Now.AddDays(-30)) //remove files with creation lime longer than 30 days ago
                {
                    fi.Delete();
                }
            }
        }

        public static void PushProdPtsToDb(Product prod, List<PartQty> parts)
        {
            // clear product parts first
            DBHandler.ExSql(string.Format("DELETE FROM part_find WHERE product_id={0}", prod.Id));

            // re-add updated product's parts
            foreach (PartQty part in parts)
            {
                string sql = string.Format("INSERT INTO part_find (product_id, part_id, qty) VALUES({0}, {1}, {2})", prod.Id, part.Id, part.OrderQty);
                ExSql(sql);
            }
        }

    }
}
