using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LWSEvent.Event.Account;
using LWSEvent.Event.Deployment;
using LWSKubernetesService.Repository;
using LWSKubernetesService.Service;
using Moq;
using Newtonsoft.Json;
using Xunit;
using DeploymentType = LWSKubernetesService.Model.DeploymentType;

namespace LWSKubernetesUnitTest.Service;

public class KubernetesServiceTest
{
    private readonly Mock<IKubernetesRepository> _kubernetesRepository;
    private readonly Mock<IDeploymentRepository> _deploymentRepository;

    private KubernetesService TestKubernetesService =>
        new KubernetesService(_kubernetesRepository.Object, _deploymentRepository.Object);

    public KubernetesServiceTest()
    {
        _kubernetesRepository = new Mock<IKubernetesRepository>();
        _deploymentRepository = new Mock<IDeploymentRepository>();
    }

    [Fact(DisplayName =
        "HandleAccountCreationAsync: HandleAccountCreationAsync should deserialize object and create namespace well.")]
    public async Task Is_HandleAccountCreationAsync_Creates_Namespace_Well()
    {
        // Let
        var message = new AccountCreatedEvent
        {
            AccountId = "testId",
            CreatedAt = DateTimeOffset.Now
        };
        _kubernetesRepository.Setup(a => a.CreateNameSpaceAsync(message.AccountId.ToLower()));

        // Do
        await TestKubernetesService.HandleAccountCreationAsync(message);

        // Verify
        _kubernetesRepository.VerifyAll();
    }

    [Fact(DisplayName =
        "HandleAccountDeletionAsync: HandleAccountDeletionAsync should deserialize object and remove namespace well.")]
    public async Task Is_HandleAccountDeletionAsync_Deletes_Namespace_Well()
    {
        // Let
        var message = new AccountDeletedEvent
        {
            AccountId = "test",
            DeletedAt = DateTimeOffset.UtcNow
        };
        _kubernetesRepository.Setup(a => a.DeleteNameSpaceAsync(message.AccountId));

        // Do
        await TestKubernetesService.HandleAccountDeletionAsync(message);

        // Verify
        _kubernetesRepository.VerifyAll();
    }

    [Fact(DisplayName =
        "HandleDeploymentCreatedAsync: HandleDeploymentCreatedAsync should create deployment data to database.")]
    public async Task Is_HandleDeploymentCreatedAsync_Creates_Deployment_To_Database()
    {
        // Let
        var deploymentObject = new Dictionary<string, object>
        {
            ["Id"] = "test",
            ["AccountId"] = "test2"
        };

        var message = new DeploymentCreatedEvent
        {
            DeploymentType = LWSEvent.Event.Deployment.DeploymentType.UbuntuDeployment,
            AccountId = "kangdroid",
            CreatedAt = DateTimeOffset.UtcNow,
            DeploymentObject = deploymentObject
        };
        _deploymentRepository.Setup(a => a.CreateDeploymentAsync(It.IsAny<object>()))
            .Callback((object targetInput) =>
            {
                Assert.True(targetInput is Dictionary<string, object>);

                var dictionaryInput = (Dictionary<string, object>) targetInput;
                Assert.Equal(deploymentObject.Count, dictionaryInput.Count);
                Assert.True(dictionaryInput.ContainsKey("_id"));
                Assert.Equal("test", (string) dictionaryInput["_id"]);
                Assert.Equal("test2", (string) dictionaryInput["accountId"]);
            });

        // Do
        await TestKubernetesService.HandleDeploymentCreatedAsync(message);

        // Verify
        _deploymentRepository.VerifyAll();
    }
}