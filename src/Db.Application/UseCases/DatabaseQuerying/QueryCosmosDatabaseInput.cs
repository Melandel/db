namespace Db.Application.UseCases.DatabaseQuerying;

public record QueryDatabaseInput(Database DatabaseSpecification, Domain.DatabaseQuery Query) : UseCaseInput()
{
	public static QueryDatabaseInput Create(DatabaseName databaseName, DeploymentEnvironment deploymentEnvironment, Domain.DatabaseQuery query)
	=> new QueryDatabaseInput(new Database(databaseName, deploymentEnvironment), query);
}
