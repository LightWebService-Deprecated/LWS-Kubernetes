namespace LWSKubernetesService.Model.Event;

public class AccountCreatedMessage
{
    public string AccountId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}