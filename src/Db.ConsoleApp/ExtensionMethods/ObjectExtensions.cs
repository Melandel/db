using Newtonsoft.Json;

namespace Db.ConsoleApp.ExtensionMethods;

public static class ObjectExtensions
{
	public static string ToIndentedString(this object[] objects)
	{
		if (objects == null)
		{
			return string.Empty;
		}
		if (objects.Length == 1)
		{
			var singledOut = objects[0];
			return JsonConvert.SerializeObject(JsonConvert.DeserializeObject(singledOut.ToString()), Formatting.Indented);
		}
		else
		{
			var arrayAsString = JsonConvert.SerializeObject(JsonConvert.DeserializeObject($"[{string.Join(",", objects)}]"), Formatting.Indented);
			//return $"[ // {objects.Length} elements" + arrayAsString[1..];
			//System.Console.WriteLine($"{{ \"count\": {objects.Length}, \"_\": {System.Environment.NewLine}{arrayAsString}}}");
			return $"{{ \"count\": {objects.Length}, \"_\": {System.Environment.NewLine}{arrayAsString}}}";
		}
	}
}
