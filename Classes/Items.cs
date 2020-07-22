using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Production_Planner
{

    public class Product : INotifyPropertyChanged
    {
        public int Id { get; set; }
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }
        public double CostRmb { get; set; }

        public Product() { }

        public Product(int id, string name, double costRmb)
        {
            Id = id;
            Name = name;
            CostRmb = costRmb;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ProductQty : Product, INotifyPropertyChanged
    {
        
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
                OnPropertyChanged("Qty");
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

        public ProductQty(Product prod, int qty)
        {
            Id = prod.Id;
            Name = prod.Name;
            CostRmb = prod.CostRmb;
            Qty = qty;
        }
    }


    public class Part : INotifyPropertyChanged
    {
        public int Id { get; set; }
        private string _name;
        public string Name {
        get
            {
                return _name;
            }
         set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }
        public int TypeId { get; set; }

        public Part(int id, string name, int typeId)
        {
            Id = id;
            Name = name;
            TypeId = typeId;
            PartType = new PartType(TypeId, DBHandler.GetPartTypeName(typeId));
        }

        public Part() { }

        public string TypeName
        {
            get
            {
                return DBHandler.GetPartTypeName(TypeId);
            }
        }

        private PartType _partType;
        public PartType PartType
        {
            get
            {
                return _partType;
            }
            set
            {
                _partType = value;
                OnPropertyChanged("PartType");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class PartQty : Part
    {
        private int _orderQty;
        public int OrderQty { 
            get
            {
                return _orderQty;
            }
            set
            {
                _orderQty = value;
                OnPropertyChanged("OrderQty");
            }
        }
        public PartQty(int id, string name, int typeId, int orderQty)
            : base(id, name, typeId)
        {
            this.OrderQty = orderQty;
        }

        public PartQty(Part part, int orderQty)
        {
            this.Id = part.Id;
            this.Name = part.Name;
            this.TypeId = part.TypeId;
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

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is PartType))
                return false;

            return ((PartType)obj).Id == this.Id;
        }
    }
}
