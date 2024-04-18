using System.Collections.Generic;

namespace Db.Application.UseCases.DatabaseQuerying;

public record QueryDatabaseOutput(
	long ExecutionTimeInMs,
	int NumberOfReturnedRows,
	IReadOnlyCollection<object> Result)
: UseCaseOutput();
