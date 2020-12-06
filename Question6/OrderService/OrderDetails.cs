using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app
{
    public class OrderDetails
    {
        [Key]
        public string Id { get; set; }
        public double price { get; set; }
        public int count { get; set;}
        public string name { get; set; }
        public string OrderId { get; set; }
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

        public override bool Equals(object obj)
        {
            var item = obj as OrderDetails;
            return item != null &&
                   Id == item.Id;
        }

        public override int GetHashCode()
        {
            return 2108858624 + EqualityComparer<string>.Default.GetHashCode(Id);
        }
    }
}
