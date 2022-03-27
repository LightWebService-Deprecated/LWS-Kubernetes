using LWSEvent.Event.Deployment;
using MassTransit;
using Newtonsoft.Json;

namespace LWSKubernetesService.Service;

public class DeploymentCreatedConsumer: IConsumer<DeploymentCreatedEvent>
{
    private readonly ILogger _logger;
    private readonly KubernetesService _kubernetesService;

    public DeploymentCreatedConsumer(KubernetesService kubernetesService, ILogger<DeploymentCreatedConsumer> logger)
    {
        _logger = logger;
        _kubernetesService = kubernetesService;
    }
    
    public async Task Consume(ConsumeContext<DeploymentCreatedEvent> context)
    {
        _logger.LogInformation("Consumed Message {messageStr} From {topic}", JsonConvert.SerializeObject(context.Message), JsonConvert.SerializeObject(context.Headers));
        await _kubernetesService.HandleDeploymentCreatedAsync(context.Message);
    }
}