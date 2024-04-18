using System.Threading.Tasks;

namespace Db.Domain
{
	public abstract class DatabaseClient
	{
		public abstract Task<QueryExecutionReport> Query(DatabaseQuery query);
	}
}
