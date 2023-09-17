namespace Tests.Range;

public class RangesApparently
{
	[Test]
	public void Start_From_The_Item_AFTER_The_Nth_Position()
	{
		// Arrange
		var str = "abcdef";

		// Act
		var slice = str["abc".Length..];

		// Assert
		Assert.That(slice, Is.EqualTo("def"));
	}
}
