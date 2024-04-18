using Db.Domain;

namespace Db.Application.UseCases.DatabaseQuerying.ServiceProviders;

public interface IDatabaseClientFactory
{
	DatabaseClient CreateDatabaseClientFor(Database database);
}
