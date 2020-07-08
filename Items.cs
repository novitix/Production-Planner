using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Production_Planner
{

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Cost_rmb { get; set; }
        public int Qty { get; set; }

        public Product() { }

        public Product(int id, string name, double cost_rmb)
        {
            Id = id;
            Name = name;
            Cost_rmb = cost_rmb;
            Qty = 0;
        }
        public Product(int id, string name, double cost_rmb, int qty)
        {
            Id = id;
            Name = name;
            Cost_rmb = cost_rmb;
            Qty = qty;
        }
    }


    public class Part
    {
        public int Id;
        public string Name;
        public int Order_qty;

        public Part(int id, string name, int order_qty)
        {
            Id = id;
            Name = name;
            Order_qty = order_qty;
        }
    }
}
