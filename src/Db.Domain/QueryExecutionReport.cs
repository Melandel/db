using System.Collections.Generic;

namespace Db.Domain;

public record QueryExecutionReport(
	long ExecutionTimeInMs,
	int NumberOfReturnedRows,
	IReadOnlyCollection<object> Result
	);
