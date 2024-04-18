using Db.Application.UseCases.DatabaseQuerying;
using Db.Application.UseCases.DatabasesListing;

namespace Db.ConsoleApp.ExtensionMethods;

public static class ArgsToInputExtensions
{
	public static ListDatabasesInput ToListDatabasesInput(this string[] args)
		=> new ListDatabasesInput();

	public static QueryDatabaseInput ToQueryDatabaseInput(this string[] args)
	{
		var argDatabaseName = args[0];
		var argDeploymentEnvironment = args[1];
		var queryArgs = args[2..];
		return QueryDatabaseInput.Create(
			databaseName: DatabaseName.From(argDatabaseName),
			deploymentEnvironment: DeploymentEnvironment.From(argDeploymentEnvironment),
			query: queryArgs.Length switch
			{
				1 => queryArgs[0] switch
				{
					"table" or "tb" or "t" => Domain.DatabaseQuery.ThatBrowsesAllTables,
					"routine" or "rt" or "r" => Domain.DatabaseQuery.ThatBrowsesAllRoutinesViewsAndTriggers,
					var tableName => Domain.DatabaseQuery.ThatSelectsTop20Rows(tableName)
				},
				2 => (queryArgs[0], queryArgs[1]) switch
				{
					("table"  or "tb"  or "t", var tableNameFilter)  => Domain.DatabaseQuery.ThatBrowsesTablesContaining(tableNameFilter),
					("column" or "col" or "c", var columnNameFilter) => Domain.DatabaseQuery.ThatBrowsesTablesContainingColumn(columnNameFilter),
					("TABLE"  or "TB"  or "T", var tableNameFilter)  => Domain.DatabaseQuery.ThatBrowsesTablesExactlyNamed(tableNameFilter),
					("COLUMN" or "COL" or "C", var columnNameFilter) => Domain.DatabaseQuery.ThatBrowsesTablesContainingColumnExactlyNamed(columnNameFilter),
					("routine" or "rt" or "r", var routineNameFilter) => Domain.DatabaseQuery.ThatBrowsesRoutinesViewsAndTriggersNamesContaining(routineNameFilter),
					("ROUTINE" or "RT" or "R", var routineNameFilter) => Domain.DatabaseQuery.ThatBrowsesRoutinesViewsAndTriggersExactlyNamed(routineNameFilter),
					("referencing" or "ref", var exactTableName) => Domain.DatabaseQuery.ThatFindsTablesReferencing(exactTableName),
					("referencedby" or "refed", var exactTableName) => Domain.DatabaseQuery.ThatFindsTablesReferencedBy(exactTableName),
					("zco_code" or "zco" or "code", var code) => Domain.DatabaseQuery.ThatSelectsTop20RefCodes(code),
					("prd_produit" or "produit" or "prd", var produitId) => Domain.DatabaseQuery.ThatSelectsTop20Products(produitId),
					_ => Domain.DatabaseQuery.ThatIsComposedOf(queryArgs)
				},
				3 => (queryArgs[0], queryArgs[1], queryArgs[2]) switch
				{
					("table" or "tb" or "t", var tableNameFilter, var columnNameFilter) => Domain.DatabaseQuery.ThatBrowsesTablesContainingColumn(tableNameFilter, columnNameFilter),
					("routine" or "rt" or "r", var routineNameFilter, var routineBodyFilter) => Domain.DatabaseQuery.ThatBrowsesRoutinesViewsAndTriggersNamesAndBodiesContaining(routineNameFilter, routineBodyFilter),
					_ => Domain.DatabaseQuery.ThatIsComposedOf(queryArgs)
				},
				_ => Domain.DatabaseQuery.ThatIsComposedOf(queryArgs)
			}
		);
	}
}
