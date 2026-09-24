using Catalog.Application.Interfaces;
using Catalog.Application.Search;
using Catalog.Domain.Entities;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Mapping;

namespace Catalog.Infrastructure.Search;

public class ElasticsearchProductSearchService
    : IProductSearchService
{
    private const string IndexName = "products";

    private readonly ElasticsearchClient _client;

    public ElasticsearchProductSearchService(
        ElasticsearchClient client)
    {
        _client = client;
    }

    public async Task CreateIndexAsync()
    {
        var existsResponse =
            await _client.Indices.ExistsAsync(IndexName);

        if (existsResponse.Exists)
        {
            return;
        }

        var response = await _client.Indices.CreateAsync(
            IndexName,
            index => index
                .Mappings<ProductSearchDocument>(
                    mapping => mapping
                        .Properties(properties => properties
                            .Keyword(x => x.Id)
                            .Text(x => x.Name)
                            .Text(x => x.Description)
                            .Keyword(x => x.Category)
                            .Keyword(x => x.Brand)
                            .DoubleNumber(x => x.Price)
                            .IntegerNumber(x => x.Stock)
                        )
                )
        );

        if (!response.IsValidResponse)
        {
            throw new InvalidOperationException(
                $"Failed to create Elasticsearch index: {response.DebugInformation}");
        }
    }

    public async Task IndexAsync(Product product)
    {
        var document = new ProductSearchDocument
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Category = product.Category,
            Brand = product.Brand,
            Price = product.Price,
            Stock = product.Stock
        };

        var response = await _client.IndexAsync(
            document,
            index => index
                .Index(IndexName)
                .Id(product.Id)
        );

        if (!response.IsValidResponse)
        {
            throw new InvalidOperationException(
                $"Failed to index product: {response.DebugInformation}");
        }
    }

    public async Task<IReadOnlyList<ProductSearchDocument>> SearchAsync(string query)
    {
        var response =
            await _client.SearchAsync<ProductSearchDocument>(
                search => search
                    .Indices(IndexName)
                    .Query(q => q
                        .MultiMatch(m => m
                            .Query(query)
                            .Fields(new[]
                            {
                                "name",
                                "description"
                            })
                        )
                    )
            );

        if (!response.IsValidResponse)
        {
            throw new InvalidOperationException(
                $"Failed to search products: {response.DebugInformation}");
        }

        return response.Documents.ToList();
    }
}
