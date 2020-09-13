using System;
using System.Globalization;

namespace Q3
{
    class Program
    {
        static void Main(string[] args)
        {
            int n = 0;
            string s = "";
            s=Console.ReadLine();
            n = Int32.Parse(s);

            int[] arr = new int[n];
            bool[] isPrime = new bool[n+1];
            for (int i = 0; i <= n; i++)
            {
                isPrime[i] = true; ;
            }
            int num = 0;
            for (int i = 2; i <= n; i++)
            {
                if (isPrime[i])
                {
                    arr[num] = i;
                    num = num + 1;
                    for (int j = 2 * i; j <= n; j = j + i)
                    {
                        isPrime[j] = false;
                    }
                }
            }
            for (int i = 0; i < arr.Length; i++) 
            { 
                if(arr[i]!=0)
                     Console.WriteLine(arr[i]);
            }
        }
    }
}
