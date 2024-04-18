using System;
using System.Linq;
using Db.Application.UseCases.DatabaseQuerying;
using Db.Application.UseCases.DatabaseQuerying.ServiceProviders;
using Db.Domain;
using Microsoft.Extensions.Options;

namespace Db.Infrastructure;

class DatabaseClientFactory : IDatabaseClientFactory
{
	readonly DbConfiguration _databasesConfiguration;
	public DatabaseClientFactory(IOptions<DbConfiguration> databasesConfiguration)
	{
		_databasesConfiguration = databasesConfiguration.Value;
	}

	public DatabaseClient CreateDatabaseClientFor(Database database)
	{
		var matchingSqlServerDatabaseConfigurations = _databasesConfiguration.Databases.SqlServer.Where(db => string.Equals(db.Name, database.Name, StringComparison.InvariantCultureIgnoreCase));
		if (matchingSqlServerDatabaseConfigurations.Any())
		{
			var databaseInstance = matchingSqlServerDatabaseConfigurations.Last().Instances.Single(inst => inst.Environment == database.DeploymentEnvironment);
			return SqlServerDatabaseClient.Create(databaseInstance.ConnectionString);
		}

		var matchingCosmosDatabaseConfigurations = _databasesConfiguration.Databases.CosmosDB.Where(db => string.Equals(db.Name, database.Name, StringComparison.InvariantCultureIgnoreCase));
		if (matchingCosmosDatabaseConfigurations.Any())
		{
			var databaseInstance = matchingCosmosDatabaseConfigurations.Last().Instances.Single(inst => inst.Environment == database.DeploymentEnvironment);
			return CosmosDatabaseClient.Create(databaseInstance.EndpointUrl, databaseInstance.AuthorizationKey, databaseInstance.DatabaseId, databaseInstance.ContainerId);
		}

		return null;
	}
}
