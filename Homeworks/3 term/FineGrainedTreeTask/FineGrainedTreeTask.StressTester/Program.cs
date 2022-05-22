using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TreeTask.TreeLib;

namespace TreeTask.Tester
{
	class Program
	{
		private static ITree<int> tree;
		private static List<Task> requests;
		private static readonly int tasksNum = 1000; // Num of tasks

		static void Main() // More informative stress testing (although without recording in file)
		{
			Console.WriteLine("Started!");

			var stopwatch = new Stopwatch();
			requests = new List<Task>();

			TasksInit();
			tree = new NonParallelizedTree<int>();
			stopwatch.Start();
			StartTest();
			stopwatch.Stop();
			Console.WriteLine($"Non-parallelized tree running time after {tasksNum} requests: {stopwatch.ElapsedMilliseconds} ms");
			requests.Clear();

			TasksInit();
			tree = new ParallelizedTree<int>();
			stopwatch.Restart();
			StartTest();
			stopwatch.Stop();
			Console.WriteLine($"Parallelized tree running time after {tasksNum} requests: {stopwatch.ElapsedMilliseconds} ms");
			requests.Clear();

			Console.WriteLine("Finished!");
			// There's an improvement in performance when debugging
		}

		public static void TasksInit()
		{
			var rnd = new Random();
			for (int i = 0; i < tasksNum; i++)
			{
				int chs = rnd.Next(100);

				if (chs < 50) // 50% searching request chance
				{
					requests.Add(new Task(() => tree.Search(rnd.Next())));
				}
				else if (chs < 75) // 25% adding request chance
				{
					requests.Add(new Task(() => tree.Insert(rnd.Next(), rnd.Next())));

				}
				else // 25% deleting request chance
				{
					requests.Add(new Task(() => tree.Delete(rnd.Next())));

				}
			}
		}

		public static void StartTest() // Starting stress test
		{
			foreach (var task in requests)
			{
				task.Start();
			}
			foreach (var task in requests)
			{
				task.Wait();
			}
		}
	}
}