﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CascadeLib
{
	public class CascadeSimple : IVectorLengthComputer
	{
		public int ComputeLength(int[] a)
		{
			try
			{
				var tasks = new List<Task<int>>();

				if (a.Length % 2 != 0)
				{
					a.Append(default);
				}

				for (int i = 0; i < a.Length; i += 2)
				{
					var tempFirst = a[i];
					var tempSecond = a[i + 1];

					tasks.Add(Task.Factory.StartNew(() => Func.Square(tempFirst) + Func.Square(tempSecond)));
				}

				while (tasks.Count > 1)
				{
					for (int i = 0; i < tasks.Count - 1; i += 2)
					{
						tasks[i].Wait();
						var tempFirst = tasks[i].Result;

						var tempSecond = 0;
						if (i + 1 <= tasks.Count - 1)
						{
							tasks[i + 1].Wait();
							tempSecond = tasks[i + 1].Result;

							tasks[i + 1].Dispose();
							tasks.RemoveAt(i + 1);
						}

						tasks[i].Dispose();
						tasks[i] = Task.Factory.StartNew(() => tempFirst + tempSecond);
					}
				}

				tasks[0].Wait();
				int sum = (int)Math.Sqrt(tasks[0].Result);

				tasks[0].Dispose();
				tasks.Clear();

				return sum;

			}
			catch (Exception ex)
			{
				Console.WriteLine("Something went wrong!\n" + ex.ToString());
				return -1;
			}
		}
	}
}