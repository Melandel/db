using Microsoft.Extensions.DependencyInjection;
using Db.Infrastructure;
using Db.Application.UseCases.DatabaseQuerying.ServiceProviders;
using Microsoft.Extensions.Configuration;
using System.IO;
using Db.Application.UseCases.DatabasesListing.ServiceProviders;

namespace Db
{
	public static class DependencyInjection
	{
			public static IServiceCollection AddServiceProviders(this IServiceCollection services)
			{
				if (services is null)
					return null;

				var config = new ConfigurationBuilder()
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile("config.json", optional: false)
					.Build();
				services.Configure<DbConfiguration>(config);

				services.AddTransient<IDatabaseNameProvider, DatabaseNameProvider>();
				services.AddTransient<IDeploymentEnvironmentProvider, DeploymentEnvironmentProvider>();
				services.AddTransient<IDatabaseClientFactory, DatabaseClientFactory>();

				return services;
			}
	}
}
