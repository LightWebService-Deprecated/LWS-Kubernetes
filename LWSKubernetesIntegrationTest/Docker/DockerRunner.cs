using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Docker.DotNet;
using LWSKubernetesIntegrationTest.Docker.ContainerDefinition;

namespace LWSKubernetesIntegrationTest.Docker;

public enum ContainerType
{
    K3S,
    MongoDb
}

public class DockerRunner : IDisposable
{
    private readonly DockerClient _dockerClient;
    public readonly Dictionary<ContainerType, DockerImageBase> DockerImageDictionary;

    public DockerRunner()
    {
        _dockerClient = new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock"))
            .CreateClient();

        DockerImageDictionary = new Dictionary<ContainerType, DockerImageBase>
        {
            [ContainerType.K3S] = new K3SContainer(_dockerClient),
            [ContainerType.MongoDb] = new MongoDbContainer(_dockerClient)
        };

        CreateAllContainer().Wait();
    }

    private async Task CreateAllContainer()
    {
        foreach (var (key, eachContainer) in DockerImageDictionary)
        {
            await eachContainer.CreateContainerAsync();
            await eachContainer.RunContainerAsync();
        }
    }

    private async Task RemoveAllContainer()
    {
        foreach (var (key, eachContainer) in DockerImageDictionary)
        {
            await eachContainer.RemoveContainerAsync();
        }
    }

    public void Dispose()
    {
        RemoveAllContainer().Wait();
        _dockerClient.Dispose();
    }
}