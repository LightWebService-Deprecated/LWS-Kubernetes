namespace LWSKubernetesService.Model.Event;

public class DeploymentCreatedMessage
{
    public DeploymentType DeploymentType { get; set; }
    public string AccountId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Dictionary<string, object> DeploymentObject { get; set; }
}