﻿using FakeSurveyGenerator.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FakeSurveyGenerator.API.Filters;

public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
{
    private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;

    public ApiExceptionFilterAttribute()
    {
        // Register known exception types and handlers.
        _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
        {
            { typeof(ValidationException), HandleValidationException }
        };
    }

    public override void OnException(ExceptionContext context)
    {
        HandleException(context);

        base.OnException(context);
    }

    private void HandleException(ExceptionContext context)
    {
        var type = context.Exception.GetType();
        if (_exceptionHandlers.TryGetValue(type, out var handler))
        {
            handler.Invoke(context);
            return;
        }

        HandleUnknownException(context);
    }

    private static void HandleUnknownException(ExceptionContext context)
    {
        var details = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An error occurred while processing your request.",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };

        context.Result = new ObjectResult(details)
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };

        context.ExceptionHandled = true;
    }

    private static void HandleValidationException(ExceptionContext context)
    {
        var exception = context.Exception as ValidationException;

        var details = new ValidationProblemDetails(exception?.Errors ?? throw new InvalidOperationException($"Errors property on {nameof(ValidationException)} was not set"))
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        };

        context.Result = new UnprocessableEntityObjectResult(details);

        context.ExceptionHandled = true;
    }
}