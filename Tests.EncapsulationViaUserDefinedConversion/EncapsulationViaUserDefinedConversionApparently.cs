using Tests.TestData;

namespace Tests.EncapsulationViaUserDefinedConversion;

public class EncapsulationViaUserDefinedConversionApparently
{
	[Test]
	public void Does_Not_Call_Constructor_When_Using_Conversion()
	{
		// Arrange
		var obj = ClassWithPrivatePositiveIntFieldPubliclyExposedUsingUserDefinedImplicitConversion.FromInteger(2);

		// Act & Assert
		Assert.That(() =>
		{
			var asInt = (int) obj;
			asInt = -3;
		}, Throws.Nothing);
	}

	[Test]
	public void Does_Not_Make_Encapsulated_List_Field_Assignable_Using_Conversion_Back_And_Forth()
	{
		// Arrange
		var obj = ClassWithPrivateListFieldPubliclyExposedUsingUserDefinedImplicitConversion.FromList(new List<int> { 1, 2, 3 });

		// Act
		var asList = (List<int>) obj;
		asList = new List<int>();
		var asList2 = (List<int>) obj;

		// Assert
		Assert.That(asList2, Is.Not.EquivalentTo(new List<int>()));
		Assert.That(asList2, Is.EquivalentTo(new[] { 1, 2, 3 }));
	}

	[Test]
	public void Keeps_Encapsulated_List_Field_Mutable_Using_List_Methods_AndConversion_Back_And_Forth2()
	{
		// Arrange
		var obj = ClassWithPrivateListFieldPubliclyExposedUsingUserDefinedImplicitConversion.FromList(new List<int> { 1, 2, 3 });

		// Act
		var asList = (List<int>) obj;
		asList.Add(4);
		var asList2 = (List<int>) obj;

		// Assert
		Assert.That(asList2, Is.EquivalentTo(new[] { 1, 2, 3, 4 }));
	}
}
