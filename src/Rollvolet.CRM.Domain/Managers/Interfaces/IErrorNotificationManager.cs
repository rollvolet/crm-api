using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface IErrorNotificationManager
    {
        Task<ErrorNotification> CreateAsync(ErrorNotification errorNotification);
    }
}