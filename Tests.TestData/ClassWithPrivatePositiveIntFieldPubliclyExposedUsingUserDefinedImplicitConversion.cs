namespace Tests.TestData;

public class ClassWithPrivatePositiveIntFieldPubliclyExposedUsingUserDefinedImplicitConversion
{
	readonly int _encapsulated;
	ClassWithPrivatePositiveIntFieldPubliclyExposedUsingUserDefinedImplicitConversion(int encapsulated)
	=> _encapsulated = encapsulated switch
	{
		< 0 => throw new Exception("foo"),
		_ => encapsulated
	};

	public static ClassWithPrivatePositiveIntFieldPubliclyExposedUsingUserDefinedImplicitConversion FromInteger(int i)
	=> new ClassWithPrivatePositiveIntFieldPubliclyExposedUsingUserDefinedImplicitConversion(i);

	public static implicit operator int(ClassWithPrivatePositiveIntFieldPubliclyExposedUsingUserDefinedImplicitConversion obj)
	=> obj._encapsulated;
}
