using System;
using System.Collections.Generic;
using System.Text;

namespace app
{
    public class OrderDetails
    {
        public double price { get; set; }
        public int count { get; set;}
        public string name { get; set; }
        public double totalPrice { get => price * count; }

        public OrderDetails(double price,int count,string name) {
            this.price = price;
            this.count = count;
            this.name = name;
        }
        public OrderDetails() { }
        public override string ToString()
        {
            string s = "商品价格:" + price + "商品数量:" + count + "商品名字:" + name;
            return s;
        }
    }
}
