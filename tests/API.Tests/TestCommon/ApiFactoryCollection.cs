namespace API.Tests.TestCommon;

[CollectionDefinition(Name)]
public class ApiFactoryCollection : ICollectionFixture<ApiFactory>
{
    public const string Name = "ApiFactoryCollection";
}