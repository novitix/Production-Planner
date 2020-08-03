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
        private double _costRmb;
        public double CostRmb {
        get
            {
                return _costRmb;
            }
        set
            {
                _costRmb = value;
                OnPropertyChanged("CostRmb");
            }
        }

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

        public Part(int id, string name, PartType partType)
        {
            Id = id;
            Name = name;
            this.PartType = partType;
            PartType = partType;
        }

        public Part() { }

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
        private double _orderQty;
        public double OrderQty { 
            get
            {
                return _orderQty;
            }
            set
            {
                if (_orderQty != value)
                {
                    _orderQty = value;
                    OnPropertyChanged("OrderQty");
                }

            }
        }
        public PartQty(int id, string name, PartType partType, double orderQty)
            : base(id, name, partType)
        {
            this.OrderQty = orderQty;
        }

        public PartQty(Part part, double orderQty)
        {
            this.Id = part.Id;
            this.Name = part.Name;
            this.PartType = part.PartType;
            this.OrderQty = orderQty;
        }
    }

    public class PartType : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void InvokeIdChanged()
        {
            OnPropertyChanged("Id");
        }

        private int _id;
        public int Id {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                OnPropertyChanged("Id");
                TypeName = DBHandler.GetPartTypeName(_id);
            }
        }

        private string _typeName;
        public string TypeName {
        get
            {
                return _typeName;
            }
        set
            {
                _typeName = value;
                OnPropertyChanged("TypeName");
            }
        }

        public PartType(int id, string typeName)
        {
            Id = id;
            TypeName = typeName;
        }

        public PartType(int id)
        {
            Id = id;
            TypeName = DBHandler.GetPartTypeName(id);
        }

    }
}
