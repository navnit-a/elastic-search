using Bogus;
using Elasticsearch.Net;
using Nest;

namespace ElasticSearch;

internal class Program
{
    private static async Task Main(string[] args)
    {
        
        var connectionPool = new SingleNodeConnectionPool(new Uri("https://my-elastic-deployment.es.europe-west2.gcp.elastic-cloud.com"));
        var settings = new ConnectionSettings(connectionPool)
            .DefaultIndex("people")
            .BasicAuthentication("elastic", "xxx") // Add your Elastic Cloud password here
            .ServerCertificateValidationCallback((o, c, ch, e) => true);
        ;


        var client = new ElasticClient(settings);

        var faker = new Faker<Person>()
            .RuleFor(p => p.Id, f => f.Random.Int())
            .RuleFor(p => p.FirstName, f => f.Name.FirstName())
            .RuleFor(p => p.LastName, f => f.Name.LastName());

        var people = faker.Generate(100);

        foreach (var person in people)
        {
            var indexResponse = await client.IndexDocumentAsync(person);
            Console.WriteLine(indexResponse.Result.ToString());
        }

        Console.WriteLine("Indexing complete!");

        var searchResponse = client.Search<Person>(s => s
            .From(0)
            .Size(10)
        );

        var peoplCollection = searchResponse.Documents;
    }
}