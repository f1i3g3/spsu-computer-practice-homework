using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeTask.TreeLib;

namespace TreeTask.Tests
{
	[TestClass]
	public class TreeWorkTests // Basic functionality tests
	{
		[TestMethod]
		public void ParallelizedIntTest()
		{
			var tree = new ParallelizedTree<int>();

			tree.Insert(15, 15);
			tree.Insert(10, 10);
			tree.Insert(21, 21);
			tree.Insert(20, 20);
			tree.Insert(25, 25);
			tree.Insert(40, 40);

			Assert.AreEqual(true, tree.Search(15));
			Assert.AreEqual(true, tree.Search(10));
			Assert.AreEqual(true, tree.Search(21));
			Assert.AreEqual(true, tree.Search(20));
			Assert.AreEqual(true, tree.Search(25));
			Assert.AreEqual(true, tree.Search(40));
			Assert.AreEqual(false, tree.Search(45));

			tree.Delete(25);
			tree.Delete(15);

			Assert.AreEqual(false, tree.Search(15));
			Assert.AreEqual(false, tree.Search(25));
		}

		[TestMethod]
		public void NonParallelizedIntTest()
		{
			var tree = new NonParallelizedTree<int>();

			tree.Insert(15, 15);
			tree.Insert(10, 10);
			tree.Insert(21, 21);
			tree.Insert(20, 20);
			tree.Insert(25, 25);
			tree.Insert(40, 40);

			Assert.AreEqual(true, tree.Search(15));
			Assert.AreEqual(true, tree.Search(10));
			Assert.AreEqual(true, tree.Search(21));
			Assert.AreEqual(true, tree.Search(20));
			Assert.AreEqual(true, tree.Search(25));
			Assert.AreEqual(true, tree.Search(40));
			Assert.AreEqual(false, tree.Search(45));

			tree.Delete(25);
			tree.Delete(15);

			Assert.AreEqual(false, tree.Search(15));
			Assert.AreEqual(false, tree.Search(25));
		}

		[TestMethod]
		public void ParallelizedStringTest()
		{
			var tree = new ParallelizedTree<string>();

			tree.Insert(2, "mother");
			tree.Insert(3, "father");
			tree.Insert(5, "their son");

			Assert.AreEqual(true, tree.Search(2));
			Assert.AreEqual(true, tree.Search(5));
			Assert.AreEqual(false, tree.Search(4));

			tree.Delete(2);
			tree.Delete(1);

			Assert.AreEqual(false, tree.Search(2));
			Assert.AreEqual(true, tree.Search(3));
		}

		[TestMethod]
		public void NonParallelizedStringTest()
		{
			var tree = new NonParallelizedTree<string>();

			tree.Insert(2, "mother");
			tree.Insert(3, "father");
			tree.Insert(5, "their son");

			Assert.AreEqual(true, tree.Search(2));
			Assert.AreEqual(true, tree.Search(5));
			Assert.AreEqual(false, tree.Search(4));

			tree.Delete(2);
			tree.Delete(1);

			Assert.AreEqual(false, tree.Search(2));
			Assert.AreEqual(true, tree.Search(3));
		}
	}
}