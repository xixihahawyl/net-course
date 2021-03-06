﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace app
{
    [Serializable]
    public class Order
    {
        private List<OrderDetails> details = new List<OrderDetails>();
        public int Id { get; set; }
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
        public Order(int Id,string Customer,List<OrderDetails> details)
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
            s = s + '\n';
            return s;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
