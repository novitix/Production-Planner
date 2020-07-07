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
        public static ObservableCollection<Product> GetProducts()
        {
            string con_str = string.Format("Data Source={0}", db_loc);
            var res = new ObservableCollection<Product>();
            var con = new SqliteConnection(con_str);
            con.Open();
            
            string sql = @"SELECT * FROM products";
            var cmd = new SqliteCommand(sql, con);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var prod = new Product(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), reader.GetInt32(3));
                res.Add(prod);
            }

            return res;
        }
    }
}
