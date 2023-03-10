namespace Tests.TestData;

public class ClassWithPrivateIntFieldPubliclyExposedUsingUserDefinedImplicitConversion
{
	readonly int _encapsulated;
	ClassWithPrivateIntFieldPubliclyExposedUsingUserDefinedImplicitConversion(int encapsulated)
	=> _encapsulated = encapsulated switch
	{
		< 0 => throw new Exception("foo"),
		_ => encapsulated
	};

	public static ClassWithPrivateIntFieldPubliclyExposedUsingUserDefinedImplicitConversion FromInteger(int i)
	=> new ClassWithPrivateIntFieldPubliclyExposedUsingUserDefinedImplicitConversion(i);

	public static implicit operator int(ClassWithPrivateIntFieldPubliclyExposedUsingUserDefinedImplicitConversion obj)
	=> obj._encapsulated;
}
