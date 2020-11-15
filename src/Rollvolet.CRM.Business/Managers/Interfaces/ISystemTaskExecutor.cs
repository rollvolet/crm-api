using System.Threading.Tasks;

namespace Rollvolet.CRM.Business.Managers.Interfaces
{
    public interface ISystemTaskExecutor
    {
        Task RestoreVatCertificates();
    }
}
