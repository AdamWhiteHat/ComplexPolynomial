using System;
using System.Numerics;
using ExtendedArithmetic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestComplexPolynomial
{
	[TestClass]
	public class Arithmetic
	{
		private TestContext m_testContext;
		public TestContext TestContext { get { return m_testContext; } set { m_testContext = value; } }

		[TestMethod]
		public void TestAddition()
		{
			string expected = "24*X - 1";

			IComplexPolynomial first = ComplexPolynomial.Parse("12*X + 2");
			IComplexPolynomial second = ComplexPolynomial.Parse("12*X - 3");

			IComplexPolynomial result = ComplexPolynomial.Add(first, second);
			string actual = result.ToString();

			TestContext.WriteLine("Test Addition:");
			TestContext.WriteLine($"({first}) + ({second})");
			TestContext.WriteLine($"Expected: {expected}");
			TestContext.WriteLine($"Actual:    {actual}");

			Assert.AreEqual(expected, actual, $"Test Addition");
		}

		[TestMethod]
		public void TestSubtraction()
		{
			string expected = "7*X^2 + X";

			IComplexPolynomial first = ComplexPolynomial.Parse("7*X^2 + 3*X - 2");
			IComplexPolynomial second = ComplexPolynomial.Parse("2*X - 2");

			IComplexPolynomial result = ComplexPolynomial.Subtract(first, second);
			string actual = result.ToString();

			TestContext.WriteLine("Test Subtraction:");
			TestContext.WriteLine($"({first}) - ({second})");
			TestContext.WriteLine("");
			TestContext.WriteLine($"Expected: {expected}");
			TestContext.WriteLine($"Actual   = {actual}");


			Assert.AreEqual(expected, actual, $"Test Subtraction");
		}

		[TestMethod]
		public void TestMultiply()
		{
			string expected = "144*X^2 - 12*X - 6";

			IComplexPolynomial first = ComplexPolynomial.Parse("12*X + 2");
			IComplexPolynomial second = ComplexPolynomial.Parse("12*X - 3");

			IComplexPolynomial result = ComplexPolynomial.Multiply(first, second);
			string actual = result.ToString();

			TestContext.WriteLine("Test Multiply:");
			TestContext.WriteLine($"({first}) * ({second})");
			TestContext.WriteLine("");
			TestContext.WriteLine($"Expected: {expected}");
			TestContext.WriteLine($"Actual   = {actual}");


			Assert.AreEqual(expected, actual, $"Test Multiply");
		}

		[TestMethod]
		public void TestDivide()
		{
			string expected = "24*X - 1";

			IComplexPolynomial first = ComplexPolynomial.Parse("288*X^2 + 36*X - 2");
			IComplexPolynomial second = ComplexPolynomial.Parse("12*X + 2");

			IComplexPolynomial result = ComplexPolynomial.Divide(first, second);
			string actual = result.ToString();

			TestContext.WriteLine("Test Divide:");
			TestContext.WriteLine($"({first}) / ({second})");
			TestContext.WriteLine("");
			TestContext.WriteLine($"Expected: {expected}");
			TestContext.WriteLine($"Actual   = {actual}");

			Assert.AreEqual(expected, actual, $"Test Divide");
		}

		[TestMethod]
		public void TestSquare()
		{
			string expected = "144*X^2 + 24*X + 1";

			IComplexPolynomial first = ComplexPolynomial.Parse("12*X + 1");

			IComplexPolynomial result = ComplexPolynomial.Square(first);
			string actual = result.ToString();

			TestContext.WriteLine("Test Square:");
			TestContext.WriteLine($"({first})^2");
			TestContext.WriteLine("");
			TestContext.WriteLine($"Expected: {expected}");
			TestContext.WriteLine($"Actual   = {actual}");

			Assert.AreEqual(expected, actual, $"Test Square");
		}

		[TestMethod]
		public void TestGCD()
		{
			string polyString1 = "X^2 + 7*X + 6";
			string polyString2 = "X^2 - 5*X - 6";
			string expected = "X + 1";

			IComplexPolynomial first = ComplexPolynomial.Parse(polyString1);
			IComplexPolynomial second = ComplexPolynomial.Parse(polyString2);

			IComplexPolynomial result = ComplexPolynomial.GCD(first, second);
			string actual = result.ToString();

			TestContext.WriteLine("Test GCD:");
			TestContext.WriteLine($"GCD({first}, {second})");
			TestContext.WriteLine("");
			TestContext.WriteLine($"Expected: {expected}");
			TestContext.WriteLine($"Actual   = {actual}");

			Assert.AreEqual(expected, actual, $"Test GCD");
		}

		[TestMethod]
		public void TestDerivative()
		{
			string expected = "576*X + 36";

			IComplexPolynomial first = ComplexPolynomial.Parse("288*X^2 + 36*X - 2");
			IComplexPolynomial result = ComplexPolynomial.GetDerivativePolynomial(first);
			string actual = result.ToString();

			TestContext.WriteLine("Test Derivative:");
			TestContext.WriteLine($"f' where f(x) = ({first})");
			TestContext.WriteLine("");
			TestContext.WriteLine($"Expected: {expected}");
			TestContext.WriteLine($"Actual   = {actual}");

			Assert.AreEqual(expected, actual, $"Test Derivative");
		}
	}
}
