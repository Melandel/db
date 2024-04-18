using System.Collections.Generic;
using Db.Application.UseCases.DatabaseQuerying;

namespace Db.Application.UseCases.DatabasesListing.ServiceProviders;

public interface IDatabaseNameProvider
{
	IReadOnlyCollection<DatabaseName> GetAllDatabaseNames();
}
