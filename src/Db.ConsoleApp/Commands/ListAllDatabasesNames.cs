using System;
using Db.Application.UseCases.DatabasesListing;

namespace Db.ConsoleApp.Commands;

class ListAllDatabasesNames : Command<IListDatabases, ListDatabasesInput, ListDatabasesOutput>
{
	public ListAllDatabasesNames(IListDatabases useCase, ListDatabasesInput useCaseInput)
	: base(useCase, useCaseInput)
	{
	}

	protected override string Format(ListDatabasesOutput executionOutput)
	=> string.Join(Environment.NewLine, executionOutput.Values);
}
