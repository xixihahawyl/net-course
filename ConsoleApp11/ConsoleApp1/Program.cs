using System;

namespace ConsoleApp11
{
    class PrimeNumber
    {
        static void Main(string[] args)
        {
            int a = 0;
            string s = "";
            s = Console.ReadLine();
            a = Int32.Parse(s);

            for (int i = 1; i <= a; i++)
            {
                int b = a % i;
                if (b == 0)
                {
                    Console.WriteLine(i);
                }
            }
            s = Console.ReadLine();
        }
    }
}