namespace Tests.PatternMatching;

public class RelationalPatternsApparently
{
	[Test]
	public void Finds_False_When_Comparing_Null_To_A_Numerical_Value()
	{
		decimal? i = null;
		Assert.That(i is <= 0, Is.False);
		Assert.That(i is >= 0, Is.False);
		Assert.That(i is <  0, Is.False);
		Assert.That(i is >  0, Is.False);
	}
}
