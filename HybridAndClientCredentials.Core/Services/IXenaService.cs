using System.Threading.Tasks;

namespace HybridAndClientCredentials.Core.Services
{
    public interface IXenaService
    {
        Task<string> GetUserFiscalSetupAsync();
        Task<string> GetUserMembershipAsync();
        Task<string> GetFiscalXenaAppsAsync(string fiscalId);
    }
}
