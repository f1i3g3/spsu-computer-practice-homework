using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CascadeLib
{
	public class Sequential : IVectorLengthComputer
	{
		public int ComputeLength(int[] a)
		{
			int sum = default;

			for (int i = 0; i < a.Length; i++)
			{
				sum += Func.Square(a[i]);
			}

			return (int)Math.Sqrt(sum);
		}
	}
}