using System;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ExtendedArithmetic
{
	public static class ComplexExtensionMethods
	{
		public static Complex Clone(this Complex source)
		{
			return new Complex(source.Real, source.Imaginary);
		}

		public static Complex Norm(this Complex source)
		{
			return Complex.Multiply(source, Complex.Conjugate(source));
		}

		public static int Sign(this Complex source)
		{
			return (source.Real == 0) ? Math.Sign(source.Imaginary) : Math.Sign(source.Real);
		}

		public static Complex Mod(this Complex source, Complex other)
		{
			Complex negQuot = Complex.Negate(Complex.Divide(source, other));
			Complex ceil = new Complex(Math.Round(negQuot.Real), Math.Round(negQuot.Imaginary));
			return Complex.Add(source, Complex.Multiply(other, ceil));
		}

		public static Complex NthRoot(this Complex value, int root)
		{
			double one = 1.0;
			double rt = (double)root;
			double fraction = (one / rt);
			return Complex.Pow(value, fraction);
		}

		public static string FormatString(this Complex source)
		{
			string im = "";
			string sign = "";
			double real = source.Real;
			double imaginary = source.Imaginary;
			bool hasComplexPart = false;

			if (real < Math.Pow(10d, -5d))
			{
				real = Math.Round(real, 5);
			}
			if (imaginary < Math.Pow(10d, -5d))
			{
				imaginary = Math.Round(imaginary, 5);
			}

			if (Math.Sign(imaginary) == 1)
			{
				sign = " + ";
				im = $"{imaginary}{I}";
				hasComplexPart = true;
			}
			else if (Math.Sign(imaginary) == -1)
			{
				sign = " - ";
				im = $"{Math.Abs(imaginary)}{I}";
				hasComplexPart = true;
			}

			string result = $"{real}{sign}{im}";

			if (hasComplexPart)
			{
				result = $"({result})";
			}

			return result;
		}

		private static string I;
		private static string[] iChars;

		static ComplexExtensionMethods()
		{
			iChars = new string[] { "𝒊", "𝐢", "𝑖", "𝘪", "𝕚", "i" };
			I = iChars[1];
		}
	}

	public static class IDictionaryExtensionMethods
	{
		public static void RemoveRange<TKey, TValue>(this IDictionary<TKey, TValue> source, IEnumerable<TKey> collection)
		{
			if (collection.Any())
			{
				int index = collection.Count();
				while (index-- > 0)
				{
					source.Remove(collection.ElementAt(index));
				}
			}
		}
	}
}
