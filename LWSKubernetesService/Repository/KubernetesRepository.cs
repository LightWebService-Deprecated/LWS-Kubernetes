using k8s;
using k8s.Models;

namespace LWSKubernetesService.Repository;

public interface IKubernetesRepository
{
    Task CreateNameSpaceAsync(string userId);
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

        await _kubernetesClient.CreateNamespaceWithHttpMessagesAsync(namespaceBody);
    }
}