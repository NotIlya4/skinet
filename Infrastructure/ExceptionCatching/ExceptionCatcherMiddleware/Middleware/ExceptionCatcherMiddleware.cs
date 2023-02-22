﻿using Infrastructure.ExceptionCatching.ExceptionCatcherMiddleware.Mappers;
using Infrastructure.ExceptionCatching.ExceptionCatcherMiddleware.Mappers.CreatingCustomMappers;
using Infrastructure.ExceptionCatching.ExceptionCatcherMiddleware.Mappers.Dispatchers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.ExceptionCatching.ExceptionCatcherMiddleware.Middleware;

internal class ExceptionCatcherMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionCatcherMiddleware> _logger;
    private readonly MappersDispatcher _mappersDispatcher;

    public ExceptionCatcherMiddleware(ILogger<ExceptionCatcherMiddleware> logger,
        MappersDispatcher mappersDispatcher)
    {
        _logger = logger;
        _mappersDispatcher = mappersDispatcher;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception caught by {catcher}", nameof(ExceptionCatcherMiddleware));
            
            BadResponse badResponse = _mappersDispatcher.Map(e);
            context.Response.StatusCode = badResponse.StatusCode;

            ActionContext actionContext = new();
            actionContext.HttpContext = context;
            
            ObjectResult objectResult = new(badResponse.ExceptionDto);
            objectResult.StatusCode = badResponse.StatusCode;
            await objectResult.ExecuteResultAsync(actionContext);
        }
    }
}