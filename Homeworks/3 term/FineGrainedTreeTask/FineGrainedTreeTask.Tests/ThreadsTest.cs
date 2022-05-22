using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using TreeTask.TreeLib;

namespace TreeTask.Tests
{
	[TestClass]
	public class ThreadsTest // Unit testing on many requests per second
	{
		private readonly int tasksSize = 2000;

		[TestMethod]
		public void ParallelizedTreeTest()
		{
			var tree = new ParallelizedTree<int>();
			var tasks = new List<Task>(tasksSize);

			for (int i = 0; i < tasks.Count; i++)
			{
				tasks[i] = new Task(() => RequestTo(tree));
			}
			for (int i = 0; i < tasks.Count; i++)
			{
				tasks[i].Start();
			}
			for (int i = 0; i < tasks.Count; i++)
			{
				tasks[i].Wait();
				tasks[i].Dispose();
			}
		}

		[TestMethod]
		public void NonParallelizedTreeTest()
		{
			var tree = new NonParallelizedTree<int>();
			var tasks = new List<Task>(tasksSize);

			for (int i = 0; i < tasks.Count; i++)
			{
				tasks[i] = new Task(() => RequestTo(tree));
			}
			for (int i = 0; i < tasks.Count; i++)
			{
				tasks[i].Start();
			}
			for (int i = 0; i < tasks.Count; i++)
			{
				tasks[i].Wait();
				tasks[i].Dispose();
			}
		}

		private static void RequestTo(ITree<int> tree)
		{
			for (int i = 0; i < 120; i++)
			{
				int value = i;

				tree.Insert(value, value);
				Assert.IsTrue(tree.Search(i));

				tree.Delete(value);
				Assert.IsFalse(tree.Search(i));
			}
		}
	}
}