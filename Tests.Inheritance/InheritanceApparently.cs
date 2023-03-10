using Tests.TestData;

namespace Tests.Inheritance;

public class InheritanceApparently
{
	[Test]
	public void Sets_Only_Base_Class_Property_And_Not_Derived_Class_Property_When_Using_Base_Type_Setter_From_Instance_Created_From_Derived_Type_Constructor_As_Base_Type()
	{
		ClassWithIntPropertyCalledIntValue derivedAsBase = new ClassWithIntPropertyCalledIntValueDerivationWithIntPropertyCalledIntValue();
		var derivedAsDerived = derivedAsBase as ClassWithIntPropertyCalledIntValueDerivationWithIntPropertyCalledIntValue;
		derivedAsBase.IntValue = 42;

		Assert.Multiple(() =>
		{
			Assert.That(derivedAsBase.IntValue, Is.EqualTo(42));
			Assert.That(derivedAsDerived.IntValue, Is.EqualTo(0));
		});
	}

	[Test]
	public void Sets_Only_Base_Class_Property_And_Not_Derived_Class_Property_When_Using_Base_Type_Setter_From_Instance_Created_From_Derived_Type_Constructor_As_Derived_Type()
	{
		var derivedAsDerived = new ClassWithIntPropertyCalledIntValueDerivationWithIntPropertyCalledIntValue();
		var derivedAsBase = derivedAsDerived as ClassWithIntPropertyCalledIntValue;
		derivedAsBase.IntValue = 42;
		derivedAsDerived.IntValue = 1337;

		Assert.Multiple(() =>
		{
			Assert.That(derivedAsBase.IntValue, Is.EqualTo(42));
			Assert.That(derivedAsDerived.IntValue, Is.EqualTo(1337));
		});
	}
}
