using System.Collections.Generic;
using System.Linq;
using Db.Application.UseCases.DatabaseQuerying;
using Newtonsoft.Json;

namespace Db.ConsoleApp.ExtensionMethods;

public static class QueryDatabaseOutputExtensions
{
	public static string ToIndentedString(this QueryDatabaseOutput output)
	{
		var objects = output.Result.ToArray();
		if (objects == null || objects.Length == 0)
		{
			return string.Empty;
		}

		var isSingleRow = objects.Length == 1;

		string serialized;
		var isTablesAndColumnsDescriptions = objects[0] is KeyValuePair<string, Dictionary<string, string>>;
		if (isTablesAndColumnsDescriptions)
		{
			var tablesAndColumnsDescriptions = new Dictionary<string, Dictionary<string, string>>();
			foreach (var o in objects)
			{
				var kvp = (KeyValuePair<string, Dictionary<string, string>>) o;
				tablesAndColumnsDescriptions.Add(kvp.Key, kvp.Value);
			}
			serialized = JsonConvert.SerializeObject(
				tablesAndColumnsDescriptions,
				Formatting.Indented);
		}
		else
		{
			var toSerialize = isSingleRow
				? objects[0]
				: $"[{string.Join(",", objects)}]";

			serialized = JsonConvert.SerializeObject(
				JsonConvert.DeserializeObject(toSerialize.ToString()),
				Formatting.Indented);
		}

		return isSingleRow
			? serialized
			: $"{{ \"count\": {output.NumberOfReturnedRows}, \"duration\": \"{output.ExecutionTimeInMs}ms\", \"_\":{System.Environment.NewLine}{serialized}}}";
	}
}
