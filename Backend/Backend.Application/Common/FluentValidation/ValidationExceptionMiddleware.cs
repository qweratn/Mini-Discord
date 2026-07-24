using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Backend.Application.Common.FluentValidation;

public class ValidationExceptionMiddleware
{
    private readonly RequestDelegate next;

    public ValidationExceptionMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var errors = ex.Errors.Select(x => new
            {
                x.PropertyName,
                x.ErrorMessage,
            });

            await context.Response.WriteAsJsonAsync(new
            {
                context.Response.StatusCode,
                Message = "Validation failed",
                Errors = errors,
            });
        }
    }
}
