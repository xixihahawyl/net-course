using System;

namespace Question3
{
    class Program
    {
        static void Main(string[] args)
        {
            OrderService os = new OrderService();
            bool judge = true;
            while (judge)
            {
                Console.WriteLine("1添加Order||2删除Order||3查询||4排序||5序列化||6反序列化");
                int choice = Convert.ToInt32(Console.ReadLine());
                switch (choice)
                {
                    case 1:os.addOrder(); break;
                    case 2:os.deleteOrder(); break;
                    case 3:os.SearchOrder(); break;
                    case 4:os.orders.Sort();break;
                    case 5:os.export();break;
                    case 6:  Console.WriteLine(os.ToString());break;
                }
            }
            Console.WriteLine("Hello World!");
        }
    }
}
