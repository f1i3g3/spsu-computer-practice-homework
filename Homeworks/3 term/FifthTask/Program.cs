using System;
using System.Diagnostics;
using CascadeLib;

namespace FifthTask
{
	class Program
	{
		static void Main(string[] args)
		{
			int capacity = 200000;
			int maxValue = 100;
			var time = new Stopwatch();

			var arr = ArrayGenerator.Generate(capacity, maxValue);

			time.Start();
			var sequential = new Sequential();
			Console.WriteLine(sequential.ComputeLength(arr));
			time.Stop();
			var sResult = time.ElapsedMilliseconds;

			time.Restart();
			var cascadeSimple = new CascadeSimple();
			Console.WriteLine(cascadeSimple.ComputeLength(arr));
			time.Stop();
			var csResult = time.ElapsedMilliseconds;

			time.Restart();
			var cascadeMod = new CascadeMod();
			Console.WriteLine(cascadeMod.ComputeLength(arr));
			time.Stop();
			var cmResult = time.ElapsedMilliseconds;

			Console.WriteLine($"Time intervals: {sResult} ms, {csResult} ms, {cmResult} ms");
		}
	}
}