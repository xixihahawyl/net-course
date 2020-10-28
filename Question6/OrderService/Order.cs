using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app
{
    [Serializable]
    public class Order
    {
        private List<OrderDetails> details = new List<OrderDetails>();
        [Key]
        public string Id { get; set; }
        public string Customer { get; set; }

        public double TotalPrice {
            get {
                double total = 0;
                foreach (OrderDetails detail in details) total += detail.totalPrice;
                return total;
                }
         }
        public List<OrderDetails> Details
        {
            get { return details; }
        }

        public void AddItem(OrderDetails orderItem)
        {
            Details.Add(orderItem);
        }

        public Order()
        { }
        public Order(string Id,string Customer,List<OrderDetails> details)
        {
            this.Id = Id;
            this.Customer = Customer;
            foreach (OrderDetails det in details)
                this.details.Add(det);
        }

        public void RemoveItem(OrderDetails orderItem)
        {
            Details.Remove(orderItem);
        }


        public override bool Equals(object obj)
        {
            var order = obj as Order;
            return order != null &&
                   Id == order.Id;
        }



        public override string ToString()
        {
            string s = "id:" + Id + "顾客名字:" + Customer + "总价:" + TotalPrice;
            foreach (OrderDetails det in details)
                s += det.ToString();
            s = s + '\n';
            return s;
        }

        public override int GetHashCode()
        {
            return 2108858624 + EqualityComparer<string>.Default.GetHashCode(Id);
        }
    }
}
