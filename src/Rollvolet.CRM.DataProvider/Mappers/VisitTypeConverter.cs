using AutoMapper;

namespace Rollvolet.CRM.DataProvider.Mappers
{
    public class VisitTypeConverter : ITypeConverter<Models.Visit, Domain.Models.CalendarEvent>
    {
        public Domain.Models.CalendarEvent Convert(Models.Visit src, Domain.Models.CalendarEvent dest, ResolutionContext context)
        {
            if (src == null || src.VisitDate == null)
                return null;

            Domain.Models.Request request = null;
            if (src.Request != null)
                request = new Domain.Models.Request() {
                    Id = (int) src.Request.Id,
                    Comment = src.Comment,
                    Visitor = src.Visitor,
                    OfferExpected = src.OfferExpected
                };
            else if (src.RequestId != null)
                request = new Domain.Models.Request() { Id = (int) src.RequestId };

            return new Domain.Models.CalendarEvent {
                Id = src.Id,
                MsObjectId = src.MsObjectId,
                CalendarId = src.CalendarId,
                CalendarSubject = src.CalendarSubject,
                VisitDate = src.VisitDate,
                Request = request
            };
        }
    }
}