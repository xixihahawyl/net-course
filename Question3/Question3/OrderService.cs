using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Serialization;
using System.IO;

namespace Question3
{
    class OrderService
    {
        public List<Order> orders = new List<Order>();
        public void SearchOrder()
        {
            //订单金额查询
            double minNum, maxNum;
            Console.WriteLine("输入要查询的最小金额：");
            minNum = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("输入要查询的最大金额：");
            maxNum = Convert.ToInt32(Console.ReadLine());

            var q1 = from s1 in orders
                     where maxNum > s1.TotalPrice
                     orderby s1.TotalPrice
                     select s1;
            var q3 = from s3 in q1
                         where s3.TotalPrice > minNum
                         orderby s3.TotalPrice
                         select s3;
            OrderService a = new OrderService();
            a.orders = q3.ToList();
            Console.WriteLine(a.ToString());
        }

        public void addOrder()
        {
            Console.WriteLine("ID");
            int id = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("客户名");
            string customer = Console.ReadLine();
            Order a = new Order();
            List<OrderDetails> det = new List<OrderDetails>();
            //detail输入
            bool flag = true;
            while (flag)
            {
                Console.WriteLine("商品名字");
                string name = Console.ReadLine();
                Console.WriteLine("商品数量");
                int count = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("商品价格");
                double price = Convert.ToInt32(Console.ReadLine());
                OrderDetails detail = new OrderDetails(price, count, name);
                det.Add(detail);
                Console.WriteLine("0退出,1继续");
                int ab = Convert.ToInt32(Console.ReadLine());
                if (ab == 0) flag = false;
            }
            a = new Order(id,customer,det);

            bool same = false;
            foreach (Order m in orders)
            {
                if (m.Equals(a))
                    same = true;
            }
            if (same) Console.WriteLine("订单重复");
            else
            {
                orders.Add(a);
                Console.WriteLine("添加成功");
            }    
        }

        public void deleteOrder()
        {
            Console.WriteLine("输入要删除的ID");
            int id = Convert.ToInt32(Console.ReadLine());
            int index = 0;
            foreach (Order a in this.orders)
            {
                if (a.Id == id) index = this.orders.IndexOf(a);
            }
            this.orders.RemoveAt(index); Console.WriteLine("删除成功");
        }

        public void changeOrder()
        { 
            
        }

        public void export()
        {
            XmlSerializer a = new XmlSerializer(typeof(List<Order>));
            using (FileStream b = new FileStream("order.xml", FileMode.Create))
            {
                a.Serialize(b, this.orders);
            }
            Console.WriteLine(File.ReadAllText("order.xml"));
            Console.WriteLine("序列化完成");
        }

        public void import()
        {
            try
            {
                XmlSerializer a = new XmlSerializer(typeof(List<Order>));
                using (FileStream b = new FileStream("order.xml", FileMode.Open))
                {
                    List<Order> c = (List<Order>)a.Deserialize(b);
                    Console.WriteLine("反序列化结果：");
                    foreach (Order d in c)
                    {
                        Console.WriteLine(d);
                    }
                }
            }
            catch
            {
                Console.WriteLine("序列化系列操作错误");
            }
        }
        public override string ToString()
        {
            string s = "";
            foreach (Order o in orders)
            {
                s += o.ToString();
                Console.WriteLine();
            }
            return s;
        }

    }
}
