namespace FiltersLib
{
	public class Median : Filter
	{
		public Median()
		{
			matrix = new double[,]
			{
				{ 1, 1, 1 },
				{ 1, 1, 1 },
				{ 1, 1, 1 }
			};
			divider = 9;
		}
	}
}