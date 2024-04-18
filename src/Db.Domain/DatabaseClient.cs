using System.Threading.Tasks;

namespace Db.Domain
{
	public abstract class DatabaseClient
	{
		public abstract Task<object[]> Query(DatabaseQuery query);
	}
}
