using LWSKubernetesService.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace LWSKubernetesService.Repository;

public class MongoContext
{
    public readonly IMongoClient MongoClient;
    public readonly IMongoDatabase MongoDatabase;
    public readonly MongoConfiguration MongoConfiguration;

    public IMongoCollection<BsonDocument> DeploymentBsonCollection =>
        MongoDatabase.GetCollection<BsonDocument>(MongoConfiguration.DeploymentContainerName);


    public MongoContext(MongoConfiguration mongoConfiguration)
    {
        // Setup MongoDB Naming Convention
        var camelCase = new ConventionPack {new CamelCaseElementNameConvention()};
        ConventionRegistry.Register("CamelCase", camelCase, a => true);

        MongoConfiguration = mongoConfiguration;
        MongoClient = new MongoClient(mongoConfiguration.ConnectionString);
        MongoDatabase = MongoClient.GetDatabase(mongoConfiguration.DatabaseName);

        // Create Indexes
        CreateDeploymentIndexAsync().Wait();
    }

    private async Task CreateDeploymentIndexAsync()
    {
        // User Email Index
        var userEmailKey = Builders<BsonDocument>.IndexKeys.Ascending("accountId");
        var userEmailIndexModel = new CreateIndexModel<BsonDocument>(userEmailKey);
        await DeploymentBsonCollection.Indexes.CreateOneAsync(userEmailIndexModel);
    }
}