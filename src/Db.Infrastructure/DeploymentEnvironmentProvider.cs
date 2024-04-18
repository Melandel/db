using System.Collections.Generic;
using System.Linq;
using Db.Application.UseCases.DatabaseQuerying;
using Db.Application.UseCases.DatabasesListing.ServiceProviders;
using Db.Infrastructure;
using Microsoft.Extensions.Options;

namespace Db;

class DeploymentEnvironmentProvider : IDeploymentEnvironmentProvider
{
	readonly DbConfiguration _databasesConfiguration;
	public DeploymentEnvironmentProvider(IOptions<DbConfiguration> databasesConfiguration)
	{
		_databasesConfiguration = databasesConfiguration.Value;
	}

	public IReadOnlyCollection<DeploymentEnvironment> GetAllDeploymentEnvironments()
	{
		return _databasesConfiguration.Databases.SqlServer.SelectMany(db => db.Instances.Select(dbi => dbi.Environment))
			.Concat(_databasesConfiguration.Databases.CosmosDB.SelectMany(db => db.Instances.Select(dbi => dbi.Environment)))
			.Distinct()
			.Select(environmentName => DeploymentEnvironment.From(environmentName))
			.ToArray();
	}
}
