using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Db.Application.UseCases.DatabasesListing.ServiceProviders;
using Microsoft.Extensions.DependencyInjection;

namespace Db.ConsoleApp.Commands;

class DisplayHighLevelConsoleAppDocumentation : Command
{
	static readonly string ProgramName = System.Diagnostics.Process.GetCurrentProcess().ProcessName.ToLower();
	readonly IServiceProvider _serviceProvider;
	readonly string _prefix;
	public DisplayHighLevelConsoleAppDocumentation(IServiceProvider serviceProvider, string prefix = null)
	{
		_serviceProvider = serviceProvider;
		_prefix = prefix;
	}

	public override Task<string> Run()
	{
		var commandTypes = AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(s => s.GetTypes())
			.Where(p => typeof(Command).IsAssignableFrom(p) && !p.IsAbstract && p != GetType());

		var documentationByCommandType = BuildDocumentationByCommandType();
		var commandDocumentations = documentationByCommandType.SelectMany(kvp => kvp.Value());
		var prefixMaxLength = commandDocumentations.Max(doc => doc.Prefix.Length);
		var section1MaxLength = commandDocumentations.Max(doc => doc.Section1.Length);
		var section2MaxLength = commandDocumentations.Max(doc => doc.Section2.Length);
		var section3MaxLength = commandDocumentations.Max(doc => doc.Section3.Length);
		var section4MaxLength = commandDocumentations.Max(doc => doc.Section4.Length);
		var startingRow = $"┌{new string('-', 16+prefixMaxLength+section1MaxLength+section2MaxLength+section3MaxLength+section4MaxLength)}┐";
		var finishingRow = $"└{new string('-', 16+prefixMaxLength+section1MaxLength+section2MaxLength+section3MaxLength+section4MaxLength)}┘";
		var emptyRow = $"├{new string('-', 16+prefixMaxLength+section1MaxLength+section2MaxLength+section3MaxLength+section4MaxLength)}┤";

		var messageBuilder = new StringBuilder(startingRow);
		foreach(var commandType in commandTypes)
		{
			if (!documentationByCommandType.ContainsKey(commandType))
			{
				throw new Exception($"{commandType.Name} does not have a documentation yet.");
			}

			var documentationsForThisCommand = documentationByCommandType[commandType]();
			messageBuilder.AppendLine((messageBuilder.ToString() == startingRow) ? "" : emptyRow);

			foreach (var documentationForThisCommand in documentationsForThisCommand)
			{
				if (documentationForThisCommand == CommandDocumentation.VerticalSeparator)
				{
					messageBuilder.AppendLine(emptyRow);
					continue;
				}
				messageBuilder.Append("| ");
				messageBuilder.Append(documentationForThisCommand.Prefix.PadRight(prefixMaxLength + 1));
				messageBuilder.Append(" | ");
				messageBuilder.Append(ProgramName);
				messageBuilder.Append(' ');
				messageBuilder.Append(documentationForThisCommand.Section1.PadRight(section1MaxLength + 1));
				messageBuilder.Append(documentationForThisCommand.Section2.PadRight(section2MaxLength + 1));
				messageBuilder.Append(documentationForThisCommand.Section3.PadRight(section3MaxLength + 1));
				messageBuilder.Append(" | ");
				messageBuilder.Append(documentationForThisCommand.Section4.PadRight(section4MaxLength + 1));
				messageBuilder.Append(" |");
				messageBuilder.AppendLine();
			}
		}
		messageBuilder.AppendLine(finishingRow);

		return Task.FromResult(messageBuilder.ToString());
	}

	Dictionary<Type, Func<IReadOnlyCollection<CommandDocumentation>>> BuildDocumentationByCommandType()
	{
		var documentationByCommandType = new Dictionary<Type, Func<IReadOnlyCollection<CommandDocumentation>>>();
		documentationByCommandType.Add(typeof(ListAllDatabasesNames), () => new[] { CommandDocumentation.ForDatabasesListing() });
		documentationByCommandType.Add(typeof(QueryDatabase), () =>
		{
			var deploymentEnvironmentProvider = _serviceProvider.GetRequiredService<IDeploymentEnvironmentProvider>();
			var possibleDeploymentEnvironments = deploymentEnvironmentProvider.GetAllDeploymentEnvironments()
				.Select(env => (string)env)
				.ToArray();

			var commandDocumentations = new List<CommandDocumentation>();
			commandDocumentations.AddRange(BuildCosmosDbQueriesDocumentation(possibleDeploymentEnvironments));
			commandDocumentations.Add(CommandDocumentation.VerticalSeparator);
			commandDocumentations.AddRange(BuildSqlServerQueriesDocumentation(possibleDeploymentEnvironments));

			return commandDocumentations;
		});

		return documentationByCommandType;
	}

	static IEnumerable<CommandDocumentation> BuildSqlServerQueriesDocumentation(string[] possibleDeploymentEnvironments)
	{
		var sqlQueryModelsByQueryModelDescriptions = new Dictionary<string, string[]>
		{
			{ "Find top 20 rows of {table_name}", new[] { "" } },
			{ "Run {sql_query}", new[] { "" } },
			{ "Find table names LIKE '%{pattern}%'", new[] { "table", "tb", "t" } },
			{ "Find column names LIKE '%{pattern}%'", new[] { "column", "col", "c" } },
			{ "Find tables names LIKE '{pattern}'", new[] { "TABLE", "TB", "T" } },
			{ "Find column names LIKE '{pattern}'", new[] { "COLUMN", "COL", "C" } },
			{ "Find routines/views/triggers LIKE '%{pattern}%' (name or body)", new[] { "routine", "rt", "r" } },
			{ "Find routines/views/triggers LIKE '{pattern}' (name or body)", new[] { "ROUTINE", "RT", "R" } },
			{ "Find tables referencing '%{table_name_pattern}%'", new[] { "referencing", "ref" } },
			{ "Find tables referenced by '{table_name_pattern}'", new[] { "referenced", "refed" } },
		};
		return sqlQueryModelsByQueryModelDescriptions.Select(queryModelDescription => CommandDocumentation.ForSqlServerDatabaseQuerying(
			possibleDeploymentEnvironments,
			queryModelDescription.Value,
			queryModelDescription.Key));
	}

	static IEnumerable<CommandDocumentation> BuildCosmosDbQueriesDocumentation(string[] possibleDeploymentEnvironments)
	{
		var cosmosQueryModelsByQueryModelDescriptions = new Dictionary<string, string[]>
		{
			{ "Find one document", new[] { "" } },
			{ "Find at most 10 documents with 'c.id = \"{id}\"'", new[] { "" } },
		};
		return cosmosQueryModelsByQueryModelDescriptions.Select(queryModelDescription => CommandDocumentation.ForCosmosDbDatabaseQuerying(
			possibleDeploymentEnvironments,
			queryModelDescription.Value,
			queryModelDescription.Key));
	}
}
