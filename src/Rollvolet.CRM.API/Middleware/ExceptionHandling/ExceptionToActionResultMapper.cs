using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.API.Middleware.ExceptionHandling.Interfaces;
using Rollvolet.CRM.APIContracts.JsonApi;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.Hosting;

namespace Rollvolet.CRM.API.Middleware.ExceptionHandling
{
    public class
    ExceptionToActionResultMapper : IExceptionToActionResultMapper
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger _logger;

        public ExceptionToActionResultMapper(IWebHostEnvironment hostingEnvironment, ILogger<ExceptionToActionResultMapper> logger)
        {
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        public virtual IActionResult Map(Exception ex)
        {
            if (ex is EntityNotFoundException)
            {
                return new NotFoundResult();
            }

            if (ex is EntityAlreadyExistsException)
            {
                var error = new Error
                {
                    Code = "AlreadyExists",
                    Status = "422",
                    Title = "Entity already exists",
                    Detail = "The entity to be created already exists."
                };
                return new ObjectResult(error) { StatusCode = StatusCodes.Status422UnprocessableEntity };
            }

            if (ex is NotSupportedException)
            {
                var error = new Error
                {
                    Code = "NotSupported",
                    Status = "400",
                    Title = "Not supported",
                    Detail = "The operation is not supported by the API."
                };
                return new BadRequestObjectResult(error);
            }

            if (ex is IllegalArgumentException)
            {
                var typedEx = ex as IllegalArgumentException;

                var error = new Error
                {
                    Code = typedEx.Code,
                    Status = "400",
                    Title = typedEx.Title,
                    Detail = typedEx.Message
                };
                return new BadRequestObjectResult(error);
            }

            if (ex is InvalidOperationException)
            {
                var typedEx = ex as InvalidOperationException;

                var error = new Error
                {
                    Code = "InvalidOperation",
                    Status = "400",
                    Title = "Invalid operation",
                    Detail = typedEx.Message
                };
                return new BadRequestObjectResult(error);
            }

            if (ex is InvalidDataException)
            {
                var typedEx = ex as InvalidDataException;

                var error = new Error
                {
                    Code = "InvalidData",
                    Status = "500",
                    Title = "Invalid data",
                    Detail = typedEx.Message
                };
                return new ObjectResult(error) { StatusCode = StatusCodes.Status500InternalServerError };
            }

            if (ex is CodedException)
            {
                var typedEx = ex as CodedException;
                var error = new Error
                {
                    Code = typedEx.Code,
                    Status = "500",
                    Title = typedEx.Title,
                    Detail = typedEx.Message
                };
                return new ObjectResult(error) { StatusCode = StatusCodes.Status500InternalServerError };
            }

            _logger.LogDebug($"Exception of type {ex.GetType().Name} was mapped by the catch all mapper.");
            var catchAllError = new Error
            {
                Code = "CATCHALL",
                Status = "500",
                Title = "Something went wrong",
            };

            if (_hostingEnvironment.IsDevelopment())
              catchAllError.Detail = ex.Message;

            return new ObjectResult(catchAllError) { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }
}