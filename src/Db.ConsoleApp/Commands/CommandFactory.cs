using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Db.ConsoleApp.ExtensionMethods;
using Db.Application.UseCases.DatabasesListing;
using Db.Application.UseCases.DatabaseQuerying;
using Db.Application.UseCases.DatabasesListing.ServiceProviders;

namespace Db.ConsoleApp.Commands;

class CommandFactory
{
	readonly IListDatabases _listDatabases;
	readonly IQueryDatabase _queryDatabase;
	readonly IServiceProvider _serviceProvider;
	public CommandFactory(IServiceProvider serviceProvider)
	{
		_listDatabases = serviceProvider.GetRequiredService<IListDatabases>();
		_queryDatabase = serviceProvider.GetRequiredService<IQueryDatabase>();
		_serviceProvider = serviceProvider;
	}

	public Command CreateCommandFromCommandLineArgs(string[] args)
	{
		var deploymentEnvironmentProvider = _serviceProvider.GetRequiredService<IDeploymentEnvironmentProvider>();
		return args switch
		{
			_ when args.Length == 1 && args[0] == "list" => new ListAllDatabasesNames(_listDatabases, args.ToListDatabasesInput()),
			_ when args.Length  < 2                      => new DisplayHighLevelConsoleAppDocumentation(_serviceProvider, $"The arguments passed to the console application are not valid ({string.Join(',', args)})."),
			_ when args.Any(a => a.Contains("help"))     => new DisplayHighLevelConsoleAppDocumentation(_serviceProvider),
			_                                            => new QueryDatabase(_queryDatabase, args.ToQueryDatabaseInput(), deploymentEnvironmentProvider)
		};
	}
}
