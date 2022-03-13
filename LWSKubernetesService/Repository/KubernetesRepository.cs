using k8s;
using k8s.Models;

namespace LWSKubernetesService.Repository;

public interface IKubernetesRepository
{
    Task CreateNameSpaceAsync(string userId);
    Task DeleteNameSpaceAsync(string userId);
}

public class KubernetesRepository : IKubernetesRepository
{
    private readonly IKubernetes _kubernetesClient;

    public KubernetesRepository(IConfiguration configuration)
    {
        var config = KubernetesClientConfiguration.BuildConfigFromConfigFile(configuration["KubePath"]);
        _kubernetesClient = new Kubernetes(config);
    }

    public async Task CreateNameSpaceAsync(string userId)
    {
        var namespaceBody = new V1Namespace
        {
            Metadata = new V1ObjectMeta
            {
                Name = userId,
            }
        };

        var response = await _kubernetesClient.CreateNamespaceWithHttpMessagesAsync(namespaceBody);
        var test = 1;
    }

    public async Task DeleteNameSpaceAsync(string userId)
    {
        await _kubernetesClient.DeleteNamespaceWithHttpMessagesAsync(userId);
    }
}