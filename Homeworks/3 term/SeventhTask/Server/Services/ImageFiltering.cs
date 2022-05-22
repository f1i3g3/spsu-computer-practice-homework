using FiltersLib;
using Grpc.Core;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
	public class ImageFiltering
	{
		private readonly Bitmap image;
		private readonly IServerStreamWriter<FilterReply> responseStream;
		private readonly CancellationToken token;

		public void Apply(string filterName)
		{
			try
			{
				// Setting
				BitmapData srcData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, image.PixelFormat);

				int stride = srcData.Stride;
				int perPixel = Image.GetPixelFormatSize(image.PixelFormat) / 8;

				int height = srcData.Height;
				int width = srcData.Width * perPixel;

				int countOfBytes = stride * height;
				var oldPixels = new byte[countOfBytes];
				var newPixels = new byte[countOfBytes];

				// Blocking memory & applying filter
				try
				{
					var pnt = srcData.Scan0;
					Marshal.Copy(pnt, oldPixels, 0, countOfBytes);

					var applyingFilter = SelectFilter(filterName);

					ParallelLoopResult result = Parallel.For(0, height, new ParallelOptions { CancellationToken = token }, (h, state) =>
					{
						int progress;
						if (token.IsCancellationRequested)
						{
							state.Break();
						}

						for (int w = 0; w < width; w++)
						{
							applyingFilter.Convolution(w, h, width, height, stride, perPixel, oldPixels, newPixels);
						}

						progress = 100 * h / height;

						lock (responseStream)
						{
							responseStream.WriteAsync(new FilterReply
							{
								CurrentProgress = new CurrentProgress
								{
									Progress = progress
								}
							});
						}
					});
				}
				// Finishing
				finally
				{
					Marshal.Copy(newPixels, 0, srcData.Scan0, newPixels.Length);
					image.UnlockBits(srcData);
				}
			}
			catch (OperationCanceledException)
			{
				throw new OperationCanceledException();
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		private static Filter SelectFilter(string filterName)
		{
			return filterName switch
			{
				"SobelX" => new Sobel(0),
				"SobelY" => new Sobel(1),
				// check math
				"Grey" => new Grey(),
				// check math
				"Median" => new Median(),
				// check math
				"Gauss3x3" => new Gauss(3),
				// check math
				"Gauss5x5" => new Gauss(5),
				_ => throw new Exception("Filter is not choosed!"),
			};
		}

		public ImageFiltering(Bitmap image, IServerStreamWriter<FilterReply> responseStream, CancellationToken token)
		{
			this.image = image;
			this.responseStream = responseStream;
			this.token = token;
		}
	}
}