using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace LWSKubernetesService.Repository;

public interface IDeploymentRepository
{
    Task CreateDeploymentAsync(object deployment);
}

public class DeploymentRepository : IDeploymentRepository
{
    private readonly ILogger _logger;
    private readonly IMongoCollection<BsonDocument> _deploymentCollection;

    public DeploymentRepository(MongoContext mongoContext, ILogger<IDeploymentRepository> logger)
    {
        _deploymentCollection = mongoContext.DeploymentBsonCollection;
        _logger = logger;
    }

    public async Task CreateDeploymentAsync(object deployment)
    {
        var jsonStr = JsonConvert.SerializeObject(deployment);
        var bsonDocument = BsonSerializer.Deserialize<BsonDocument>(jsonStr);
        _logger.LogInformation("Creating Deployment Preference for {bsonDocument}", bsonDocument.ToString());
        await _deploymentCollection.InsertOneAsync(bsonDocument);
    }
}