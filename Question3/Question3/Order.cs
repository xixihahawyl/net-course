using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Question3
{
    class Order
    {
        public int Id { get; set; }
        public string Customer { get; set; }

        public double TotalPrice {
            get {
                double total = 0;
                foreach (OrderDetails detail in details) total += detail.totalPrice;
                return total;
                }
         }
        public List<OrderDetails> details = new List<OrderDetails>();
        public Order()
        {
            this.Id = 0;
            this.Customer = string.Empty;
        }

        public Order(int Id,string Customer,List<OrderDetails> details)
        {
            this.Id = Id;
            this.Customer = Customer;
            foreach (OrderDetails det in details)
                this.details.Add(det);
        }

        public override bool Equals(object obj)
        {
            Order a = obj as Order; 
            if (this.Id == a.Id && this.Customer == a.Customer)
                return true;
            return false;
        }



        public override string ToString()
        {
            string s = "id:" + Id + "顾客名字:" + Customer + "总价:" + TotalPrice;
            foreach (OrderDetails det in details)
                s += det.ToString();
            return s;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
