using System;
using System.Collections.Generic;
using System.Text;

namespace Question2
{
    class Triangle:Shape
    {
        double side1;
        double side2;
        double side3;

        public Triangle(double side1,double side2,double side3)
        {
            this.side1 = side1;
            this.side2 = side2;
            this.side3 = side3;
            //Initialize();

        }
        public override double Area()
        {
            if (isOK())
            {
                double p = (side1 + side2 + side3) / 2;
                return Math.Sqrt(p * (p - side1) * (p - side2) * (p - side3));
            }
            return 1;
        }
        public override bool isOK()
        {
            if ((side1 + side2 > side3) && (side1 + side3 > side2) && (side2 + side3 > side1)&&side1>0&&side2>0&&side3>0)
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
            Console.WriteLine("输入第一条边");
            s = Console.ReadLine();
            side1 = double.Parse(s);
            Console.WriteLine("输入第二条边");
            s = Console.ReadLine();
            side2 = double.Parse(s);
            Console.WriteLine("输入第三条边");
            s = Console.ReadLine();
            side3 = double.Parse(s);
        }
    }
}
