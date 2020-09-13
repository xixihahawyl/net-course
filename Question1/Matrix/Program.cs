using System;

namespace Q4
{
    class Program
    {
        static bool isMatrix(int[,] arr)
        {
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    if (i != 0 && j != 0 && arr[i, j] != arr[i - 1, j - 1])
                        return false;
                }
            }
            return true;
        }
        static void Main(string[] args)
        {
            int [,] arr =
            {
                { 1,2,3,4},
                { 5,1,2,3},
                { 6,5,1,2},
            };
           bool b = isMatrix(arr);
            if(b)
                Console.WriteLine("true");
        }
    }
}
