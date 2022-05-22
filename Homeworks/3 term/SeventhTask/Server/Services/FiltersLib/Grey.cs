namespace FiltersLib
{
	public class Grey : Filter
	{
		public override void Convolution(int w, int h, int width, int height, int stride, int perPixel, byte[] oldPixels, byte[] newPixels)  // GreyFilterMethod
		{
			byte alpha = default;

			var index = Index(w, h, height, width, stride, perPixel);
			if (index >= 0)
			{
				var b = oldPixels[index];
				var g = oldPixels[index + 1];
				var r = oldPixels[index + 2];

				var value = ToByte((b + g + r) / divider);

				if (perPixel == 4)
				{
					newPixels[index + 3] = alpha;
				}

				newPixels[index] = value;
				newPixels[index + 1] = value;
				newPixels[index + 2] = value;
			}
		}

		public Grey()
		{
			matrix = null;
			divider = 3;
		}
	}
}