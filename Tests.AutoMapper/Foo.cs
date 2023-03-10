namespace Tests.AutoMapper;

public class Base
{
	public int Value { get; set; }
}
public class Derived : Base
{
	public new int Value { get; set; }
}

public class Foo
{
	[Test]
	public void Toto()
	{
		var derived = new Derived() { Value = 42 };
		Base asBase = derived;

		Assert.Multiple(() =>
		{
			Assert.That(asBase.Value, Is.EqualTo(0));
			Assert.That((asBase as Derived).Value, Is.EqualTo(42));

			Assert.That(System.Text.Json.JsonSerializer.Serialize(asBase),            Contains.Substring("0").And.Not.Contain("42"));
			Assert.That(System.Text.Json.JsonSerializer.Serialize(asBase as Base),    Contains.Substring("0").And.Not.Contain("42"));
			Assert.That(System.Text.Json.JsonSerializer.Serialize(asBase as Derived), Contains.Substring("42").And.Not.Contain("0"));

			Assert.That(Newtonsoft.Json.JsonConvert.SerializeObject(asBase),            Contains.Substring("42").And.Not.Contain("0"));
			Assert.That(Newtonsoft.Json.JsonConvert.SerializeObject(asBase as Base),    Contains.Substring("42").And.Not.Contain("0"));
			Assert.That(Newtonsoft.Json.JsonConvert.SerializeObject(asBase as Derived), Contains.Substring("42").And.Not.Contain("0"));
		});
	}
}
