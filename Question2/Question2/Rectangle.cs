using System;
using System.Collections.Generic;
using System.Text;

namespace Question2
{
    class Rectangle:Shape
    {
        double length;
        double width;
        public Rectangle(double length,double width)
        {
            this.length = length;
            this.width = width;
            //Initialize();
        }
        public override double Area()
        {
            return length * width;
        }
        public override bool isOK()
        {
            if (length > 0 && width > 0)
            {
                Console.WriteLine("合理");
                return true;
            }
            Console.WriteLine("不合理");
            return false;
        }
        public override void Initialize()
        {
            string s = "";
            Console.WriteLine("输入长");
            s = Console.ReadLine();
            length = double.Parse(s);
            Console.WriteLine("输入宽");
            s = Console.ReadLine();
            width = double.Parse(s);
        }

    }
}
