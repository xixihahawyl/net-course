using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Serialization;
using System.IO;

namespace app
{
    public class OrderService
    {
        private List<Order> orders;
        public OrderService()
        {
            orders = new List<Order>();
        }
        public List<Order> Orders
        {
            get => orders;
        }

        //查询by价格
        public void SearchOrderByPrice()
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


        //get一个order准备添加到orderlist
        public Order getOrder()
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
            a = new Order(id, customer, det);
            return a;
        }
        //添加
        public void addOrder(Order a)
        { 

            if (orders.Contains(a))
            {
                throw new ApplicationException($"the orderList contains an order with ID {a.Id} !");
            }
            orders.Add(a);
        }
        //获得id准备删除
        public int getId()
        {
            Console.WriteLine("输入要删除的ID");
            int id = Convert.ToInt32(Console.ReadLine());
            return id;
        }
        //删除byid
        public void deleteOrder(int id )
        {
            int index = 0;
            foreach (Order a in this.orders)
            {
                if (a.Id == id) index = this.orders.IndexOf(a);
            }
            this.orders.RemoveAt(index); Console.WriteLine("删除成功");
        }
        //getOrderById
        public Order GetOrder(int id)
        {
            return orders.Where(o => o.Id == id).FirstOrDefault();
        }
        //AddByOrder
        public void AddOrder(Order order)
        {
            if (orders.Contains(order))
                throw new ApplicationException($"添加错误: 订单{order.Id} 已经存在了!");
            orders.Add(order);
        }
        //RemoveById
        public void RemoveOrder(int orderId)
        {
            Order order = GetOrder(orderId);
            if (order != null)
            {
                orders.Remove(order);
            }
        }
        //Update
        public void UpdateOrder(Order newOrder)
        {
            Order oldOrder = GetOrder(newOrder.Id);
            if (oldOrder == null)
                throw new ApplicationException($"修改错误：订单 {newOrder.Id} 不存在!");
            orders.Remove(oldOrder);
            orders.Add(newOrder);
        }

        //序列化
        public void Export(String fileName)
        {
            if (Path.GetExtension(fileName) != ".xml")
                throw new ArgumentException("the exported file must be a xml file!");
            XmlSerializer xs = new XmlSerializer(typeof(List<Order>));
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                xs.Serialize(fs, this.orders);
            }
        }
        //反序列化
        public void Import(string path)
        {
            if (Path.GetExtension(path) != ".xml")
                throw new ArgumentException($"{path} isn't a xml file!");
            XmlSerializer xs = new XmlSerializer(typeof(List<Order>));
            List<Order> result = new List<Order>();
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    List<Order> temp = (List<Order>)xs.Deserialize(fs);
                    temp.ForEach(order => {
                        if (!orders.Contains(order))
                        {
                            orders.Add(order);
                        }
                    });
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException("import error:" + e.Message);
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
        //lambda表达式排序
        public void Sort(Comparison<Order> comparison)
        {
            orders.Sort(comparison);
        }

    }
}
