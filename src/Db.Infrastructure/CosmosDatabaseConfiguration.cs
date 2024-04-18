namespace Db.Infrastructure;

class CosmosDatabaseConfiguration
{
	public string Name { get; set; }
	public string Description { get; set; }
	public CosmosDatabaseInstanceConfiguration[] Instances { get; set; }
}
