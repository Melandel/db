namespace Db.Domain;

public enum DatabaseQueryType
{
	TechnicalDefaultEnumValue = 0,
	Custom = 1,
	TablesAndColumnsBrowsing = 2,
	RoutinesViewsAndTriggersBrowsing = 3,
	TablesReferencingTargetTable = 4,
	TablesReferencedByTargetTable = 5,
}
