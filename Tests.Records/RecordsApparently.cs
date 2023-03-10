using Tests.TestData;

namespace Tests.Records;

public class Tests
{
	[Test]
	public void Are_Cool()
	{
		// Arrange
		int i = 42;
		int j = 43;

		// Act
		PositionalRecordWithTwoIntPropertiesCalledIAndJ r = (i,j) switch
		{
			var (a, b) => new(a,b)
		};

		// Assert
		Assert.That(r.I is 42, Is.True);
	}
}
