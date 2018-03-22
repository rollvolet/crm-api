
using Microsoft.AspNetCore.Mvc;
using System;

namespace Rollvolet.CRM.API.Middleware.ExceptionHandling.Interfaces
{
    public interface IExceptionToActionResultMapper
    {
        IActionResult Map(Exception ex);
    }
}