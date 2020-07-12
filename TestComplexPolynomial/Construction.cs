using System;
using System.Numerics;
using ExtendedArithmetic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestComplexPolynomial
{
	[TestClass]
	public class Construction
	{
		private TestContext m_testContext;
		public TestContext TestContext { get { return m_testContext; } set { m_testContext = value; } }

		[TestMethod]
		public void TestParse()
		{
			string expectedFirst = "7*X^2 + 3*X - 2";
			string expectedSecond = "2*X - 2";
			string expectedThird = "X^4 + 8*X^3 + 21*X^2 + 22*X + 8";
			string expectedFourth = "X^4 + 8*X^3 + 21*X^2 + 22*X + 8";
			string expectedFifth = "X^3 + 6*X^2 + 11*X + 6";

			IComplexPolynomial firstPoly = ComplexPolynomial.Parse(expectedFirst);
			IComplexPolynomial secondPoly = ComplexPolynomial.Parse(expectedSecond);
			IComplexPolynomial thirdPoly = ComplexPolynomial.Parse(expectedThird);
			IComplexPolynomial firstFourth = ComplexPolynomial.Parse(expectedFourth);
			IComplexPolynomial firstFifth = ComplexPolynomial.Parse(expectedFifth);

			string actualFirst = firstPoly.ToString();
			string actualSecond = secondPoly.ToString();
			string actualThird = thirdPoly.ToString();
			string actualFouth = firstFourth.ToString();
			string actualFifth = firstFifth.ToString();

			Assert.AreEqual(expectedFirst, actualFirst, $"Test Parse({expectedFirst})");
			Assert.AreEqual(expectedSecond, actualSecond, $"Test Parse({expectedSecond})");
			Assert.AreEqual(expectedThird, actualThird, $"Test Parse({expectedThird})");
			Assert.AreEqual(expectedFourth, actualFouth, $"Test Parse({expectedFourth})");
			Assert.AreEqual(expectedFifth, actualFifth, $"Test Parse({expectedFifth})");
		}

		[TestMethod]
		public void TestFromRoots()
		{
			Complex oneTwelvth = new Complex(1d / 12d, 0);
			Complex minusOneTwelvth = Complex.Negate(oneTwelvth);
			Complex imaginaryOneTwelvth = new Complex(0, 1d / 12d);
			Complex imaginaryTwelve = new Complex(0, 12);

			IComplexPolynomial poly3 = ComplexPolynomial.FromRoots(oneTwelvth, imaginaryOneTwelvth, 12);

			string expected = "X^3 + (-12.08333 - 0.08333𝐢)*X^2 + (1 + 1.00694444444444𝐢)*X + (0 - 0.08333𝐢)";
			string actual = poly3.ToString();

			TestContext.WriteLine($"Expecting: {expected}");
			TestContext.WriteLine($"Actual:    {actual}");

			TestContext.WriteLine(actual);

			Assert.AreEqual(expected, actual);
		}
	}
}
