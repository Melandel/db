using NUnit.Framework;
using Tests.TestData;

namespace Tests.ConversionUsingKeywordAs;

public class ConversionUsingKeywordAsApparently
{
	[Test]
	public void Foo()
	{
		// Arrange
		var foo = new ClassWithClassPropertyCalledProperty();
		foo.Property = null;

		// Act
		var prop = foo.Property as object;

		// Assert
		Assert.That(prop, Is.Null);
	}
}
