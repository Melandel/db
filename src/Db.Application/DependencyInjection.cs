using Microsoft.Extensions.DependencyInjection;
using Db.Application.UseCases.DatabasesListing;
using Db.Application.UseCases.DatabaseQuerying;

namespace Db.Application
{
	public static class DependencyInjection
	{
			public static IServiceCollection AddApplicationUseCases(this IServiceCollection services)
			{
				if (services is null)
					return null;

				services.AddTransient<IQueryDatabase, QueryDatabase>();
				services.AddTransient<IListDatabases, ListDatabases>();
				return services;
			}
	}
}
