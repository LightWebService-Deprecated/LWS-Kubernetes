using LWSEvent.Event.Account;
using MassTransit;
using Newtonsoft.Json;

namespace LWSKubernetesService.Service;

public class AccountCreatedConsumer: IConsumer<AccountCreatedEvent>
{
    private readonly KubernetesService _kubernetesService;
    private readonly ILogger _logger;
    
    public AccountCreatedConsumer(KubernetesService kubernetesService, ILogger<AccountCreatedConsumer> logger)
    {
        _kubernetesService = kubernetesService;
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<AccountCreatedEvent> context)
    {
        _logger.LogInformation("Consumed Message {messageStr} From {topic}", JsonConvert.SerializeObject(context.Message), JsonConvert.SerializeObject(context.Headers));
        await _kubernetesService.HandleAccountCreationAsync(context.Message);
    }
}