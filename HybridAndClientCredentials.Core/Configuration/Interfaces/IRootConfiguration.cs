namespace HybridAndClientCredentials.Core.Configuration.Interfaces
{
    public interface IRootConfiguration
    {
        IAuthConfiguration AuthConfiguration { get; }
        IApiEndpoints ApiEndpoints { get; }
    }
}
