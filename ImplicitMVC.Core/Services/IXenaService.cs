using System.Threading.Tasks;

namespace ImplicitMVC.Core.Services
{
    public interface IXenaService
    {
        Task<string> GetUserFiscalSetupAsync();
        Task<string> GetUserMembershipAsync();
        Task<string> GetFiscalXenaAppsAsync(string fiscalId);
    }
}
