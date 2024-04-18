namespace Db.Infrastructure;

class SqlServerDatabaseConfiguration
{
	public string Name { get; set; }
	public string Description { get; set; }
	public SqlServerDatabaseInstanceConfiguration[] Instances { get; set; }
}
