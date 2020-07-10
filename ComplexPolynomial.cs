using System;
using System.Text;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ExtendedArithmetic
{
	public class ComplexPolynomial : IComplexPolynomial
	{
		public static IComplexPolynomial Zero = new ComplexPolynomial(ComplexTerm.GetTerms(new Complex[] { 0 }));
		public static IComplexPolynomial One = new ComplexPolynomial(ComplexTerm.GetTerms(new Complex[] { 1 }));
		public static IComplexPolynomial Two = new ComplexPolynomial(ComplexTerm.GetTerms(new Complex[] { 2 }));

		public IComplexTerm[] Terms { get { return _terms.Values.ToArray(); } }
		private SortedList<int, IComplexTerm> _terms = new SortedList<int, IComplexTerm>();
		public int Degree { get; private set; }

		#region Indexer

		public Complex this[int degree]
		{
			get { return _terms.ContainsKey(degree) ? _terms[degree].CoEfficient : Complex.Zero; }
			set
			{
				if (_terms.ContainsKey(degree)) { _terms[degree].CoEfficient = value; }
				else if (value != Complex.Zero)
				{
					_terms.Add(degree, new ComplexTerm(value, degree));
					SetDegree();
				}
			}
		}

		#endregion

		private ComplexPolynomial() { _terms = new SortedList<int, IComplexTerm>(); }

		public ComplexPolynomial(IComplexTerm[] terms)
		{
			SetTerms(terms);
		}

		public ComplexPolynomial(Complex n, Complex polynomialBase, int forceDegree)
		{
			Degree = forceDegree;
			SetTerms(GetPolynomialTerms(n, polynomialBase, Degree));
		}

		private void SetTerms(IEnumerable<IComplexTerm> terms)
		{
			if (terms.Any())
			{
				foreach (IComplexTerm term in terms)
				{
					_terms.Add(term.Exponent, term);
				}
			}
			RemoveZeros();
		}

		public void RemoveZeros()
		{
			var toRemove = _terms.Where(kvp => kvp.Value.CoEfficient == 0).Select(kvp => kvp.Key);
			if (toRemove.Any())
			{
				_terms.RemoveRange(toRemove);
			}

			if (!_terms.Any())
			{
				_terms.Add(0, ComplexTerm.Zero);
			}
			SetDegree();
		}

		private void SetDegree()
		{
			Degree = (_terms.Keys.Max());
		}

		private static List<IComplexTerm> GetPolynomialTerms(Complex value, Complex polynomialBase, int degree)
		{
			int d = degree; // (int)Math.Truncate(Complex.Log(value, (double)polynomialBase)+ 1);
			Complex toAdd = value;
			List<IComplexTerm> result = new List<IComplexTerm>();
			while (d >= 0 && Complex.Abs(toAdd) > 0)
			{
				Complex placeValue = Complex.Pow(polynomialBase, d);

				if (placeValue == 1)
				{
					result.Add(new ComplexTerm(toAdd, d));
					toAdd = 0;
				}
				else if (placeValue == toAdd)
				{
					result.Add(new ComplexTerm(1, d));
					toAdd -= placeValue;
				}
				else if (Complex.Abs(placeValue) < Complex.Abs(toAdd))
				{
					Complex quotient = Complex.Divide(toAdd, placeValue);
					if (Complex.Abs(quotient) > Complex.Abs(placeValue))
					{
						quotient = placeValue;
					}

					result.Add(new ComplexTerm(quotient, d));
					Complex toSubtract = Complex.Multiply(quotient, placeValue);

					toAdd -= toSubtract;
				}

				d--;
			}
			return result.ToList();
		}

		public static IComplexPolynomial FromRoots(params Complex[] roots)
		{
			return ComplexPolynomial.Product(
				roots.Select(
					zero => new ComplexPolynomial(
						new ComplexTerm[]
						{
						new ComplexTerm( 1, 1),
						new ComplexTerm( Complex.Negate(zero), 0)
						}
					)
				)
			);
		}

		public Complex Evaluate(Complex indeterminateValue)
		{
			return Evaluate(Terms, indeterminateValue);
		}

		public static Complex Evaluate(IComplexTerm[] terms, Complex indeterminateValue)
		{
			Complex result = Complex.Zero;
			foreach (IComplexTerm term in terms)
			{
				Complex placeValue = Complex.Pow(indeterminateValue, term.Exponent);
				Complex addValue = Complex.Multiply(term.CoEfficient, placeValue);
				result = Complex.Add(result, addValue);
			}
			return result;
		}

		public double EvaluateDouble(double indeterminateValue)
		{
			return EvaluateDouble(Terms, indeterminateValue);
		}

		public static double EvaluateDouble(IComplexTerm[] terms, double x)
		{
			double result = 0;

			int d = terms.Count() - 1;
			while (d >= 0)
			{
				double placeValue = Math.Pow(x, terms[d].Exponent);

				double addValue = (double)(Complex.Abs(terms[d].CoEfficient)) * placeValue;

				result += addValue;

				d--;
			}

			return result;
		}

		public static IComplexPolynomial GetDerivativePolynomial(IComplexPolynomial poly)
		{
			int d = 0;
			List<IComplexTerm> terms = new List<IComplexTerm>();
			foreach (IComplexTerm term in poly.Terms)
			{
				d = term.Exponent - 1;
				if (d < 0)
				{
					continue;
				}
				terms.Add(new ComplexTerm(term.CoEfficient * term.Exponent, d));
			}

			IComplexPolynomial result = new ComplexPolynomial(terms.ToArray());
			return result;
		}

		#region IsIrreducible

		/*
        public static bool IsIrreducibleOverField(IPoly f, Complex m, Complex p)
        {
            IPoly splittingField = new ComplexPoly(
                    new PolyTerm[] {
                        new PolyTerm(  1, (int)Complex.Abs(p)),
                        new PolyTerm( -1, 1)
                    });

            IPoly reducedField = ComplexPoly.ModMod(splittingField, f, p);

            if (!EisensteinsIrreducibilityCriterion(reducedField, p))
            {
                return false;
            }

            Complex fieldValue = reducedField.Evaluate(m);

            IPoly gcd = ComplexPoly.GCDMod(f, reducedField, m);
            return (gcd.CompareTo(ComplexPoly.One) == 0);
        }

        public static bool IsIrreducibleOverP(IPoly poly, Complex p)
        {
            List<Complex> coefficients = poly.Terms.Select(t => t.CoEfficient).ToList();

            Complex leadingCoefficient = coefficients.Last();
            Complex constantCoefficient = coefficients.First();

            coefficients.Remove(leadingCoefficient);
            coefficients.Remove(constantCoefficient);

            Complex leadingRemainder = -1;
            Complex.DivRem(leadingCoefficient, p, out leadingRemainder);

            Complex constantRemainder = -1;
            Complex.DivRem(constantCoefficient, p.Square(), out constantRemainder);

            bool result = (leadingRemainder != 0); // p does not divide leading coefficient

            result &= (constantRemainder != 0);    // p^2 does not divide constant coefficient

            coefficients.Add(p);
            result &= (Maths.GCD(coefficients) == 1);

            return result;
        }
        */

		#endregion

		public static void Swap(ref IComplexPolynomial a, ref IComplexPolynomial b)
		{
			if (b.CompareTo(a) > 0)
			{
				IComplexPolynomial swap = b;
				b = a;
				a = swap;
			}
		}

		public static IComplexPolynomial GCDMod(IComplexPolynomial left, IComplexPolynomial right, Complex polynomialBase)
		{
			IComplexPolynomial a = left.Clone();
			IComplexPolynomial b = right.Clone();


			Swap(ref a, ref b);

			while (a.Degree != b.Degree)
			{
				IComplexPolynomial smallerA = ComplexPolynomial.ReduceDegree(a, polynomialBase);
				a = smallerA;

				Swap(ref a, ref b);
			}

			while (a.Degree != 1)
			{
				IComplexPolynomial smallerA = ComplexPolynomial.ReduceDegree(a, polynomialBase);
				IComplexPolynomial smallerB = ComplexPolynomial.ReduceDegree(b, polynomialBase);

				a = smallerA;
				b = smallerB;

				Swap(ref a, ref b);
			}

			while (a.Degree >= 1)
			{
				Swap(ref a, ref b);

				var bSign = b.Terms.Last().CoEfficient.Sign();
				if (bSign < 0)
				{
					break;
				}

				while (!(b.Terms.Length == 0 || b.Terms[0].CoEfficient == 0 || a.CompareTo(b) < 0))
				{
					var aSign = a.Terms.Last().CoEfficient.Sign();
					bSign = b.Terms.Last().CoEfficient.Sign();

					if (aSign < 0 || bSign < 0)
					{
						break;
					}

					a = ComplexPolynomial.Subtract(a, b);
				}
			}

			if (a.Degree == 0)
			{
				return ComplexPolynomial.One;
			}
			else
			{
				return a;
			}
		}

		public static IComplexPolynomial ExtendedGCD(IComplexPolynomial left, IComplexPolynomial right, Complex mod)
		{
			IComplexPolynomial rem = ComplexPolynomial.Two;
			IComplexPolynomial a = left.Clone();
			IComplexPolynomial b = right.Clone();
			IComplexPolynomial c = ComplexPolynomial.Zero;


			while (c.CompareTo(ComplexPolynomial.Zero) != 0 && rem.CompareTo(ComplexPolynomial.Zero) != 0 && rem.CompareTo(ComplexPolynomial.One) != 0)
			{
				c = ComplexPolynomial.Divide(a, b, out rem);

				a = b;
				b = rem;
			}

			if (rem.CompareTo(ComplexPolynomial.Zero) != 0 || rem.CompareTo(ComplexPolynomial.One) != 0)
			{
				return ComplexPolynomial.One;
			}

			return rem;
		}

		public static IComplexPolynomial GCD(IComplexPolynomial left, IComplexPolynomial right)
		{
			IComplexPolynomial a = left.Clone();
			IComplexPolynomial b = right.Clone();

			IComplexPolynomial swap;

			if (b.Degree > a.Degree)
			{
				swap = b;
				b = a;
				a = swap;
			}

			while (!(b.Terms.Length > 1 || (b.Terms[0].CoEfficient.Real == 0 || b.Terms[0].CoEfficient.Imaginary == 0)))
			{
				swap = a;
				a = b;
				b = ComplexPolynomial.Mod(swap, b);
			}

			if (a.Degree == 0)
			{
				return ComplexPolynomial.One;
			}
			else
			{
				return a;
			}
		}

		public static IComplexPolynomial GCD(IComplexPolynomial left, IComplexPolynomial right, Complex modulus)
		{
			IComplexPolynomial a = left.Clone();
			IComplexPolynomial b = right.Clone();

			if (b.Degree > a.Degree)
			{
				IComplexPolynomial swap = b;
				b = a;
				a = swap;
			}

			while (!(b.Terms.Length == 0 || b.Terms[0].CoEfficient == 0))
			{
				IComplexPolynomial temp = a;
				a = b;
				b = ComplexPolynomial.ModMod(temp, b, modulus);
			}

			if (a.Degree == 0)
			{
				return ComplexPolynomial.One;
			}
			else
			{
				return a;
			}
		}

		public static IComplexPolynomial ModularInverse(IComplexPolynomial poly, Complex mod)
		{
			return new ComplexPolynomial(ComplexTerm.GetTerms(poly.Terms.Select(trm => (mod - trm.CoEfficient).Mod(mod)).ToArray()));
		}

		public static IComplexPolynomial ModMod(IComplexPolynomial toReduce, IComplexPolynomial modPoly, Complex modPrime)
		{
			return ComplexPolynomial.Modulus(ComplexPolynomial.Mod(toReduce, modPoly), modPrime);
		}

		public static IComplexPolynomial Mod(IComplexPolynomial poly, IComplexPolynomial mod)
		{
			int sortOrder = mod.CompareTo(poly);
			if (sortOrder > 0)
			{
				return poly;
			}
			else if (sortOrder == 0)
			{
				return ComplexPolynomial.Zero;
			}

			IComplexPolynomial remainder = new ComplexPolynomial();
			Divide(poly, mod, out remainder);

			return remainder;
		}

		public static IComplexPolynomial Modulus(IComplexPolynomial poly, Complex mod)
		{
			IComplexPolynomial clone = poly.Clone();
			List<IComplexTerm> terms = new List<IComplexTerm>();

			foreach (IComplexTerm term in clone.Terms)
			{
				Complex remainder = Complex.Divide(term.CoEfficient, mod);

				terms.Add(new ComplexTerm(remainder, term.Exponent));
			}

			// Recalculate the degree
			IComplexTerm[] termArray = terms.SkipWhile(t => t.CoEfficient.Sign() == 0).ToArray();
			IComplexPolynomial result = new ComplexPolynomial(termArray);
			return result;
		}

		public static IComplexPolynomial Divide(IComplexPolynomial left, IComplexPolynomial right)
		{
			IComplexPolynomial remainder = ComplexPolynomial.Zero;
			return ComplexPolynomial.Divide(left, right, out remainder);
		}

		public static IComplexPolynomial Divide(IComplexPolynomial left, IComplexPolynomial right, out IComplexPolynomial remainder)
		{
			if (left == null) throw new ArgumentNullException(nameof(left));
			if (right == null) throw new ArgumentNullException(nameof(right));
			if (right.Degree > left.Degree || right.CompareTo(left) == 1)
			{
				remainder = ComplexPolynomial.Zero; return left;
			}

			int rightDegree = right.Degree;
			int quotientDegree = (left.Degree - rightDegree) + 1;
			Complex leadingCoefficent = right[rightDegree].Clone();

			IComplexPolynomial rem = left.Clone();
			IComplexPolynomial quotient = ComplexPolynomial.Zero;

			// The leading coefficient is the only number we ever divide by
			// (so if right is monic, polynomial division does not involve division at all!)
			for (int i = quotientDegree - 1; i >= 0; i--)
			{
				quotient[i] = Complex.Divide(rem[rightDegree + i], leadingCoefficent);
				rem[rightDegree + i] = Complex.Zero;

				for (int j = rightDegree + i - 1; j >= i; j--)
				{
					rem[j] = Complex.Subtract(rem[j], Complex.Multiply(quotient[i], right[j - i]));
				}
			}

			// Remove zeros
			rem.RemoveZeros();
			quotient.RemoveZeros();

			remainder = rem;
			return quotient;
		}

		public static IComplexPolynomial DivideMod(IComplexPolynomial left, IComplexPolynomial right, Complex mod, out IComplexPolynomial remainder)
		{
			if (left == null) throw new ArgumentNullException(nameof(left));
			if (right == null) throw new ArgumentNullException(nameof(right));
			if (right.Degree > left.Degree || right.CompareTo(left) == 1)
			{
				remainder = ComplexPolynomial.Zero; return left;
			}

			int rightDegree = right.Degree;
			int quotientDegree = (left.Degree - rightDegree) + 1;
			Complex leadingCoefficent = right[rightDegree].Clone().Mod(mod);

			IComplexPolynomial rem = left.Clone();
			IComplexPolynomial quotient = ComplexPolynomial.Zero;

			// The leading coefficient is the only number we ever divide by
			// (so if right is monic, polynomial division does not involve division at all!)
			for (int i = quotientDegree - 1; i >= 0; i--)
			{
				quotient[i] = Complex.Divide(rem[rightDegree + i], leadingCoefficent).Mod(mod);
				rem[rightDegree + i] = Complex.Zero;

				for (int j = rightDegree + i - 1; j >= i; j--)
				{
					rem[j] = Complex.Subtract(rem[j], Complex.Multiply(quotient[i], right[j - i]).Mod(mod)).Mod(mod);
				}
			}

			// Remove zeros
			rem.RemoveZeros();
			quotient.RemoveZeros();

			remainder = rem;
			return quotient;
		}

		public static IComplexPolynomial Multiply(IComplexPolynomial left, IComplexPolynomial right)
		{
			if (left == null) { throw new ArgumentNullException(nameof(left)); }
			if (right == null) { throw new ArgumentNullException(nameof(right)); }

			Complex[] terms = new Complex[left.Degree + right.Degree + 1];

			for (int i = 0; i <= left.Degree; i++)
			{
				for (int j = 0; j <= right.Degree; j++)
				{
					terms[(i + j)] += Complex.Multiply(left[i], right[j]);
				}
			}

			IComplexTerm[] newTerms = ComplexTerm.GetTerms(terms);
			return new ComplexPolynomial(newTerms);
		}

		public static IComplexPolynomial MultiplyMod(IComplexPolynomial poly, Complex multiplier, Complex mod)
		{
			IComplexPolynomial result = poly.Clone();

			foreach (IComplexTerm term in result.Terms)
			{
				Complex newCoefficient = term.CoEfficient;
				if (newCoefficient != 0)
				{
					newCoefficient = (newCoefficient * multiplier);
					term.CoEfficient = (newCoefficient.Mod(mod));
				}
			}

			return result;
		}

		public static IComplexPolynomial PowMod(IComplexPolynomial poly, Complex exp, Complex mod)
		{
			IComplexPolynomial result = poly.Clone();

			foreach (IComplexTerm term in result.Terms)
			{
				Complex newCoefficient = term.CoEfficient;
				if (newCoefficient != 0)
				{
					newCoefficient = Complex.Pow(newCoefficient, exp).Mod(mod);
					if (newCoefficient.Sign() == -1)
					{
						throw new Exception("Complex.ModPow returned negative number");
					}
					term.CoEfficient = newCoefficient;
				}
			}

			return result;
		}

		public static IComplexPolynomial Product(params IComplexPolynomial[] polys)
		{
			return Product(polys.ToList());
		}

		public static IComplexPolynomial Product(IEnumerable<IComplexPolynomial> polys)
		{
			IComplexPolynomial result = null;

			foreach (IComplexPolynomial p in polys)
			{
				if (result == null)
				{
					result = p;
				}
				else
				{
					result = ComplexPolynomial.Multiply(result, p);
				}
			}

			return result;
		}

		public static IComplexPolynomial Square(IComplexPolynomial poly)
		{
			return ComplexPolynomial.Multiply(poly, poly);
		}

		public static IComplexPolynomial Pow(IComplexPolynomial poly, int exponent)
		{
			if (exponent < 0)
			{
				throw new NotImplementedException("Raising a polynomial to a negative exponent not supported. Build this functionality if it is needed.");
			}
			else if (exponent == 0)
			{
				return new ComplexPolynomial(new ComplexTerm[] { new ComplexTerm(1, 0) });
			}
			else if (exponent == 1)
			{
				return poly.Clone();
			}
			else if (exponent == 2)
			{
				return Square(poly);
			}

			IComplexPolynomial total = ComplexPolynomial.Square(poly);

			int counter = exponent - 2;
			while (counter != 0)
			{
				total = ComplexPolynomial.Multiply(total, poly);
				counter -= 1;
			}

			return total;
		}

		#region ExponentiateMod

		/*
        public static IPoly ExponentiateMod(IPoly startPoly, Complex s2, IPoly f, Complex p)
        {
            IPoly result = ComplexPoly.One;
            if (s2 == 0) { return result; }

            IPoly A = startPoly.Clone();

            byte[] byteArray = s2.ToByteArray();
            bool[] bitArray = new BitArray(byteArray).Cast<bool>().ToArray();

            // Remove trailing zeros ?
            if (bitArray[0] == true)
            {
                result = startPoly;
            }

            int i = 1;
            int t = bitArray.Length;
            while (i < t)
            {
                A = ComplexPoly.ModMod(ComplexPoly.Square(A), f, p);
                if (bitArray[i] == true)
                {
                    result = ComplexPoly.ModMod(ComplexPoly.Multiply(A, result), f, p);
                }
                i++;
            }

            return result;
        }
        */

		#endregion

		public static IComplexPolynomial ModPow(IComplexPolynomial poly, Complex exponent, IComplexPolynomial modulus)
		{
			//if (exponent.Sign() == -1)
			//{
			//	throw new NotImplementedException("Raising a polynomial to a negative exponent not supported. Build this functionality if it is needed.");
			//}
			if (exponent == Complex.Zero)
			{
				return ComplexPolynomial.One;
			}
			else if (exponent == Complex.One)
			{
				return poly.Clone();
			}
			else if (exponent == 2)
			{
				return ComplexPolynomial.Square(poly);
			}

			IComplexPolynomial total = ComplexPolynomial.Square(poly);

			Complex counter = exponent - 2;
			while (counter != 0)
			{
				total = Multiply(poly, total);

				if (total.CompareTo(modulus) < 0)
				{
					total = ComplexPolynomial.Mod(total, modulus);
				}

				counter -= 1;
			}

			return total;
		}

		public static IComplexPolynomial Subtract(IComplexPolynomial left, IComplexPolynomial right)
		{
			if (left == null) throw new ArgumentNullException(nameof(left));
			if (right == null) throw new ArgumentNullException(nameof(right));

			Complex[] terms = new Complex[Math.Min(left.Degree, right.Degree) + 1];
			for (int i = 0; i < terms.Length; i++)
			{
				Complex l = left[i];
				Complex r = right[i];
				terms[i] = (l - r);
			}

			IComplexPolynomial result = new ComplexPolynomial(ComplexTerm.GetTerms(terms.ToArray()));
			return result;
		}

		public static IComplexPolynomial Sum(params IComplexPolynomial[] polys)
		{
			return Sum(polys.ToList());
		}

		public static IComplexPolynomial Sum(IEnumerable<IComplexPolynomial> polys)
		{
			IComplexPolynomial result = null;

			foreach (IComplexPolynomial p in polys)
			{
				if (result == null)
				{
					result = p;
				}
				else
				{
					result = ComplexPolynomial.Add(result, p);
				}
			}

			return result;
		}

		public static IComplexPolynomial Add(IComplexPolynomial left, IComplexPolynomial right)
		{
			if (left == null) throw new ArgumentNullException(nameof(left));
			if (right == null) throw new ArgumentNullException(nameof(right));

			Complex[] terms = new Complex[Math.Max(left.Degree, right.Degree) + 1];
			for (int i = 0; i < terms.Length; i++)
			{
				Complex l = left[i];
				Complex r = right[i];
				Complex ttl = (l + r);

				terms[i] = ttl;
			}

			IComplexPolynomial result = new ComplexPolynomial(ComplexTerm.GetTerms(terms.ToArray()));
			return result;
		}

		public static IComplexPolynomial MakeMonic(IComplexPolynomial polynomial, Complex polynomialBase)
		{
			int deg = polynomial.Degree;
			IComplexPolynomial result = new ComplexPolynomial(polynomial.Terms.ToArray());
			if (Complex.Abs(result.Terms[deg].CoEfficient) > 1)
			{
				Complex toAdd = (result.Terms[deg].CoEfficient - 1) * polynomialBase;
				result.Terms[deg].CoEfficient = 1;
				result.Terms[deg - 1].CoEfficient += toAdd;
			}
			return result;
		}

		public static IComplexPolynomial ReduceDegree(IComplexPolynomial polynomial, Complex polynomialBase)
		{
			List<Complex> coefficients = polynomial.Terms.Select(t => t.CoEfficient).ToList();
			Complex leadingCoefficient = coefficients.Last();
			coefficients.Remove(leadingCoefficient);

			Complex toAdd = (leadingCoefficient * polynomialBase);

			leadingCoefficient = coefficients.Last();

			Complex newLeadingCoefficient = leadingCoefficient + toAdd;

			coefficients.Remove(leadingCoefficient);
			coefficients.Add(newLeadingCoefficient);

			return new ComplexPolynomial(ComplexTerm.GetTerms(coefficients.ToArray()));
		}

		public static void MakeCoefficientsSmaller(IComplexPolynomial polynomial, Complex polynomialBase, Complex maxCoefficientSize = default(Complex))
		{
			Complex maxSize = maxCoefficientSize;

			if (maxSize == default(Complex))
			{
				maxSize = polynomialBase;
			}

			int pos = 0;
			int deg = polynomial.Degree;

			while (pos < deg)
			{
				if (pos + 1 > deg)
				{
					return;
				}

				if (Complex.Abs(polynomial[pos]) > Complex.Abs(maxSize) &&
					Complex.Abs(polynomial[pos]) > Complex.Abs(polynomial[pos + 1]))
				{
					Complex diff = polynomial[pos] - maxSize;

					Complex toAdd = (diff / polynomialBase) + 1;
					Complex toRemove = toAdd * polynomialBase;

					polynomial[pos] -= toRemove;
					polynomial[pos + 1] += toAdd;
				}

				pos++;
			}
		}

		public static IComplexPolynomial Parse(string input)
		{
			if (string.IsNullOrWhiteSpace(input)) { throw new ArgumentException(); }

			string inputString = input.Replace(" ", "").Replace("-", "+-");
			string[] stringTerms = inputString.Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries);

			if (!stringTerms.Any()) { throw new FormatException(); }

			List<IComplexTerm> polyTerms = new List<IComplexTerm>();
			foreach (string stringTerm in stringTerms)
			{
				string[] termParts = stringTerm.Split(new char[] { '*' });

				if (termParts.Count() != 2)
				{
					if (termParts.Count() != 1) { throw new FormatException(); }

					string temp = termParts[0];
					if (temp.All(c => char.IsDigit(c) || c == '-'))
					{
						termParts = new string[] { temp, "X^0" };
					}
					else if (temp.All(c => char.IsLetter(c) || c == '^' || c == '-' || char.IsDigit(c)))
					{
						if (temp.Contains("-"))
						{
							temp = temp.Replace("-", "");
							termParts = new string[] { "-1", temp };
						}
						else
						{
							termParts = new string[] { "1", temp };
						}
					}
					else { throw new FormatException(); }
				}

				Complex coefficient = new Complex(double.Parse(termParts[0]), 0.0);

				string[] variableParts = termParts[1].Split(new char[] { '^' });
				if (variableParts.Count() != 2)
				{
					if (variableParts.Count() != 1) { throw new FormatException(); }

					string tmp = variableParts[0];
					if (tmp.All(c => char.IsLetter(c)))
					{
						variableParts = new string[] { tmp, "1" };
					}
				}

				int exponent = int.Parse(variableParts[1]);

				polyTerms.Add(new ComplexTerm(coefficient, exponent));
			}

			if (!polyTerms.Any()) { throw new FormatException(); }

			return new ComplexPolynomial(polyTerms.ToArray());
		}

		public IComplexPolynomial Clone()
		{
			return new ComplexPolynomial(Terms.Select(pt => pt.Clone()).ToArray());
		}

		public string ToString(string format, IFormatProvider formatProvider)
		{
			return this.ToString();
		}

		public override string ToString()
		{
			return ComplexPolynomial.FormatString(this);
		}

		public static string FormatString(IComplexPolynomial polynomial)
		{
			List<string> stringTerms = new List<string>();
			int degree = polynomial.Terms.Length;
			while (--degree >= 0)
			{
				string termString = "";
				IComplexTerm term = polynomial.Terms[degree];

				if (term.CoEfficient == 0)
				{
					if (term.Exponent == 0)
					{
						if (stringTerms.Count == 0) { stringTerms.Add("0"); }
					}
					continue;
				}
				else
				{
					termString = $"{term.CoEfficient.FormatString()}";
				}

				switch (term.Exponent)
				{
					case 0:
						stringTerms.Add($"{term.CoEfficient.FormatString()}");
						break;

					case 1:
						if (term.CoEfficient == 1) stringTerms.Add("X");
						else if (term.CoEfficient == -1) stringTerms.Add("-X");
						else stringTerms.Add($"{term.CoEfficient.FormatString()}*X");
						break;

					default:
						if (term.CoEfficient == 1) stringTerms.Add($"X^{term.Exponent}");
						else if (term.CoEfficient == -1) stringTerms.Add($"-X^{term.Exponent}");
						else stringTerms.Add($"{term.CoEfficient.FormatString()}*X^{term.Exponent}");
						break;
				}
			}
			return string.Join(" + ", stringTerms).Replace(" + -", " - ");
		}

		public bool Equals(IComplexPolynomial other)
		{
			return (this.CompareTo(other) == 0);
		}

		public bool Equals(ComplexPolynomial other)
		{
			return (this.CompareTo(other) == 0);
		}

		public bool Equals(IComplexPolynomial x, IComplexPolynomial y)
		{
			return (x.CompareTo(y) == 0);
		}

		public bool Equals(ComplexPolynomial x, ComplexPolynomial y)
		{
			return (x.CompareTo(y) == 0);
		}

		public int GetHashCode(IComplexPolynomial obj)
		{
			return obj.ToString().GetHashCode();
		}

		public int GetHashCode(ComplexPolynomial obj)
		{
			IComplexPolynomial poly = obj as IComplexPolynomial;
			return poly.GetHashCode(poly);
		}

		public int CompareTo(IComplexPolynomial other)
		{
			if (other == null)
			{
				throw new ArgumentException();
			}

			if (other.Degree != this.Degree)
			{
				if (other.Degree > this.Degree)
				{
					return -1;
				}
				else
				{
					return 1;
				}
			}
			else
			{
				int counter = this.Degree;

				while (counter >= 0)
				{
					Complex thisCoefficient = this[counter];
					Complex otherCoefficient = other[counter];

					if (Complex.Abs(thisCoefficient) < Complex.Abs(otherCoefficient))
					{
						return -1;
					}
					else if (Complex.Abs(thisCoefficient) > Complex.Abs(otherCoefficient))
					{
						return 1;
					}

					counter--;
				}

				return 0;
			}
		}

	}
}
