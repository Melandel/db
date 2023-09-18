using System.Text.RegularExpressions;

namespace Tests.RegularExpressions;

public class RegularExpressionsApparently
{
	[Test]
	public void Apply_RegexOptions_IgnoreCase_To_The_Whole_Pattern()
	{
		// Arrange
		var patternEndingOnUpperCase = new Regex("^abc[A-Z]$", RegexOptions.IgnoreCase);

		// Act
		var textEndingOnLowerCase = "ABCd";
		var isMatch = patternEndingOnUpperCase.IsMatch(textEndingOnLowerCase);

		// Assert
		Assert.That(isMatch, Is.True);
	}

	[TestCase("abcD", true)]
	[TestCase("AbcD", true)]
	[TestCase("ABcD", true)]
	[TestCase("ABCD", true)]
	[TestCase("ABCd", false)]
	public void Can_Match_Partially_CaseInsensitive_Patterns_With_QuestionMarkI_Delimiters(string input, bool expectedToMatch)
	{
		// Arrange
		var patternMatchingCaseInsensitiveAbc = new Regex("^(?i)abc(?-i)[A-Z]$");

		// Act
		var isMatch = patternMatchingCaseInsensitiveAbc.IsMatch(input);

		// Assert
		Assert.That(isMatch, Is.EqualTo(expectedToMatch));
	}
}
