namespace Tests.PatternMatching;

class ListPatternsApparently
{
	[TestCase(null, false)]
	[TestCase(0, false)]
	[TestCase(1, true)]
	[TestCase(2, true)]
	public void Can_Be_Used_To_Check_NullOrEmpty_Collections(int? collectionSize, bool hasAtLeastOneItem)
	{
		// Arrange
		var collection = collectionSize switch
		{
			null => null,
			int size => Enumerable.Range(0, size).ToList()
		};

		// Act & Assert
		Assert.That(collection is [ _, .. ], Is.EqualTo(hasAtLeastOneItem));
	}
}
