using System.Threading.Tasks;

namespace HybricMVC.NetFramework.Services
{
    public interface IXenaService
    {
        Task<string> GetUserFiscalSetupAsync();
        Task<string> GetUserMembershipAsync();
        Task<string> GetFiscalXenaAppsAsync(string fiscalId);
    }
}
