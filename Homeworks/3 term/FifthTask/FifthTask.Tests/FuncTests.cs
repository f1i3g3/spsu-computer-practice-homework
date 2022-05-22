using Microsoft.VisualStudio.TestTools.UnitTesting;
using CascadeLib;

namespace FifthTask.Tests
{
	[TestClass]
	public class FuncTests  //for current capacity
	{
		private int[] arr;
		private int capacity = 10;

		[TestInitialize]
		public void TestInit()
		{
			arr = new int[capacity];
			for (int i = 0; i < capacity; i++)
			{
				arr[i] = i;
			}
		}

		[TestMethod]
		public void SquareTest()
		{
			for (int i = 0; i < capacity; i++)
			{
				Assert.AreEqual(arr[i] * arr[i], Func.Square(arr[i]));
			}
		}

		[TestMethod]
		public void ArrayMathTest()
		{
			Assert.AreEqual(3, (int)Func.Log(capacity));
			Assert.AreEqual(4, Func.NumOfBlocks(capacity));
		}
	}
}