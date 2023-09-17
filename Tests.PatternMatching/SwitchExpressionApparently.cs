namespace Tests.PatternMatching;

class SwitchExpressionApparently
{
	[Test]
	public void Covers_Null_Value_In_Default_Switch_Branch_With_Var_Syntax()
	{
		// Arrange
		string str = null;

		// Act
		var nb = str switch
		{
			"a" => 0,
			"b" => 1,
			var v when v is { } => 2,
			var v when v is { Length: 0 } => 3,
			var v => 4
		};

		// Assert
		Assert.That(nb, Is.EqualTo(4));
	}
}
