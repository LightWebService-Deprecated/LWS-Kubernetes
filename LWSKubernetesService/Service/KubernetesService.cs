using System.Text.Json;
using LWSEvent.Event.Account;
using LWSEvent.Event.Deployment;
using LWSKubernetesService.Repository;
using Newtonsoft.Json;

namespace LWSKubernetesService.Service;

public class KubernetesService
{
    private readonly IKubernetesRepository _kubernetesRepository;
    private readonly IDeploymentRepository _deploymentRepository;

    public KubernetesService(IKubernetesRepository kubernetesRepository, IDeploymentRepository deploymentRepository)
    {
        _kubernetesRepository = kubernetesRepository;
        _deploymentRepository = deploymentRepository;
    }

    public async Task HandleAccountCreationAsync(AccountCreatedEvent accountCreated)
    {
        await _kubernetesRepository.CreateNameSpaceAsync(accountCreated.AccountId.ToLower());
    }

    public async Task HandleAccountDeletionAsync(AccountDeletedEvent accountDeletedEvent)
    {
        await _kubernetesRepository.DeleteNameSpaceAsync(accountDeletedEvent.AccountId.ToLower());
    }

    public async Task HandleDeploymentCreatedAsync(DeploymentCreatedEvent deploymentCreatedMessage)
    {
        await _deploymentRepository.CreateDeploymentAsync(deploymentCreatedMessage.DeploymentObject.ToDictionary(
            key => key.Key.ToLower() == "id" ? "_id" : JsonNamingPolicy.CamelCase.ConvertName(key.Key),
            value => value.Value));
    }
}