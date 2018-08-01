using System.IO;
using System.Threading.Tasks;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface IDocumentGenerationManager
    {
        Task<Stream> CreateVisitReport(int requestId);
        Task<Stream> CreateAndStoreOfferDocument(int offerId);
    }
}
