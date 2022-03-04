using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using LWSKubernetesIntegrationTest.Docker;
using LWSKubernetesService.Repository;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace LWSKubernetesIntegrationTest.Repository;

[Collection("DockerIntegration")]
public class KubernetesRepositoryTest : IDisposable
{
    private readonly IKubernetesRepository _kubernetesRepository;
    private readonly IKubernetes _testClient;

    public KubernetesRepositoryTest(DockerRunner dockerRunner)
    {
        var kubeConfig = dockerRunner.DockerImageDictionary[ContainerType.K3S].Connections;
        var mockConfiguration = new Mock<IConfiguration>();
        mockConfiguration.Setup(a => a["KubePath"])
            .Returns(kubeConfig);

        _kubernetesRepository = new KubernetesRepository(mockConfiguration.Object);
        _testClient = new Kubernetes(KubernetesClientConfiguration.BuildConfigFromConfigFile(kubeConfig));
    }

    public void Dispose()
    {
        ResetKube().Wait();
    }

    private async Task ResetKube()
    {
        var namespaceBlackList = new List<string>
        {
            "default",
            "kube-system",
            "kube-public",
            "kube-node-lease"
        };

        var namespaceResponse = await _testClient.ListNamespaceWithHttpMessagesAsync();
        var namespaceToRemove = namespaceResponse.Body.Items
            .Where(a => !namespaceBlackList.Contains(a.Name()));
        foreach (var eachNamespace in namespaceToRemove)
        {
            await _testClient.DeleteNamespaceWithHttpMessagesAsync(eachNamespace.Name());
        }
    }

    private async Task EnsureNamespaceCreated(string namespaceName)
    {
        var namespaceResponse = await _testClient.ListNamespaceWithHttpMessagesAsync();
        var namespaceList = namespaceResponse.Body.Items.Select(a => a.Name());

        Assert.Contains(namespaceName, namespaceList);
    }

    private async Task EnsureNamespaceDeleted(string namespaceName)
    {
        var namespaceResponse = await _testClient.ListNamespaceWithHttpMessagesAsync();
        var namespaceList = namespaceResponse.Body.Items
            .Where(a => a.Status.Phase == "Active")
            .Select(a => a.Name());

        Assert.DoesNotContain(namespaceName, namespaceList);
    }

    private async Task EnsureDeploymentDeleted(string deploymentName)
    {
        var deploymentResponse = await _testClient.ListDeploymentForAllNamespacesAsync();
        Assert.DoesNotContain(deploymentName, deploymentResponse.Items.Select(a => a.Name()));
    }

    private async Task EnsureDeploymentCreated(string deploymentName)
    {
        var deploymentResponse = await _testClient.ListDeploymentForAllNamespacesAsync();
        Assert.Contains(deploymentName, deploymentResponse.Items.Select(a => a.Name()));
    }

    private string GenerateRandomToken(int length = 64)
    {
        var random = new Random();
        var charDictionary = "1234567890abcdefghijklmnopqrstuvwxyz";

        return new string(Enumerable.Repeat(charDictionary, length)
            .Select(a => a[random.Next(charDictionary.Length)]).ToArray());
    }

    [Theory(DisplayName = "CreateNameSpace: CreateNameSpace should create namespace with given name.")]
    [InlineData("testid")]
    [InlineData("testtwo")]
    [InlineData("somestrangeone")]
    public async void Is_CreateNameSpace_Creates_Namespace_Given_Name(string namespaceName)
    {
        // Do
        await _kubernetesRepository.CreateNameSpaceAsync(namespaceName);

        // Ensure
        await EnsureNamespaceCreated(namespaceName);
    }
}