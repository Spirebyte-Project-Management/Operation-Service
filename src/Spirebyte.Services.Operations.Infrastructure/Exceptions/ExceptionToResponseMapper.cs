using Convey.WebApi.Exceptions;
using System;
using System.Net;

namespace Spirebyte.Services.Operations.Infrastructure.Exceptions
{
    internal sealed class ExceptionToResponseMapper : IExceptionToResponseMapper
    {
        public ExceptionResponse Map(Exception exception)
            => exception switch
            {
                _ => new ExceptionResponse(new { code = "error", reason = "There was an error." },
                    HttpStatusCode.BadRequest)
            };
    }
}
