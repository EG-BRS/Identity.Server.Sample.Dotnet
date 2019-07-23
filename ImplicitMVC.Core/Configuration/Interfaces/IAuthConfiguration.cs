namespace ImplicitMVC.Core.Configuration.Interfaces
{
    public interface IAuthConfiguration
    {
        string Authority { get; }
        string ClientId { get; }
        string ClientSecret { get; }
    }
}
