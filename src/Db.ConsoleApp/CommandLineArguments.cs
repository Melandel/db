using Db.Application.UseCases.DatabaseQuerying;

namespace Db.ConsoleApp;

public class CommandLineArguments
{
	public DatabaseName DatabaseName { get; }
	public DeploymentEnvironment DeploymentEnvironment { get; }
	public string Query { get; }

	CommandLineArguments(DatabaseName databaseName, DeploymentEnvironment deploymentEnvironment, string query)
	{
		DatabaseName = databaseName;
		DeploymentEnvironment = deploymentEnvironment;
		Query = query;
	}

	public static CommandLineArguments CreateFrom(string[] args)
	{
		var databaseName = DatabaseName.From(args[0]);
		var deploymentEnvironment = DeploymentEnvironment.From(args[1]);
		var query = BuildQuery(args[2..]);
		return new(databaseName, deploymentEnvironment, query);
	}

	static string BuildQuery(string[] queryArgs)
	{
		return queryArgs.Length switch
		{
			0 =>  "SELECT * FROM c ORDER BY c._ts DESC OFFSET 0 LIMIT 1",
			1 => $"SELECT * FROM c WHERE c.id = \"{queryArgs[0]}\"",
			_ => string.Join(' ', queryArgs) + " OFFSET 0 LIMIT 10"
		};
	}
}
