using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Db.Infrastructure;

public class SqlServerDatabaseClient : Db.Domain.DatabaseClient, IDisposable
{
	public SqlConnection SqlConnection { get; }
	SqlServerDatabaseClient(SqlConnection sqlConnection)
	{
		SqlConnection = sqlConnection;
		if (SqlConnection.State is ConnectionState.Closed)
		{
			try
			{
				SqlConnection.Open();
			}
			catch (Exception ex)
			{
				var sb = new StringBuilder();
				var e = ex;
				while (e is not null)
				{
					sb.AppendLine(e.Message);
					e = e.InnerException;
				}

				Console.WriteLine($"Could not open SqlConnection for {SqlConnection.ConnectionString}: {sb}");
				SqlConnection.Dispose();
			}
		}
	}

	public static SqlServerDatabaseClient Create(string connectionString)
	{
		var sqlConnection = new SqlConnection(connectionString);
		return new SqlServerDatabaseClient(sqlConnection);
	}

	public override Task<Domain.QueryExecutionReport> Query(Domain.DatabaseQuery query)
	{
		var sqlCommand = new SqlCommand(BuildLiteralQuery(query), SqlConnection);
		var sw = new Stopwatch();

		sw.Start();
		using var reader = sqlCommand.ExecuteReader();
		sw.Stop();


		var columnNames = Enumerable
			.Range(0, reader.FieldCount)
			.Select(i => reader.GetName(i))
			.ToArray();

		var rows = new List<object>();
		var tableAndColumnsQueryCols = new[] {"TABLE_NAME", "COLUMN_NAME", "COLUMN_TYPE", "IS_NULLABLE", "COLUMN_DESCRIPTION", "SCRIPT_MISE_A_JOUR_DOCUMENTATION" };

		if (columnNames.SequenceEqual(tableAndColumnsQueryCols))
		{
			var tableAndColumnsDescriptions = new Dictionary<string, Dictionary<string, string>>();
			while (reader.Read())
			{
				var tableName = reader.GetString(0);
				if (!tableAndColumnsDescriptions.ContainsKey(tableName))
				{
					tableAndColumnsDescriptions.Add(tableName, new Dictionary<string, string>());
				}
				var tableCols = tableAndColumnsDescriptions[tableName];
				var colName = reader.GetString(1);
				var isPrimaryKey = colName.StartsWith("|||| ");
				var colType = reader.GetString(2);
				var isColNullable = reader.GetString(3);
				var nullDescription = isColNullable == "NO" ? "" : " null";
				tableCols.Add(colName.Trim(new[]{'|', ' '}), $"{(isPrimaryKey ? "[PK] " : "")}{colType.ToUpper()}{nullDescription}");
			}
			rows = tableAndColumnsDescriptions.Select(kvp => (object)kvp).ToList();
		}
		else
		{
			while (reader.Read())
			{
				var rowAsDictionary = columnNames
					.GroupBy(columnName => columnName) // 👈 duplicate columns keys from inner joins
					.ToDictionary(
						columnNameGrouping => columnNameGrouping.First(),
						columnNameGrouping => GetValue(reader, columnNameGrouping.Key));
				//Console.WriteLine($"types: {string.Join(',', columnNames.Select(columnName => reader.GetDataTypeName(columnName)))}");
				//Console.WriteLine($"row: {string.Join(',', rowAsDictionary.Select(_ => _.ToString()))}");
				var rowAsSerializedDictionary = JsonConvert.SerializeObject(rowAsDictionary);
				var rowAsObject = JsonConvert.DeserializeObject(rowAsSerializedDictionary);
				rows.Add(rowAsObject);
			}
		}

		return Task.FromResult(
			new Domain.QueryExecutionReport(
				sw.ElapsedMilliseconds,
				rows.Count,
				rows));
	}

	static object GetValue(SqlDataReader reader, string columnName)
	{
		if (reader.IsDBNull(columnName)) return null;
		SqlDbType dbType = (SqlDbType)Enum.Parse(typeof(SqlDbType), reader.GetDataTypeName(columnName), true);

		return dbType switch
		{
			SqlDbType.BigInt => reader.GetInt64(columnName) as object,
			SqlDbType.Binary => reader.GetString(columnName),
			SqlDbType.Image => reader.GetString(columnName),
			SqlDbType.Timestamp => BitConverter.ToUInt64((byte[])reader.GetValue(columnName), 0),
			SqlDbType.VarBinary => reader.GetString(columnName),
			SqlDbType.Bit => reader.GetBoolean(columnName),
			SqlDbType.Char => reader.GetValue(columnName).ToString().TrimEnd(),
			SqlDbType.NChar => reader.GetString(columnName).TrimEnd(),
			SqlDbType.NText => reader.GetString(columnName).TrimEnd(),
			SqlDbType.NVarChar => reader.GetString(columnName).TrimEnd(),
			SqlDbType.Text => reader.GetString(columnName).TrimEnd(),
			SqlDbType.Xml => reader.GetString(columnName).TrimEnd(),
			SqlDbType.VarChar => reader.GetValue(columnName).ToString().TrimEnd(),
			SqlDbType.DateTime => reader.GetDateTime(columnName).ToString("s"),
			SqlDbType.SmallDateTime => reader.GetDateTime(columnName).ToString("s"),
			SqlDbType.Date => reader.GetDateTime(columnName).ToString("s"),
			SqlDbType.Time => (TimeSpan)reader.GetValue(columnName),
			SqlDbType.DateTime2 => reader.GetDateTime(columnName).ToString("s"),
			SqlDbType.DateTimeOffset => reader.GetDateTime(columnName),
			SqlDbType.Decimal => reader.GetDecimal(columnName),
			SqlDbType.Money => reader.GetDecimal(columnName),
			SqlDbType.SmallMoney => reader.GetDecimal(columnName),
			SqlDbType.Float => reader.GetDouble(columnName),
			SqlDbType.Int => reader.GetInt32(columnName),
			SqlDbType.Real => reader.GetFloat(columnName),
			SqlDbType.UniqueIdentifier => reader.GetGuid(columnName),
			SqlDbType.SmallInt => reader.GetInt16(columnName),
			SqlDbType.TinyInt => (int) reader.GetByte(columnName),
			SqlDbType.Variant => reader.GetValue(columnName),
			SqlDbType.Udt => reader.GetValue(columnName),
			SqlDbType.Structured => reader.GetValue(columnName), // DataTable
			_ => throw new NotImplementedException($"SqlDbType not supported: {reader.GetDataTypeName(columnName)}")
		};
	}

	public void Dispose()
	{
		SqlConnection.Dispose();
	}

	string BuildLiteralQuery(Domain.DatabaseQuery query)
	{
	return query.Type switch
	{
		Domain.DatabaseQueryType.TechnicalDefaultEnumValue => throw new Exception("DatabaseQueryType.TechnicalDefaultEnumValue"),
		Domain.DatabaseQueryType.Custom => string.Join(' ', query.Elements),
		Domain.DatabaseQueryType.TablesAndColumnsBrowsing => String.Format(BrowseTablesAndColumnsWhenTableNameLikeAndColumnNameLike, query.Elements.FirstOrDefault() ?? "%", query.Elements.Skip(1).FirstOrDefault() ?? "%"),
		Domain.DatabaseQueryType.RoutinesViewsAndTriggersBrowsing => String.Format(BrowseRoutesViewsAndTriggers, query.Elements.FirstOrDefault() ?? "%", query.Elements.Skip(1).FirstOrDefault() ?? "%"),
		Domain.DatabaseQueryType.TablesReferencingTargetTable => String.Format(BrowseTablesReferencingTargetTable, query.Elements.FirstOrDefault() ?? "%"),
		Domain.DatabaseQueryType.TablesReferencedByTargetTable => String.Format(BrowseTablesReferencedByTargetTable, query.Elements.FirstOrDefault() ?? "%"),
		_ => throw new Exception("DatabaseQueryType")
	};
	}

	const string BrowseTablesAndColumnsWhenTableNameLikeAndColumnNameLike = @"
DECLARE @Table VARCHAR(100) =/*LIKE*/ '{0}', @Colonne VARCHAR(100) =/*LIKE*/ '{1}'; WITH CataloguePKs AS( SELECT KU.TABLE_NAME, KU.COLUMN_NAME AS ColonnePK FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS TC with(nolock) INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KU with(nolock) ON TC.CONSTRAINT_NAME = KU.CONSTRAINT_NAME WHERE TC.CONSTRAINT_TYPE = 'PRIMARY KEY'), ListingDescriptions AS ( SELECT sysTable.NAME AS TABLE_NAME, sysColumn.NAME AS COLUMN_NAME ,CAST(sysProp.value AS VARCHAR(8000)) AS COLUMN_DESCRIPTION, ColonnePK FROM sys.tables sysTable INNER JOIN sys.columns sysColumn ON sysTable.object_id = sysColumn.object_id LEFT JOIN sys.extended_properties sysProp ON sysTable.object_id = sysProp.major_id AND sysColumn.column_id = sysProp.minor_id AND sysProp.NAME = 'MS_Description' LEFT OUTER JOIN CataloguePKs ON CataloguePKs.TABLE_NAME = sysTable.name AND CataloguePKs.ColonnePK = sysColumn.name), ColonneEnBase AS (SELECT ISC.TABLE_NAME, CASE WHEN (ColonnePK IS NOT NULL) THEN  '|||| ' + ISC.COLUMN_NAME + ' ||||' ELSE ISC.COLUMN_NAME END as COLUMN_NAME, ISC.DATA_TYPE + CASE WHEN ISC.DATA_TYPE IN ('varchar','char','varbinary','binary','text') THEN '(' + CASE WHEN ISC.CHARACTER_MAXIMUM_LENGTH = -1 THEN 'max' ELSE CAST(ISC.CHARACTER_MAXIMUM_LENGTH as VARCHAR(10)) END + ')' WHEN ISC.DATA_TYPE IN ('nvarchar', 'nchar', 'ntext') THEN '(' + CASE WHEN ISC.CHARACTER_MAXIMUM_LENGTH = -1 THEN 'max' ELSE CAST(ISC.CHARACTER_MAXIMUM_LENGTH/2 as VARCHAR(10)) END + ')' WHEN ISC.DATA_TYPE IN ('time2','datetimeoffset') THEN '(' + CAST(ISC.NUMERIC_SCALE AS VARCHAR(10)) + ')' WHEN ISC.DATA_TYPE = 'decimal' THEN '(' + CAST(ISC.NUMERIC_PRECISION AS VARCHAR(10)) + ',' + CAST(ISC.NUMERIC_SCALE AS VARCHAR(10)) + ')' ELSE '' END as COLUMN_TYPE, IS_NULLABLE, CASE WHEN (ColonnePK IS NOT NULL) THEN -1 ELSE ISC.ORDINAL_POSITION END AS ORDINAL_POSITION_WITH_PK_FIRST, ListingDescriptions.COLUMN_DESCRIPTION FROM INFORMATION_SCHEMA.COLUMNS ISC with(nolock) LEFT OUTER JOIN ListingDescriptions ON ISC.TABLE_NAME = ListingDescriptions.TABLE_NAME AND ISC.COLUMN_NAME= ListingDescriptions.COLUMN_NAME), DocumentationColonne AS (SELECT TABLE_NAME, COLUMN_NAME, COLUMN_TYPE, IS_NULLABLE, COLUMN_DESCRIPTION, ORDINAL_POSITION_WITH_PK_FIRST, REPLACE(REPLACE('BEGIN' +CHAR(13)+CHAR(10)+CHAR(9)+ 'DECLARE @columnID NVARCHAR(200) = N''{{TABLE_NAME}}.{{COLUMN_NAME_RAW}}'',' +CHAR(13)+CHAR(10)+CHAR(9)+CHAR(9)+CHAR(9)+ '@columnDOC NVARCHAR(4000) = N''' + ISNULL(REPLACE(COLUMN_DESCRIPTION, '''', '''''') +CHAR(13)+CHAR(10)+REPLICATE(CHAR(9),10)+REPLICATE(' ',2), '') + '(Aide au formatage)' +CHAR(13)+CHAR(10)+REPLICATE(CHAR(9),10)+REPLICATE(' ',2) + '[Préfixe pour les PKs] Cette TABLE recense des/les----' +CHAR(13)+CHAR(10)+REPLICATE(CHAR(9),10)+REPLICATE(' ',2)+ '[Préfixe pour les BOOLs] Indique si----(=-1) ou non(=0)' +CHAR(13)+CHAR(10)+REPLICATE(CHAR(9),10)+REPLICATE(' ',2)+ '[Préfixe pour les FKs] L''''identifiant CONSTANTE.SCONSTANTE|52 du/de la---- indiquant----' +CHAR(13)+CHAR(10)+REPLICATE(CHAR(9),10)+REPLICATE(' ',2)+ '[Préfixe pour les AUTRES] Désigne quel---- : entreprise(1), personne(2) [exemple de la colonne ENTITE.STYPEENTITE]'';' +CHAR(13)+CHAR(10)+CHAR(9)+ 'DECLARE @nomTable NVARCHAR(100) = SUBSTRING(@columnID, 0, CHARINDEX(''.'', @columnID)), @nomColonne NVARCHAR(100) = SUBSTRING(@columnID, CHARINDEX(''.'', @columnID) + 1, LEN(@columnID)-CHARINDEX(''.'', @columnID));' +CHAR(13)+CHAR(10)+CHAR(9)+ 'DECLARE @requeteSQL NVARCHAR(max) = N''' +CHAR(13)+CHAR(10)+CHAR(9)+CHAR(9)+ 'IF EXISTS(SELECT * FROM sys.tables sysTable INNER JOIN sys.columns sysColumn ON sysTable.object_id = sysColumn.object_id LEFT JOIN sys.extended_properties prop ON sysTable.object_id = prop.major_id WHERE sysColumn.column_id = prop.minor_id AND prop.NAME = ''''MS_Description'''' AND prop.NAME IS NOT NULL AND sysTable.NAME = @nomTable AND sysColumn.name = @nomColonne)' +CHAR(13)+CHAR(10)+CHAR(9)+CHAR(9)+CHAR(9)+ 'EXEC sys.sp_updateextendedproperty @name=N''''MS_Description'''',@value=@columnDOC,@level0type=N''''SCHEMA'''',@level0name=N''''dbo'''',@level1type=N''''TABLE'''',@level1name=@nomTable,@level2type=N''''COLUMN'''',@level2name=@nomColonne' +CHAR(13)+CHAR(10)+CHAR(9)+CHAR(9)+ 'ELSE' +CHAR(13)+CHAR(10)+CHAR(9)+CHAR(9)+CHAR(9)+ 'EXEC sys.sp_addextendedproperty @name=N''''MS_Description'''',@value=@columnDOC,@level0type=N''''SCHEMA'''',@level0name=N''''dbo'''',@level1type=N''''TABLE'''',@level1name=@nomTable,@level2type=N''''COLUMN'''',@level2name=@nomColonne' +CHAR(13)+CHAR(10)+CHAR(9)+ ''';' +CHAR(13)+CHAR(10)+ +CHAR(13)+CHAR(10)+CHAR(9)+ 'EXECUTE sp_executesql @stmt=@requeteSQL, @params=N''@nomTable NVARCHAR(100), @nomColonne NVARCHAR(100), @columnDOC NVARCHAR(4000)'', @nomTable=@nomTable, @nomColonne=@nomColonne, @columnDOC=@columnDOC' +CHAR(13)+CHAR(10)+CHAR(9)+ ';WITH CataloguePKs AS( SELECT KU.TABLE_NAME, KU.COLUMN_NAME AS ColonnePK FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS TC with(nolock) INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KU with(nolock) ON TC.CONSTRAINT_NAME = KU.CONSTRAINT_NAME WHERE TC.CONSTRAINT_TYPE = ''PRIMARY KEY''), CatalogueDescriptions AS( SELECT sysTable.NAME AS TABLE_NAME ,sysColumn.NAME AS COLUMN_NAME ,CAST(sysProp.value AS VARCHAR(8000)) AS COLUMN_DESCRIPTION ,ColonnePK FROM sys.tables sysTable INNER JOIN sys.columns sysColumn ON sysTable.object_id = sysColumn.object_id LEFT JOIN sys.extended_properties sysProp ON sysTable.object_id = sysProp.major_id AND sysColumn.column_id = sysProp.minor_id AND sysProp.NAME = ''MS_Description'' LEFT OUTER JOIN CataloguePKs ON CataloguePKs.TABLE_NAME = sysTable.name AND CataloguePKs.ColonnePK = sysColumn.name), CatalogueDescriptionsAvecPKsMisesEnAvant AS ( SELECT TABLE_NAME,CASE WHEN ColonnePK IS NOT NULL AND COLUMN_NAME = @nomColonne THEN REPLACE('' >> |||| {{COLUMN_NAME}} |||| >>'', ''{{COLUMN_NAME}}'', COLUMN_NAME) WHEN ColonnePK IS NOT NULL AND COLUMN_NAME <> @nomColonne THEN REPLACE('' |||| {{COLUMN_NAME}} ||||'', ''{{COLUMN_NAME}}'', COLUMN_NAME) WHEN COLUMN_NAME = @nomColonne AND ColonnePK IS NULL THEN ''>> '' + COLUMN_NAME + '' >>'' ELSE COLUMN_NAME END as COLUMN_NAME, COLUMN_DESCRIPTION FROM CatalogueDescriptions)' +CHAR(13)+CHAR(10)+CHAR(9)+ 'SELECT * FROM CatalogueDescriptionsAvecPKsMisesEnAvant WHERE TABLE_NAME = @nomTable AND COLUMN_NAME NOT IN (''IDESACTIVE'', ''QUI'', ''QUAND'', ''QUI_CREA'', ''QUAND_CREA'', ''QUI_MODIF'', ''QUAND_MODIF'', ''SAGENCE'') ORDER BY TABLE_NAME, COLUMN_NAME' +CHAR(13)+CHAR(10)+ 'END' +CHAR(13)+CHAR(10)+ 'GO' +CHAR(13)+CHAR(10)+CHAR(13)+CHAR(10), '{{TABLE_NAME}}', TABLE_NAME), '{{COLUMN_NAME_RAW}}', REPLACE(REPLACE(COLUMN_NAME, '|||| ', ''), ' ||||', '')) as SCRIPT_MISE_A_JOUR_DOCUMENTATION FROM ColonneEnBase)
	SELECT TABLE_NAME, COLUMN_NAME, COLUMN_TYPE, IS_NULLABLE, COLUMN_DESCRIPTION, SCRIPT_MISE_A_JOUR_DOCUMENTATION
	FROM DocumentationColonne
	WHERE TABLE_NAME LIKE @Table
	  AND (COLUMN_NAME LIKE @Colonne OR COLUMN_NAME LIKE '|||| '+@Colonne+' ||||') AND COLUMN_NAME NOT IN ('top_sup', 'id_cre', 'dt_cre', 'id_mod', 'dt_mod', 'lastupdate')
	  AND IS_NULLABLE IN ('NO', 'YES')
	ORDER BY TABLE_NAME, ORDINAL_POSITION_WITH_PK_FIRST
";

	const string BrowseRoutesViewsAndTriggers = @"
DECLARE @Nom VARCHAR(max) =/*LIKE*/ '{0}' , @ScriptDeCreation VARCHAR(max) =/*LIKE*/ '{1}';
WITH WithoutParams AS (SELECT o.[object_id] as ID, REPLACE(o.type_desc, 'SQL_', '') as [TYPE], o.name as [NOM], m.[definition] as [DEFINITION], type_desc, name FROM  sys.objects o INNER JOIN sys.sql_modules m ON m.[object_id] = o.[object_id] WHERE (o.name LIKE @Nom AND m.[definition] LIKE @ScriptDeCreation) AND o.[type] IN ('P', 'IF', 'FN', 'TF', 'TR', 'V'))
SELECT [TYPE], [NOM], (SELECT STUFF((SELECT ', ' + CASE WHEN (ISNULL(p.name, '') = '' OR p.is_output = 1) THEN type_name(p.user_type_id) + CASE WHEN CHARINDEX('var', type_name(p.user_type_id))>0 THEN '[' + CASE WHEN p.max_length = -1 THEN 'max' ELSE CAST(p.max_length as VARCHAR(8000)) END + ']' ELSE '' END + ' ' + ISNULL(p.name, '') + ' OUTPUT' ELSE type_name(p.user_type_id) + CASE WHEN CHARINDEX('var', type_name(p.user_type_id))>0 THEN '[' + CASE WHEN p.max_length = -1 THEN 'max' ELSE CAST(p.max_length as VARCHAR(8000)) END + ']' ELSE '' END+ ' ' + p.name END FROM sys.parameters p WHERE p.[object_id] = WithoutParams.[ID] ORDER BY p.parameter_id FOR XML PATH('')), 1, 2, '')) as PARAMS,	[DEFINITION] FROM WithoutParams ORDER BY type_desc, name
";

	const string BrowseTablesReferencingTargetTable = @"
SELECT fkTable as [Table], fkCol as [Column] FROM (SELECT f.name AS ForeignKey, '|===' AS Sep1, OBJECT_NAME(f.parent_object_id) AS fkTable, COL_NAME(fc.parent_object_id, fc.parent_column_id) AS fkCol, '===>' AS Sep2, OBJECT_NAME (f.referenced_object_id) AS pkTable, COL_NAME(fc.referenced_object_id, fc.referenced_column_id) AS pkCol FROM sys.foreign_keys AS f INNER JOIN sys.foreign_key_columns AS fc ON f.OBJECT_ID = fc.constraint_object_id) as PK_FK_TABLE
WHERE pkTable IN ('{0}')
";

	const string BrowseTablesReferencedByTargetTable = @"
SELECT fkTable as [Table], fkCol as [Column] FROM (SELECT f.name AS ForeignKey, '|===' AS Sep1, OBJECT_NAME(f.parent_object_id) AS fkTable, COL_NAME(fc.parent_object_id, fc.parent_column_id) AS fkCol, '===>' AS Sep2, OBJECT_NAME (f.referenced_object_id) AS pkTable, COL_NAME(fc.referenced_object_id, fc.referenced_column_id) AS pkCol FROM sys.foreign_keys AS f INNER JOIN sys.foreign_key_columns AS fc ON f.OBJECT_ID = fc.constraint_object_id) as PK_FK_TABLE
WHERE fkTable IN ('{0}')
";
}
