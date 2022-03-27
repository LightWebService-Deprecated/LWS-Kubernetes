using LWSEvent.Event.Account;
using MassTransit;
using Newtonsoft.Json;

namespace LWSKubernetesService.Service;

public class AccountDeletedConsumer: IConsumer<AccountDeletedEvent>
{
    private readonly KubernetesService _kubernetesService;
    private readonly ILogger _logger;

    public AccountDeletedConsumer(KubernetesService kubernetesService, ILogger<AccountDeletedConsumer> logger)
    {
        _kubernetesService = kubernetesService;
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<AccountDeletedEvent> context)
    {
        _logger.LogInformation("Consumed Message {messageStr} From {topic}", JsonConvert.SerializeObject(context.Message), JsonConvert.SerializeObject(context.Headers));
        await _kubernetesService.HandleAccountDeletionAsync(context.Message);
    }
}