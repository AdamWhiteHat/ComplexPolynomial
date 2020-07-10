using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;

namespace ExtendedArithmetic
{
	public class ComplexTerm : IComplexTerm
	{
		public static ComplexTerm Zero = new ComplexTerm(Complex.Zero, 0);

		public int Exponent { get; private set; }
		public Complex CoEfficient { get; set; }
		private static string IndeterminateSymbol = "X";

		public ComplexTerm(Complex coefficient, int exponent)
		{
			Exponent = exponent;
			CoEfficient = coefficient;
		}

		public static IComplexTerm[] GetTerms(Complex[] terms)
		{
			List<IComplexTerm> results = new List<IComplexTerm>();

			int degree = 0;
			foreach (Complex term in terms)
			{
				results.Add(new ComplexTerm(term, degree));

				degree += 1;
			}

			return results.ToArray();
		}

		public Complex Evaluate(Complex indeterminate)
		{
			return Complex.Multiply(CoEfficient, Complex.Pow(indeterminate, Exponent));
		}

		public IComplexTerm Clone()
		{
			return new ComplexTerm(this.CoEfficient, this.Exponent);
		}

		public override string ToString()
		{
			return $"{CoEfficient.FormatString()}*{IndeterminateSymbol}^{Exponent}";
		}
	}
}
