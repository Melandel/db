namespace Db.Infrastructure;

class DatabasesConfiguration
{
	public SqlServerDatabaseConfiguration[] SqlServer { get; set; }
	public CosmosDatabaseConfiguration[] CosmosDB { get; set; }
}
