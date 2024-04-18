namespace Tests.Range;

public class RangesApparently
{
	[Test]
	public void Start_AFTER_The_Nth_Character()
	{
		// Arrange
		var str = "abcdef";
		var three = "abc".Length;

		// Act
		var slice = str[three..];

		// Assert
		Assert.That(slice, Is.EqualTo("def"));
	}

	[Test]
	public void End_Including_The_Item_AT_The_Nth_Position()
	{
		// Arrange
		var str = "abcdef";
		var three = "abc".Length;

		// Act
		var slice = str[1..three];

		// Assert
		Assert.That(slice, Is.EqualTo("bc"));
	}

	[Test]
	public void Is_Equivalent_To_Substring()
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

	[TestCase(1)]
	[TestCase(9)]
	public void Throws_When_Index_GreaterThen_StringLength(int offset)
	{
		// Arrange
		var str = "abc";
		var numberGreaterThanStringLength = str.Length + offset;

		// Act & Assert
		Assert.That(
			() => { _ = str[numberGreaterThanStringLength..]; },
			Throws.InstanceOf<ArgumentOutOfRangeException>());
	}
}
