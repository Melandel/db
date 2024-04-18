using System;
using System.Collections.Generic;
using System.Linq;
using Db.Domain.ExtensionMethods;

namespace Db.Domain;

public class DatabaseQuery
{
	readonly string[] _queryElements;

	public string[] Elements => _queryElements;
	public DatabaseQueryType Type { get; }

	DatabaseQuery(string[] queryElements, DatabaseQueryType databaseQueryType)
	{
		_queryElements = queryElements switch
		{
			null => Array.Empty<string>(),
			_ => queryElements.ToArray()
		};
		Type = databaseQueryType switch
		{
			DatabaseQueryType.TechnicalDefaultEnumValue => throw new Exception("DatabaseQueryType.TechnicalDefaultEnumValue"),
			var type => type
		};
	}

	public static DatabaseQuery ThatIsComposedOf(IEnumerable<string> queryElements)
	=> new DatabaseQuery(queryElements?.ToArray(), DatabaseQueryType.Custom);

	public static DatabaseQuery ThatSelectsTop20Rows(string tableName)
	=> new DatabaseQuery(new[] { "SELECT", "TOP 20", "*", "FROM", tableName}, DatabaseQueryType.Custom);

	public static DatabaseQuery ThatSelectsTop20Products(string productId)
	=> new DatabaseQuery(new[] { $"SELECT TOP 20 * FROM prd_produit WHERE prd_Id = '{productId.TrimQuotes()}'"}, DatabaseQueryType.Custom);

	public static DatabaseQuery ThatSelectsTop20RefCodes(string code)
	=> new DatabaseQuery(new[] { $"SELECT TOP 20 * FROM zco_code WHERE zco_code = '{code.TrimQuotes()}'"}, DatabaseQueryType.Custom);

	public static DatabaseQuery ThatBrowsesTablesContaining(string tableName)
	=> new DatabaseQuery(new[] { tableName.ToLIKE() }, DatabaseQueryType.TablesAndColumnsBrowsing);

	public static DatabaseQuery ThatBrowsesTablesExactlyNamed(string tableName)
	=> new DatabaseQuery(new[] { tableName }, DatabaseQueryType.TablesAndColumnsBrowsing);

	public static DatabaseQuery ThatBrowsesTablesContainingColumn(string columnName)
	=> new DatabaseQuery(new[] { "%", columnName.ToLIKE() }, DatabaseQueryType.TablesAndColumnsBrowsing);

	public static DatabaseQuery ThatBrowsesTablesContainingColumnExactlyNamed(string columnName)
	=> new DatabaseQuery(new[] { "%", columnName }, DatabaseQueryType.TablesAndColumnsBrowsing);

	public static DatabaseQuery ThatBrowsesTablesContainingColumn(string tableName, string columnName)
	=> new DatabaseQuery( new[] { tableName.ToLIKE(), columnName.ToLIKE() }, DatabaseQueryType.TablesAndColumnsBrowsing);

	public static DatabaseQuery ThatBrowsesAllTables
	=> new DatabaseQuery(new[] { "%" }, DatabaseQueryType.TablesAndColumnsBrowsing);

	public static DatabaseQuery ThatBrowsesRoutinesViewsAndTriggersNamesContaining(string routineName)
	=> new DatabaseQuery(new[] { routineName.ToLIKE() }, DatabaseQueryType.RoutinesViewsAndTriggersBrowsing);

	public static DatabaseQuery ThatBrowsesRoutinesViewsAndTriggersExactlyNamed(string routineName)
	=> new DatabaseQuery(new[] { routineName }, DatabaseQueryType.RoutinesViewsAndTriggersBrowsing);

	public static DatabaseQuery ThatBrowsesRoutinesViewsAndTriggersNamesAndBodiesContaining(string routineName, string routineBody)
	=> new DatabaseQuery(new[] { routineName.ToLIKE(), routineBody.ToLIKE() }, DatabaseQueryType.RoutinesViewsAndTriggersBrowsing);

	public static DatabaseQuery ThatBrowsesAllRoutinesViewsAndTriggers
	=> new DatabaseQuery(new[] { "%" }, DatabaseQueryType.RoutinesViewsAndTriggersBrowsing);

	public static DatabaseQuery ThatFindsTablesReferencing(string tableName)
	=> new DatabaseQuery(new[] { tableName }, DatabaseQueryType.TablesReferencingTargetTable);

	public static DatabaseQuery ThatFindsTablesReferencedBy(string tableName)
	=> new DatabaseQuery(new[] { tableName }, DatabaseQueryType.TablesReferencedByTargetTable);
}
