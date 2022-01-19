using System;
using System.Net;
using Convey.WebApi.Exceptions;

namespace Spirebyte.Services.Operations.Infrastructure.Exceptions;

internal sealed class ExceptionToResponseMapper : IExceptionToResponseMapper
{
    public ExceptionResponse Map(Exception exception)
    {
        return exception switch
        {
            _ => new ExceptionResponse(new { code = "error", reason = "There was an error." },
                HttpStatusCode.BadRequest)
        };
    }
}