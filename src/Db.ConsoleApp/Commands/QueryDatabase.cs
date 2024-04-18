using Db.Application.UseCases.DatabaseQuerying;
using Db.Application.UseCases.DatabasesListing.ServiceProviders;
using Db.ConsoleApp.ExtensionMethods;

namespace Db.ConsoleApp.Commands;

class QueryDatabase : Command<IQueryDatabase, QueryDatabaseInput, QueryDatabaseOutput>
{
	readonly IDeploymentEnvironmentProvider _deploymentEnvironmentProvider;
	public QueryDatabase(IQueryDatabase useCase, QueryDatabaseInput useCaseInput, IDeploymentEnvironmentProvider deploymentEnvironmentProvider)
	: base(useCase, useCaseInput)
	{
		_deploymentEnvironmentProvider = deploymentEnvironmentProvider;
	}

	protected override string Format(QueryDatabaseOutput executionOutput)
	=> executionOutput.ToIndentedString();
}
