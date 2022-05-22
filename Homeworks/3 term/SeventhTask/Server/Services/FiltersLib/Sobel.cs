namespace FiltersLib
{
	public class Sobel : Filter
	{
		public Sobel(int param)
		{
			if (param == 0) // SobelX
			{
				matrix = new double[,]
				{
					{ 1, 2, 1 },
					{ 0, 0, 0 },
					{ -1, -2, -1 }
				};
			}
			else // SobelY
			{
				matrix = new double[,]
				{
					{ -1, 0, 1 },
					{ -2, 0, 2 },
					{ -1, 0, 1 }
				};
			}
			divider = 1;
		}
	}
}