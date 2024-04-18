using System.Collections.Generic;

namespace Db.Application.UseCases.DatabasesListing;

public record ListDatabasesOutput(IReadOnlyCollection<string> Values) : UseCaseOutput()
{
}

