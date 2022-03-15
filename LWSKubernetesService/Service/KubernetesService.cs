using System.Text.Json;
using LWSKubernetesService.Model.Event;
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

    public async Task HandleAccountCreationAsync(string accountCreatedMessage)
    {
        var accountCreated = JsonConvert.DeserializeObject<AccountCreatedMessage>(accountCreatedMessage);
        await _kubernetesRepository.CreateNameSpaceAsync(accountCreated.AccountId.ToLower());
    }

    public async Task HandleAccountDeletionAsync(string accountDeletedMessage)
    {
        var accountDeleted = JsonConvert.DeserializeObject<AccountDeletedMessage>(accountDeletedMessage);
        await _kubernetesRepository.DeleteNameSpaceAsync(accountDeleted.AccountId.ToLower());
    }

    public async Task HandleDeploymentCreatedAsync(string deploymentCreatedMessage)
    {
        var deploymentCreated = JsonConvert.DeserializeObject<DeploymentCreatedMessage>(deploymentCreatedMessage);
        await _deploymentRepository.CreateDeploymentAsync(deploymentCreated.DeploymentObject.ToDictionary(
            key => key.Key.ToLower() == "id" ? "_id" : JsonNamingPolicy.CamelCase.ConvertName(key.Key),
            value => value.Value));
    }
}