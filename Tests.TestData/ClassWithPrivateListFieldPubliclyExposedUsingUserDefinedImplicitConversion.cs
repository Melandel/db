namespace Tests.TestData;

public class ClassWithPrivateListFieldPubliclyExposedUsingUserDefinedImplicitConversion
{
	readonly List<int> _encapsulated;
	ClassWithPrivateListFieldPubliclyExposedUsingUserDefinedImplicitConversion(List<int> encapsulated)
	=> _encapsulated = encapsulated;

	public static ClassWithPrivateListFieldPubliclyExposedUsingUserDefinedImplicitConversion FromList(List<int> l)
	=> new ClassWithPrivateListFieldPubliclyExposedUsingUserDefinedImplicitConversion(l);

	public static implicit operator List<int>(ClassWithPrivateListFieldPubliclyExposedUsingUserDefinedImplicitConversion obj)
	=> obj._encapsulated;
}
