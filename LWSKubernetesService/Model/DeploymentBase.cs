namespace LWSKubernetesService.Model;

public class DeploymentBase
{
    public string Id { get; set; }
    public DeploymentType DeploymentType { get; set; }
    public string AccountId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}