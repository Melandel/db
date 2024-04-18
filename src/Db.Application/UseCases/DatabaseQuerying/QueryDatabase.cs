using System.Threading.Tasks;
using Db.Application.UseCases.DatabaseQuerying.ServiceProviders;

namespace Db.Application.UseCases.DatabaseQuerying;

public interface IQueryDatabase: IUseCase<QueryDatabaseInput, QueryDatabaseOutput>
{
}

class QueryDatabase : IQueryDatabase
{
	readonly IDatabaseClientFactory _databaseClientFactory;
	public QueryDatabase(IDatabaseClientFactory databaseFactory)
	{
		_databaseClientFactory = databaseFactory;
	}

	public async Task<QueryDatabaseOutput> Process(QueryDatabaseInput input)
	{
		var database = _databaseClientFactory.CreateDatabaseClientFor(input.DatabaseSpecification);

		var queryExecutionReport = await database.Query(input.Query);

		return new QueryDatabaseOutput(
			queryExecutionReport.ExecutionTimeInMs,
			queryExecutionReport.NumberOfReturnedRows,
			queryExecutionReport.Result);
	}
}
