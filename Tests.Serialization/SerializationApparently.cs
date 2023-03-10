using Tests.TestData;

namespace Tests.Serialization;

public class SerializationApparently
{
	[Test]
	public void Serialize_Properties_Set_Using_Derived_Class_Setter_Hiding_Base_Class_Setter_Differently_When_Using_Newtonsoft_And_When_Using_SystemTextSerialization()
	{
		var derived = new ClassWithIntPropertyCalledIntValueDerivationWithIntPropertyCalledIntValue() { IntValue = 42 };
		var derivedAsBase = derived as ClassWithIntPropertyCalledIntValue;

		Assert.Multiple(() =>
		{
			Assert.That(derived.IntValue, Is.EqualTo(42));
			Assert.That(derivedAsBase.IntValue, Is.EqualTo(0));

			Assert.That(System.Text.Json.JsonSerializer.Serialize(derivedAsBase), Contains.Substring("0").And.Not.Contain("42"));
			Assert.That(System.Text.Json.JsonSerializer.Serialize(derived),       Contains.Substring("42").And.Not.Contain("0"));

			Assert.That(Newtonsoft.Json.JsonConvert.SerializeObject(derivedAsBase), Contains.Substring("42").And.Not.Contain("0"));
			Assert.That(Newtonsoft.Json.JsonConvert.SerializeObject(derived),       Contains.Substring("42").And.Not.Contain("0"));
		});
	}
}
