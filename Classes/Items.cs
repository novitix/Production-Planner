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
        public double CostRmb { get; set; }

        public Product() { }

        public Product(int id, string name, double costRmb)
        {
            Id = id;
            Name = name;
            CostRmb = costRmb;
        }
    }

    public class ProductQty : Product, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private int _qty;
        public int Qty
        {
            get
            {
                return _qty;
            }
            set
            {
                _qty = value;
                OnPropertyChanged("TotalCost");
            }
        }

        public double TotalCost { 
            get
            { 
                return Math.Round(Qty * CostRmb, 2);
            } 
        }

        public ProductQty(int id, string name, double costRmb, int qty)
            : base(id, name, costRmb)
        {
            Qty = qty;
        }
    }


    public class Part
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TypeId { get; set; }

        public Part(int id, string name, int typeId)
        {
            Id = id;
            Name = name;
            TypeId = typeId;
        }

        public string TypeName
        {
            get
            {
                return DatabaseHandler.GetPartTypeName(TypeId);
            }
        }
    }

    public class PartQty : Part
    {
        public int OrderQty { get; set; }
        public PartQty(int id, string name, int typeId, int orderQty)
            : base(id, name, typeId)
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
