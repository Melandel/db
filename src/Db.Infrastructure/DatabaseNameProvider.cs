using System.Collections.Generic;
using System.Linq;
using Db.Application.UseCases.DatabaseQuerying;
using Db.Application.UseCases.DatabasesListing.ServiceProviders;
using Db.Infrastructure;
using Microsoft.Extensions.Options;

namespace Db;

class DatabaseNameProvider : IDatabaseNameProvider
{
	readonly DbConfiguration _databasesConfiguration;
	public DatabaseNameProvider(IOptions<DbConfiguration> databasesConfiguration)
	{
		_databasesConfiguration = databasesConfiguration.Value;
	}

	public IReadOnlyCollection<DatabaseName> GetAllDatabaseNames()
	{
		return _databasesConfiguration.Databases.SqlServer.Select(db => DatabaseName.From(db.Name))
			.Concat(_databasesConfiguration.Databases.CosmosDB.Select(db => DatabaseName.From(db.Name)))
			.ToArray();
	}
}
