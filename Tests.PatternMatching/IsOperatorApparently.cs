using Tests.TestData;

namespace Tests.PatternMatching;

class IsOperatorApparently
{
	[Test]
	public void FindsNullableReferenceToBeConvertibleIntoNonNullableValue()
	{
		// Arrange
		int? maybe = 12;

		// Act & Assert
		if (maybe is int number)
			Assert.Pass();

		Assert.Fail();
	}

	[Test]
	public void DoesNotFindNullReferenceToBeConvertibleIntoNonNullableValue()
	{
		// Arrange
		int? maybe = null;

		// Act & Assert
		if (maybe is int number)
			Assert.Fail();
	}

	[Test]
	public void FindsNullableReferenceToBeConvertibleIntoNonNullableValue_Also_When_Using_Not()
	{
		// Arrange
		int? maybe = 12;

		// Act & Assert
		if (maybe is not int number)
			Assert.Fail();
	}

	[Test]
	public void DoesNotFindNullReferenceToBeConvertibleIntoNonNullableValue_Also_When_Using_Not()
	{
		// Arrange
		int? maybe = null;

		// Act & Assert
		if (maybe is not int number)
			Assert.Pass();

		Assert.Fail();
	}

	[Test]
	public void CorrectlyDoesntAssertThatNotNullPropertyConditionIsFalse_ContraryToWhatSonarqubeDetectsUsingCsharpsquidS2583_IfClause()
	{
		// Arrange
		var o = new ClassWithTwoNullablePropertiesCalledN1AndN2 { N1 = 42 };

		// Act
		int v = 0;
		if (o is { N1: not null})      v = 1;
		else if (o is { N2: not null}) v = 2;
		else                           v = 3;

		// Assert
		Assert.That(v, Is.EqualTo(1));
	}

	[Test]
	public void CorrectlyDoesntAssertThatNotNullPropertyConditionIsFalse_ContraryToWhatSonarqubeDetectsUsingCsharpsquidS2583_ElseIfClause()
	{
		// Arrange
		var o = new ClassWithTwoNullablePropertiesCalledN1AndN2 { N2 = 42 };

		// Act
		int v = 0;
		if (o is { N1: not null})      v = 1;
		else if (o is { N2: not null}) v = 2;
		else                           v = 3;

		// Assert
		Assert.That(v, Is.EqualTo(2));
	}
}
