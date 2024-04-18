namespace Db.Infrastructure;

public class CosmosDatabaseInstanceConfiguration
{
	public string Environment { get; set; }
	public string EndpointUrl { get; set; }
	public string AuthorizationKey { get; set; }
	public string DatabaseId { get; set; }
	public string ContainerId { get; set; }
}
