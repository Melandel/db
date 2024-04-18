using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Db.Infrastructure;

public class CosmosDatabaseClient : Db.Domain.DatabaseClient
{
	public string EndpointUrl { get; }
	public string AuthorizationKey { get; }
	public string DatabaseId { get; }
	public string ContainerId { get; }


	CosmosDatabaseClient(string endpointUrl, string authorizationKey, string databaseId, string containerId)
	{
		EndpointUrl = endpointUrl;
		AuthorizationKey = authorizationKey;
		DatabaseId = databaseId;
		ContainerId = containerId;
	}

	public static CosmosDatabaseClient Create(string endpointUrl, string authorizationKey, string databaseId, string containerId)
		=> new CosmosDatabaseClient(endpointUrl, authorizationKey, databaseId, containerId);

	public override async Task<Domain.QueryExecutionReport> Query(Domain.DatabaseQuery query)
	{
		var list = new List<object>();
		using var feedIterator = new CosmosClient(EndpointUrl, AuthorizationKey)
			.GetContainer(DatabaseId, ContainerId)
			.GetItemQueryIterator<object>(BuildLiteralQuery(query));

		var sw = new Stopwatch();
		sw.Start();
		while (feedIterator.HasMoreResults)
		{
			FeedResponse<object> currentResultSet = await feedIterator.ReadNextAsync();
			foreach (var obj in currentResultSet)
			{
				list.Add(obj);
			}
		}
		sw.Stop();

		return new(
			sw.ElapsedMilliseconds,
			list.Count,
			list.ToArray());
	}

	string BuildLiteralQuery(Domain.DatabaseQuery query)
	=> query.Type switch
	{
		Domain.DatabaseQueryType.TechnicalDefaultEnumValue => throw new Exception("DatabaseQueryType.TechnicalDefaultEnumValue"),
		Domain.DatabaseQueryType.Custom => BuildLiteralQueryFromQueryElements(query.Elements),
		_ => throw new Exception("DatabaseQueryType")
	};

	string BuildLiteralQueryFromQueryElements(string[] queryElements)
	=> queryElements.Length switch
	{
		0 => "SELECT * FROM c ORDER BY c._ts DESC OFFSET 0 LIMIT 1",
		1 => queryElements[0].Split(" ").Length switch
		{
			1 => $"SELECT * FROM c WHERE c.id = \"{queryElements[0]}\"",
			_ => $"{queryElements[0]} OFFSET 0 LIMIT 10"
		},
		_ => throw new Exception($"Expected one string argument instead of {queryElements.Length}")
	};
}
