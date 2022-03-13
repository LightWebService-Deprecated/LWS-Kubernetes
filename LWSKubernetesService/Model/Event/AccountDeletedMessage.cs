namespace LWSKubernetesService.Model.Event;

public class AccountDeletedMessage
{
    public string AccountId { get; set; }
    public DateTimeOffset DeletedAt { get; set; }
}