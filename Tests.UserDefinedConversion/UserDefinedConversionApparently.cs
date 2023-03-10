using Tests.TestData;

namespace Tests.UserDefinedConversion;

public class UserDefinedConversionApparently
{
	[Test]
	public void Creates_A_New_Object_When_Used_With_Parens_Syntax()
	{
		// Arrange
		var obj = ClassWithPrivateListFieldPubliclyExposedUsingUserDefinedImplicitConversion.FromList(new List<int> { 1, 2, 3 });

		// Act
		var asList = (List<int>) obj;

		// Assert
		Assert.IsFalse(ReferenceEquals(obj, asList));
	}

	[Test]
	public void Creates_A_New_Object_With_Fields_Pointing_Towards_The_Same_Reference_For_ReferenceTypeTypes_Like_List()
	{
		// Arrange
		var obj = ClassWithPrivateListFieldPubliclyExposedUsingUserDefinedImplicitConversion.FromList(new List<int> { 1, 2, 3 });

		// Act
		var asList = (List<int>) obj;
		asList.Add(4);

		// Assert
		Assert.That((List<int>)obj, Is.EquivalentTo(new[] { 1, 2, 3, 4}));
	}

	[Test]
	public void Does_Not_Make_Encapsulated_List_Field_Assignable_Using_Conversion_Back_And_Forth()
	{
		// Arrange
		var obj = ClassWithPrivateListFieldPubliclyExposedUsingUserDefinedImplicitConversion.FromList(new List<int> { 1, 2, 3 });

		// Act
		var asList = (List<int>) obj;
		asList = new List<int> { 2, 4, 6 };
		var asList2 = (List<int>) obj;

		// Assert
		Assert.That(asList2, Is.Not.EquivalentTo(new[] { 2, 4, 6 }));
		Assert.That(asList2, Is.EquivalentTo(new[] { 1, 2, 3 }));
	}
}
