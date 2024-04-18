using System.Collections.Generic;
using Db.Application.UseCases.DatabaseQuerying;

namespace Db.Application.UseCases.DatabasesListing.ServiceProviders;

public interface IDeploymentEnvironmentProvider
{
	IReadOnlyCollection<DeploymentEnvironment> GetAllDeploymentEnvironments();
}
