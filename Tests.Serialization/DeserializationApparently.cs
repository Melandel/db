namespace Tests.Serialization;

public class DeserializationApparently
{
	[Test]
	public void Ignores_CaseSensitivity_ByDefault_When_Using_Newtonsoft_But_Not_When_Using_SystemTextJsonSerialization()
	{
		// Arrange
		var json = @"{ ""key"": ""foo"", ""value"": ""bar"" }";

		// Act
		var kvp = System.Text.Json.JsonSerializer.Deserialize<KeyValuePair<string, string>>(json);

		// Assert
		Assert.Multiple(() =>
		{
			Assert.That(Newtonsoft.Json.JsonConvert.DeserializeObject<KeyValuePair<string, string>>(json).Key, Is.EqualTo("foo"));
			Assert.That(Newtonsoft.Json.JsonConvert.DeserializeObject<KeyValuePair<string, string>>(json).Value, Is.EqualTo("bar"));

			Assert.That(System.Text.Json.JsonSerializer.Deserialize<KeyValuePair<string, string>>(json).Key, Is.Null);
			Assert.That(System.Text.Json.JsonSerializer.Deserialize<KeyValuePair<string, string>>(json).Value, Is.Null);

			var jsonSerializerOptions = new System.Text.Json.JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
			Assert.That(System.Text.Json.JsonSerializer.Deserialize<KeyValuePair<string, string>>(json, jsonSerializerOptions).Key, Is.EqualTo("foo"));
			Assert.That(System.Text.Json.JsonSerializer.Deserialize<KeyValuePair<string, string>>(json, jsonSerializerOptions).Value, Is.EqualTo("bar"));
		});
	}
}

