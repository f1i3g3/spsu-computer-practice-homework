using System.Drawing;

namespace FiltersLib
{
	public class Filter
	{
		protected double[,] matrix;
		protected int divider;

		public virtual void Convolution(int w, int h, int width, int height, int stride, int perPixel, byte[] oldPixels, byte[] newPixels)  // KernelFilterMethod
		{
			double r = 0d, g = 0d, b = 0d;
			byte alpha = default;

			int size = matrix.GetLength(0);

			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					var oldIndex = Index(w + i - 1, h + j - 1, height, width, stride, perPixel);

					Color pixel;
					if (oldIndex == -1)
					{
						pixel = Color.Silver; // DefaultColor
					}
					else
					{
						if (perPixel == 4)
						{
							alpha = oldPixels[oldIndex + 3];
							pixel = Color.FromArgb(oldPixels[oldIndex + 3], oldPixels[oldIndex + 2], oldPixels[oldIndex + 1], oldPixels[oldIndex]);

						}
						else // perPixel == 3
						{
							pixel = Color.FromArgb(oldPixels[oldIndex + 2], oldPixels[oldIndex + 1], oldPixels[oldIndex]);
						}
					}

					r += matrix[j, i] * pixel.R / divider;
					g += matrix[j, i] * pixel.G / divider;
					b += matrix[j, i] * pixel.B / divider;
				}
			}

			var newIndex = Index(w, h, height, width, stride, perPixel);
			if (newIndex >= 0)
			{
				newPixels[newIndex] = ToByte(b);
				newPixels[newIndex + 1] = ToByte(g);
				newPixels[newIndex + 2] = ToByte(r);

				if (perPixel == 4)
				{
					newPixels[newIndex + 3] = alpha;
				}
			}
		}

		protected static int Index(int x, int y, int height, int width, int stride, int bpp)
		{
			return (x * bpp < 0 || x * bpp >= width || y < 0 || y >= height) ? -1 : x * bpp + y * stride;
		}

		protected static byte ToByte(double value)
		{
			return value switch
			{
				> 255 => 255,
				< 0 => 0,
				_ => (byte)value,
			};
		}

		
	}
}