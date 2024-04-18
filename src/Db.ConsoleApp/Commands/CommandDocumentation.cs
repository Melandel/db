using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Db.ConsoleApp.Commands;

record CommandDocumentation
{
	public string Section1 { get; }
	public string Section2 { get; }
	public string Section3 { get; }
	public string Section4 { get; }
	public string Prefix { get; }
	CommandDocumentation(
		string section1,
		string section2,
		string section3,
		string section4,
		string prefix = "")
	{
		Section1 = section1 ?? "";
		Section2 = section2 ?? "";
		Section3 = section3 ?? "";
		Section4 = section4 ?? "";
		Prefix = prefix ?? "";
	}

	public static CommandDocumentation ForDatabasesListing()
	=> new("list", null, null, "List queryable database names");

	public static CommandDocumentation ForSqlServerDatabaseQuerying(
		IEnumerable<string> possibleDeploymentEnvironments,
		IEnumerable<string> possibleQueryModels,
		string description)
	{
		var section1 = "{database}";
		var section2 = string.Join('/', possibleDeploymentEnvironments);
		var argumentInDescriptionMatch = Regex.Match(description, @"{(\w*)}");
		var section3 = (possibleQueryModels.ToArray(), description) switch
		{
			(var models, var descr) when argumentInDescriptionMatch.Success && models is [ "" ] => $"{{{argumentInDescriptionMatch.Groups[1].Value}}}",
			(var models, var descr) when argumentInDescriptionMatch.Success => $"{string.Join('|', possibleQueryModels)} {{{(argumentInDescriptionMatch.Groups[1].Value)}}}",
				_ => string.Join('|', possibleQueryModels)
		};
		var section4 = description;
		var prefix = "SqlServer";

		return new(section1, section2, section3, section4, prefix);
	}

	public static CommandDocumentation ForCosmosDbDatabaseQuerying(
		IEnumerable<string> possibleDeploymentEnvironments,
		IEnumerable<string> possibleQueryModels,
		string description)
	{
		var section1 = "{database}";
		var section2 = string.Join('/', possibleDeploymentEnvironments);
		var argumentInDescriptionMatch = Regex.Match(description, @"{(\w*)}");
		var section3 = (possibleQueryModels.ToArray(), description) switch
		{
			(var models, var descr) when argumentInDescriptionMatch.Success && models is [ "" ] => $"{{{argumentInDescriptionMatch.Groups[1].Value}}}",
			(var models, var descr) when argumentInDescriptionMatch.Success => $"{string.Join('|', possibleQueryModels)} {{{(argumentInDescriptionMatch.Groups[1].Value)}}}",
				_ => string.Join('|', possibleQueryModels)
		};
		var section4 = description;
		var prefix = "CosmosDB";

		return new(section1, section2, section3, section4, prefix);
	}

	public static readonly CommandDocumentation VerticalSeparator = new(null, null, null, null, null);
}

