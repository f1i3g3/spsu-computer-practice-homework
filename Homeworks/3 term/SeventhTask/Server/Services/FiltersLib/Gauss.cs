namespace FiltersLib
{
	public class Gauss : Filter
	{
		public Gauss(int size)
		{
			if (size == 3)
			{
				matrix = new double[,]
				{
					{ 1, 2, 1 },
					{ 2, 4, 2 },
					{ 1, 2, 1 }
				};
				divider = 16;
			}
			else // size == 5
			{
				matrix = new double[,]
				{
					{ 1, 4, 6, 4, 1 },
					{ 4, 16, 24, 16, 4 },
					{ 6, 24, 36, 24, 6 },
					{ 4, 16, 24, 16, 4 },
					{ 1, 4, 6, 4, 1 }
				};
				divider = 256;
			}
		}
	}
}