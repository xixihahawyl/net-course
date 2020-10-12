using System;
using System.Collections.Generic;

namespace app
{
    class Program
    {
        static void Main(string[] args)
        {
            OrderService os = new OrderService();
            bool judge = true;
            while (judge)
            {
                Console.WriteLine("1添加Order||2删除Order||3查询||4排序||5序列化||6反序列化||7输出");
                int choice = Convert.ToInt32(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        Order a = new Order();
                        a = os.getOrder();
                        os.addOrder(a); break;
                    case 2:
                        int id = os.getId();
                        os.deleteOrder(id); break;
                    case 3:os.SearchOrderByPrice(); break;
                    case 4:os.Sort((o1,o2)=>o2.TotalPrice.CompareTo(o1.TotalPrice));break;
                    case 5:os.Export(@".\order.xml");break;
                    case 7:  Console.WriteLine(os.ToString());break;
                    case 6:
                        List<Order> importedOrders = os.Import(@".\order.xml");
                        importedOrders.ForEach(o => Console.WriteLine(o));
                        break;
                }
            }
            Console.WriteLine("Hello World!");
        }
    }
}
