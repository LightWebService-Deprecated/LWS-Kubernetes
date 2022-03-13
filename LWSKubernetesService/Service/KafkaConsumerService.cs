using System.Diagnostics.CodeAnalysis;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace LWSKubernetesService.Service;

[ExcludeFromCodeCoverage]
public class KafkaConsumerService : BackgroundService
{
    private readonly KubernetesService _kubernetesService;
    private readonly ILogger _logger;

    private readonly ConsumerConfig _consumerConfig;

    public KafkaConsumerService(KubernetesService kubernetesService, IConfiguration configuration,
        ILogger<KafkaConsumerService> logger)
    {
        _kubernetesService = kubernetesService;
        _logger = logger;

        _consumerConfig = configuration.GetSection("KafkaConsumerConfig").Get<ConsumerConfig>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();
        using var consumerBuilder = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build();
        consumerBuilder.Subscribe(new[] {"account.created", "account.deleted"});

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var consumedMessage = consumerBuilder.Consume(stoppingToken);
                _logger.LogInformation("Received Message: {message}", JsonConvert.SerializeObject(consumedMessage));
                try
                {
                    switch (consumedMessage.Topic)
                    {
                        case "account.created":
                        {
                            await _kubernetesService.HandleAccountCreationAsync(consumedMessage.Message.Value);
                            break;
                        }
                        case "account.deleted":
                        {
                            await _kubernetesService.HandleAccountDeletionAsync(consumedMessage.Message.Value);
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError("Exception occurred while processing message: {message}", e.Message);
                    _logger.LogError("{stackTrace}", e.StackTrace);
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogCritical("Exception Occurred while receiving message");
            _logger.LogCritical("{Message}", e.Message);
            _logger.LogCritical("{stackTrace}", e.StackTrace);
            consumerBuilder.Close();
        }
    }
}