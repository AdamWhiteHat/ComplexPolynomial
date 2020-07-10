using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;

namespace ExtendedArithmetic
{
	public interface IComplexPolynomial : ICloneable<IComplexPolynomial>, IComparable<IComplexPolynomial>,
		IEquatable<IComplexPolynomial>, IEqualityComparer<IComplexPolynomial>, IFormattable
	{
		int Degree { get; }
		IComplexTerm[] Terms { get; }

		Complex this[int degree]
		{
			get;
			set;
		}

		void RemoveZeros();
		Complex Evaluate(Complex indeterminateValue);
	}
}
