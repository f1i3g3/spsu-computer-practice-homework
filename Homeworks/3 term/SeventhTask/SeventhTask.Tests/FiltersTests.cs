using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using Server;
using Grpc.Net.Client;
using Google.Protobuf;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using Grpc.Core;

namespace SeventhTask.Tests
{
	[TestClass]
	public class FiltersTests
	{
		private static Process server;

		[ClassInitialize]
		public static void ServerInit(TestContext _)
		{
			server = new Process();
			server.StartInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\Server\bin\Debug\net5.0\Server.exe");
			server.Start();
		}

		[ClassCleanup]
		public static void ServerClean() => server.Kill();

		[TestMethod]
		public void Gauss3TestMethod()
		{
			Assert.IsTrue(CheckResults(Images.Gauss3, "Gauss3x3"));
		}

		[TestMethod]
		public void Gauss5TestMethod()
		{
			Assert.IsTrue(CheckResults(Images.Gauss5, "Gauss5x5"));
		}

		[TestMethod]
		public void GreyTestMethod()
		{
			Assert.IsTrue(CheckResults(Images.Grey, "Grey"));
		}

		[TestMethod]
		public void MedianTestMethod()
		{
			Assert.IsTrue(CheckResults(Images.Median, "Median"));
		}

		[TestMethod]
		public void SobelXTestMethod()
		{
			Assert.IsTrue(CheckResults(Images.SobelX, "SobelX"));
		}

		[TestMethod]
		public void SobelYTestMethod()
		{
			Assert.IsTrue(CheckResults(Images.SobelY, "SobelY"));
		}

		private static Task<Bitmap> ApplyFilter(string filterName)
		{
			GrpcChannel channel = null;
			try
			{
				Bitmap testingImage = Images.Original;

				channel = GrpcChannel.ForAddress("https://localhost:5001", new GrpcChannelOptions
				{
					MaxReceiveMessageSize = null,
					MaxSendMessageSize = null
				});
				var client = new FiltersApp.FiltersAppClient(channel);
				var token = new CancellationToken();

				using var ms = new MemoryStream();
				testingImage.Save(ms, ImageFormat.Bmp);

				using var call = client.ApplyFilter(new FilterRequest
				{
					FilterName = filterName,
					ImageBytes = ByteString.CopyFrom(ms.ToArray())
				}, cancellationToken: token);

				Bitmap resultedImage = null;
				var task = Task.Run(async () =>
				{
					await foreach (var response in call.ResponseStream.ReadAllAsync(token))
					{
						switch (response.ReplyCase)
						{
							case FilterReply.ReplyOneofCase.CurrentProgress:
								break;
							case FilterReply.ReplyOneofCase.Image:
								using (var ms = new MemoryStream(response.Image.ImageBytes.ToByteArray()))
								{
									resultedImage = new Bitmap(ms);
								}

								break;
						}
					}
				});

				task.Wait();
				return Task.FromResult(resultedImage);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				return null;
			}
			finally
			{
				channel?.ShutdownAsync();
			}
		}
		//testingImage.Save(string.Concat(path, "original_", filterName.ToLower(), ".bmp"));

		private static bool CheckResults(Bitmap expectedImage, string filterName)
		{
			var actualImage = ApplyFilter(filterName).Result;
			Assert.IsNotNull(actualImage);

			bool toReturn = CompareTwoImages(expectedImage, actualImage);

			expectedImage.Dispose();
			actualImage.Dispose();

			return toReturn;
		}

		private static bool CompareTwoImages(Bitmap firstImage, Bitmap secondImage)
		{
			int height = firstImage.Height;
			int width = firstImage.Width;

			for (int h = 0; h < height; h++)
			{
				for (int w = 0; w < width; w++)
				{
					var firstColor = firstImage.GetPixel(w, h);
					var secondColor = secondImage.GetPixel(w, h);

					if (!firstColor.Equals(secondColor))
					{
						Debug.WriteLine($"Pixel mismatch: [{w}, {h}]!");
						return false;
					}
				}
			}

			Debug.WriteLine("Test completed!");
			return true;
		}
	}
}