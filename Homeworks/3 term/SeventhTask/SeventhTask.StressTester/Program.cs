using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StressTester
{
	class Program
	{
		static void Main()
		{
			int sizeFlag = 1; // 0 - small, 1 - large
			Bitmap image = sizeFlag == 0 ? Images.Small : Images.Large;

			int reqsPerSecond = 50;
			int timeInSeconds = 3;
			int timeLimit = 60; // 1 minute

			Console.WriteLine("Started!");

			byte[] data = null;
			using (var ms = new MemoryStream())
			{
				image.Save(ms, ImageFormat.Bmp);
				data = ms.GetBuffer();
			}

			var requests = new List<Task<int>>();
			for (int i = 0; i < timeInSeconds; i++)
			{
				for (int j = 0; j < reqsPerSecond; j++)
				{
					var task = new Task<int>(new StressTesterUnit(data, timeLimit).OperationTime);

					task.Start();
					requests.Add(task);
				}
				Thread.Sleep(1000);
			}

			var results = new List<int>();
			foreach (var request in requests)
			{
				results.Add(request.Result);
			}

			if (results.Contains(-1))
			{
				Console.WriteLine($"Timeout exceeded! Denial of service under load of {requests} requests per second.");
			}
			else
			{
				var size = sizeFlag == 0 ? "s_" : "l_";
				var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Results\" + size + reqsPerSecond.ToString() + "rps_" + timeInSeconds.ToString() + "s.txt");

				try
				{
					using var writer = new StreamWriter(path);
					writer.WriteLine($"Image size: {image.Size.Width}x{image.Size.Height}; tested for {timeInSeconds} second(s), with gauss5x5 filter, {reqsPerSecond} request(s) per second and {timeLimit}-second time limit.");

					writer.WriteLine("\nResults (in ms):");
					foreach (var result in results)
					{
						writer.WriteLine(result);
					}

					results.Sort();

					int average = (int)results.Average();
					int median = results[results.Count / 2];

					writer.WriteLine($"\nAverage value: {average} ms.");
					writer.WriteLine($"Median value: {median} ms.");
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
					return;
				}
			}

			Console.WriteLine("Finished!");
		}
	}
}