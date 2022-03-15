namespace LWSKubernetesService.Configuration;

public class MongoConfiguration
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public string DeploymentContainerName { get; set; }
}