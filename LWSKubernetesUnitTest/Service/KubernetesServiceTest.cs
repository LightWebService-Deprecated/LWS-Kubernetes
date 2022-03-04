using System;
using System.Threading.Tasks;
using LWSKubernetesService.Model.Event;
using LWSKubernetesService.Repository;
using LWSKubernetesService.Service;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace LWSKubernetesUnitTest.Service;

public class KubernetesServiceTest
{
    private readonly Mock<IKubernetesRepository> _kubernetesRepository;
    private KubernetesService TestKubernetesService => new KubernetesService(_kubernetesRepository.Object);

    public KubernetesServiceTest()
    {
        _kubernetesRepository = new Mock<IKubernetesRepository>();
    }

    [Fact(DisplayName =
        "HandleAccountCreationAsync: HandleAccountCreationAsync should deserialize object and create namespace well.")]
    public async Task Is_HandleAccountCreationAsync_Creates_Namespace_Well()
    {
        // Let
        var message = new AccountCreatedMessage
        {
            AccountId = "testId",
            CreatedAt = DateTimeOffset.Now
        };
        _kubernetesRepository.Setup(a => a.CreateNameSpaceAsync(message.AccountId))
            .Callback((string accountId) => { Assert.Equal(message.AccountId, accountId); });

        // Do
        await TestKubernetesService.HandleAccountCreationAsync(JsonConvert.SerializeObject(message));

        // Verify
        _kubernetesRepository.VerifyAll();
    }
}