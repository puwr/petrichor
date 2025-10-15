namespace Petrichor.Services.Comments.Tests.TestUtilities;

[CollectionDefinition(Name)]
public class ApiFactoryCollection : ICollectionFixture<ApiFactory>
{
    public const string Name = "ApiFactoryCollection";
}