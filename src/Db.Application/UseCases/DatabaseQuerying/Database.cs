namespace Db.Application.UseCases.DatabaseQuerying;

public record Database(
	DatabaseName Name,
	DeploymentEnvironment DeploymentEnvironment);

