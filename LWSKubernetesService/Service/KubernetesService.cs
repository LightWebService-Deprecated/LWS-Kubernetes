using LWSKubernetesService.Model.Event;
using LWSKubernetesService.Repository;
using Newtonsoft.Json;

namespace LWSKubernetesService.Service;

public class KubernetesService
{
    private readonly IKubernetesRepository _kubernetesRepository;

    public KubernetesService(IKubernetesRepository kubernetesRepository)
    {
        _kubernetesRepository = kubernetesRepository;
    }

    public async Task HandleAccountCreationAsync(string accountCreatedMessage)
    {
        var accountCreated = JsonConvert.DeserializeObject<AccountCreatedMessage>(accountCreatedMessage);
        await _kubernetesRepository.CreateNameSpaceAsync(accountCreated.AccountId);
    }
}