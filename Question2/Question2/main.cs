using System;
using System.Collections.Generic;
using System.Text;

namespace Question2
{
    class main
    {
        static void Main(string[] args)
        {
            double sumarea = 0;
            Shape[] arr = Factory.CreateShape(10);
            for (int i = 0; i < 10; i++)
            {
                sumarea+=arr[i].Area();
            }
            Console.WriteLine(sumarea);



        }
    }
}
