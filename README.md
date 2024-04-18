# What is `db`?
`db` is a single command line interface to query any database of any technology, with custom-tailored syntax.

## Supported database technologies
* SQL Server
* CosmosDB

```
┌-----------------------------------------------------------------------------------------------------------------------------------------------------┐
|            | db list                                                              | List queryable database names                                   |
├-----------------------------------------------------------------------------------------------------------------------------------------------------┤
| CosmosDB   | db {database} dev/qa/pre/prod                                        | Find one document                                               |
| CosmosDB   | db {database} dev/qa/pre/prod {id}                                   | Find at most 10 documents with 'c.id = "{id}"'                  |
├-----------------------------------------------------------------------------------------------------------------------------------------------------┤
| SqlServer  | db {database} dev/qa/pre/prod {table_name}                           | Find top 20 rows of {table_name}                                |
| SqlServer  | db {database} dev/qa/pre/prod {sql_query}                            | Run {sql_query}                                                 |
| SqlServer  | db {database} dev/qa/pre/prod table|tb|t {pattern}                   | Find table names LIKE '%{pattern}%'                             |
| SqlServer  | db {database} dev/qa/pre/prod column|col|c {pattern}                 | Find column names LIKE '%{pattern}%'                            |
| SqlServer  | db {database} dev/qa/pre/prod TABLE|TB|T {pattern}                   | Find tables names LIKE '{pattern}'                              |
| SqlServer  | db {database} dev/qa/pre/prod COLUMN|COL|C {pattern}                 | Find column names LIKE '{pattern}'                              |
| SqlServer  | db {database} dev/qa/pre/prod routine|rt|r {pattern}                 | Find routines/views/triggers LIKE '%{pattern}%' (name or body)  |
| SqlServer  | db {database} dev/qa/pre/prod ROUTINE|RT|R {pattern}                 | Find routines/views/triggers LIKE '{pattern}' (name or body)    |
| SqlServer  | db {database} dev/qa/pre/prod referencing|ref {table_name_pattern}   | Find tables referencing '%{table_name_pattern}%'                |
| SqlServer  | db {database} dev/qa/pre/prod referenced|refed {table_name_pattern}  | Find tables referenced by '{table_name_pattern}'                |
└-----------------------------------------------------------------------------------------------------------------------------------------------------┘
```
