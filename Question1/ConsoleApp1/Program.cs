using System;

namespace Q1
{
    class PrimeNumber
    {
        static void Main(string[] args)
        {
            int a = 0;
            string s = "";
            s = Console.ReadLine();
            a = Int32.Parse(s);

            for (int i = 2; i <= a; i++)
            {
                while (true)
                {
                    int b = a % i;
                    if (b == 0)
                    {
                        Console.WriteLine(i);
                        a = a / i;
                    }
                    else break;
                }
            }
            s = Console.ReadLine();
        }
    }
}