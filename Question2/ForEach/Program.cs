using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForEach
{
	class Program
	{
		static void Main(string[] args)
		{
			GenericList<int> list = new GenericList<int>();
			for (int i = 1; i <= 10; i++) list.Add(i);
			int max = list.Head.Data;
			int min = list.Head.Data;
			int sum = 0;
			list.ForEach((x) =>
			{
				Console.WriteLine(x);
				max = Math.Max(max, x);
				min = Math.Min(min, x);
				sum += x;
			});
			Console.WriteLine($"\nMax: {max}\nMin: {min}\nSum: {sum}");
		}
	}
}