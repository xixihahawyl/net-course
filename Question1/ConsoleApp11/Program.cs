using System;
using System.Runtime.CompilerServices;

namespace Q2
{
    class Program
    {
        //输入
        static void Main(string[] args)
        {
            int[] a = { 5,8,7,16,3};

            int max = a[0];
            int min = a[0];
            double sum = 0;
            double ave=0;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] > max)
                {
                    max = a[i];
                }
                if (a[i] < min)
                {
                    min = a[i];
                }
                sum = sum + a[i];
                ave = sum / a.Length;
            }


            Console.WriteLine(max);
            Console.WriteLine(min);
            Console.WriteLine(sum);
            Console.WriteLine(ave);
        }
    }
}
