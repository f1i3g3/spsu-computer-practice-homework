using System;

namespace ThirdTask
{
	class Program
	{
		private const int producersNum = 2;
		private const int consumersNum = 2;

		static void Main()
		{
			Console.WriteLine("The program has started!");

			var manager = new Manager(producersNum, consumersNum);

			manager.Start(0);
			manager.Start(1);
			
			Console.ReadKey();

			manager.Finish(0);
			manager.Finish(1);

			Console.WriteLine("The program has finished!");
			//Console.ReadKey();
		}

	}
}