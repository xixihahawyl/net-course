using System;
using System.Collections.Generic;
using System.Text;

namespace Question2
{
    class Square:Shape
    {
        double side;
        public Square(double side)
        {
            this.side = side;
            //Initialize();
        }
        public override double Area()
        {
            return side * side;
        }
        public override bool isOK()
        {
            if (side > 0)
                return true;
            return false;
        }
        public override void Initialize()
        {
            string s = "";
            Console.WriteLine("输入长");
            s = Console.ReadLine();
            side = double.Parse(s);
        }
    }
}
