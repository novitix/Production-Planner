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

        public Product() { }

        public Product(int id, string name, double cost_rmb)
        {
            Id = id;
            Name = name;
            Cost_rmb = cost_rmb;
        }
    }


    public class Part
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        private string _typeName;

        public Part(int id, string name, int type)
        {
            Id = id;
            Name = name;
            Type = type;
        }

        public string GetTypeName()
        {
            if (string.IsNullOrEmpty(_typeName))
            {
                _typeName = DatabaseHandler.GetPartTypeName(this.Id);
            }

            return _typeName;
        }
    }

    public class PartQty : Part
    {
        public int OrderQty { get; set; }
        public PartQty(int id, string name, int type, int orderQty)
            : base(id, name, type)
        {
            this.OrderQty = orderQty;
        }
    }

    public class PartType
    {
        public int Id { get; set; }
        public string TypeName { get; set; }

        public PartType(int id, string typeName)
        {
            Id = id;
            TypeName = typeName;
        }
    }
}
