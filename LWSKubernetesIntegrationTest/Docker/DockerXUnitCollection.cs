using Xunit;

namespace LWSKubernetesIntegrationTest.Docker;

[CollectionDefinition("DockerIntegration")]
public class DockerXUnitCollection : ICollectionFixture<DockerRunner>
{
}