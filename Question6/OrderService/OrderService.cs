using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Serialization;
using System.IO;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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


        public static List<Order> GetAllOrders()
        {
            using (var db = new OrderContext())
            {
                return AllOrders(db).ToList();
            }
        }

        public static Order GetOrder(string id)
        {
            using (var db = new OrderContext())
            {
                return AllOrders(db).FirstOrDefault(o => o.Id == id);
            }
        }

        public static Order AddOrder(Order order)
        {
            try
            {
                using (var db = new OrderContext())
                {
                    db.Orders.Add(order);
                    db.SaveChanges();
                }
                return order;
            }
            catch (Exception e)
            {
                //TODO 需要更加错误类型返回不同错误信息
                throw new ApplicationException($"添加错误: {e.Message}");
            }
        }

        public static void RemoveOrder(string id)
        {
            try
            {
                using (var db = new OrderContext())
                {
                    var order = db.Orders.Include("Items").Where(o => o.Id == id).FirstOrDefault();
                    db.Orders.Remove(order);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                //TODO 需要更加错误类型返回不同错误信息
                throw new ApplicationException($"删除订单错误!");
            }
        }

        public static void UpdateOrder(Order newOrder)
        {
            RemoveItems(newOrder.Id);
            using (var db = new OrderContext())
            {
                db.Entry(newOrder).State = EntityState.Modified;
                db.OrderDetails.AddRange(newOrder.Details);
                db.SaveChanges();
            }
        }
        private static void RemoveItems(string orderId)
        {
            using (var db = new OrderContext())
            {
                var oldItems = db.OrderDetails.Where(item => item.OrderId  == orderId);
                db.OrderDetails.RemoveRange(oldItems);
                db.SaveChanges();
            }
        }

       /* public static List<Order> QueryOrdersByGoodsName(string goodsName)
        {
            using (var db = new OrderContext())
            {
                var query = AllOrders(db)
                  .Where(o => o.Details.Count(i => i.GoodsItem.Name == goodsName) > 0);
                return query.ToList();
            }
        }
       */

       /* public static List<Order> QueryOrdersByCustomerName(string customerName)
        {
            using (var db = new OrderContext())
            {
                var query = AllOrders(db)
                  .Where(o => o.Customer.Name == customerName);
                return query.ToList();
            }
        }
       */

  /*  public static object QueryByTotalAmount(float amout)
        {
            using (var db = new OrderContext())
            {
                return AllOrders(db)
                  .Where(o => o.Details.Sum(item => item.GoodsItem.Price * item.Quantity) > amout)
                  .ToList();
            }
        }
        */


       private static IQueryable<Order> AllOrders(OrderContext db)
        {
            return db.Orders.Include(o => o.Details);
        }
        //添加
       /* public void addOrder(Order a)
        { 

            if (orders.Contains(a))
            {
                throw new ApplicationException($"the orderList contains an order with ID {a.Id} !");
            }
            orders.Add(a);
        }
       */
        //获得id准备删除
      /*  public int getId()
        {
            Console.WriteLine("输入要删除的ID");
            int id = Convert.ToInt32(Console.ReadLine());
            return id;
        }
      */
        //删除byid
      /*  public void deleteOrder(int id )
        {
            int index = 0;
            foreach (Order a in this.orders)
            {
                if (a.Id == id) index = this.orders.IndexOf(a);
            }
            this.orders.RemoveAt(index); Console.WriteLine("删除成功");
        }
      */
        //getOrderById
      /*  public Order GetOrder(int id)
        {
            return orders.Where(o => o.Id == id).FirstOrDefault();
        }
      */
        //AddByOrder
        /*public void AddOrder(Order order)
        {
            if (orders.Contains(order))
                throw new ApplicationException($"添加错误: 订单{order.Id} 已经存在了!");
            orders.Add(order);
        }
        */
        //RemoveById
       /* public void RemoveOrder(int orderId)
        {
            Order order = GetOrder(orderId);
            if (order != null)
            {
                orders.Remove(order);
            }
        }
       */
        //Update
       /* public void UpdateOrder(Order newOrder)
        {
            Order oldOrder = GetOrder(newOrder.Id);
            if (oldOrder == null)
                throw new ApplicationException($"修改错误：订单 {newOrder.Id} 不存在!");
            orders.Remove(oldOrder);
            orders.Add(newOrder);
        }
       */

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
        public List<Order> Import(string path)
        {
            if (Path.GetExtension(path) != ".xml")
                throw new ArgumentException($"{path} isn't a xml file!");
            XmlSerializer xs = new XmlSerializer(typeof(List<Order>));
            List<Order> result = new List<Order>();
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    return (List<Order>)xs.Deserialize(fs);
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
