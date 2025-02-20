using Users.Contracts.Responses;
using Fv = FluentValidation;

namespace Users.Api.Mapping;

public class ValidationMappingMiddleware
{
    private readonly RequestDelegate _next;

    public ValidationMappingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch(Fv.ValidationException ex)
        {
            context.Response.StatusCode = 400;
            var validationFailureResponse = new ValidationFailureResponse
            {
                Errors = ex.Errors.Select(x => new ValidationResponse
                {
                    PropertyName = new string(x.PropertyName.SkipWhile(c => c != '.').Skip(1).ToArray()),
                    Message = x.ErrorMessage
                })
            };

            await context.Response.WriteAsJsonAsync(validationFailureResponse);
        }
    }
}
