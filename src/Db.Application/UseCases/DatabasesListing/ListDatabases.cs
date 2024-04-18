using System.Linq;
using System.Threading.Tasks;
using Db.Application.UseCases.DatabasesListing.ServiceProviders;

namespace Db.Application.UseCases.DatabasesListing;

public interface IListDatabases : IUseCase<ListDatabasesInput, ListDatabasesOutput>
{
}

public class ListDatabases : IListDatabases
{
	readonly IDatabaseNameProvider _databaseNameProvider;
	public ListDatabases(IDatabaseNameProvider databaseNameProvider)
	{
		_databaseNameProvider = databaseNameProvider;
	}

	public Task<ListDatabasesOutput> Process(ListDatabasesInput input)
	{
		var databaseNames = _databaseNameProvider
			.GetAllDatabaseNames()
			.Select(dbName => (string) dbName)
			.ToArray();
		var output = new ListDatabasesOutput(databaseNames);
		return Task.FromResult(output);
	}
}
