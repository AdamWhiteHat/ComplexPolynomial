
namespace ExtendedArithmetic
{
	public interface IComplexTerm : ICloneable<IComplexTerm>
	{
		int Exponent { get; }
		System.Numerics.Complex CoEfficient { get; set; }
	}
}
