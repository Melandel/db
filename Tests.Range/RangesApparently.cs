namespace Tests.Range;

public class RangesApparently
{
	[Test]
	public void Start_From_The_Item_AFTER_The_Nth_Position()
	{
		// Arrange
		var str = "abcdef";
		var three = "abc".Length;

		// Act
		var slice = str[three..];

		// Assert
		Assert.That(slice, Is.EqualTo("def"));
		Assert.That(slice, Is.EqualTo(str.Substring(three)));
	}

	[Test]
	public void Is_Equivalent_To_Substring_For_String_Type()
	{
		// Arrange
		var str = "abcdef";
		var three = "abc".Length;
		var substring = str.Substring(three);

		// Act
		var slice = str[three..];

		// Assert
		Assert.That(slice, Is.EqualTo(substring));
	}
}
