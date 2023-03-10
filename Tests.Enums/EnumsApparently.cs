using Tests.TestData;

namespace Tests.Enums;

public class EnumsApparently
{
	[Test]
	public void AreAssignedWithDefaultValueZero_EvenWhenFirstEnumValueIsExplicitedToOne()
	{
		var containingClass = new ClassWithEnumStartingWithExplicitedValueOneProperty();

		Assert.That((int)containingClass.EnumStartingWithExplicitedValueOne, Is.Zero);
	}
}
