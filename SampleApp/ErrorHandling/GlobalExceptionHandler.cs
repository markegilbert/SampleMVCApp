﻿using Microsoft.AspNetCore.Diagnostics;

namespace SampleApp.ErrorHandling
{

    // Adapted from Source: https://www.milanjovanovic.tech/blog/global-error-handling-in-aspnetcore-8
    // This requires these two lines be added to Program.cs:
    //      builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    //      builder.Services.AddProblemDetails();
    // and
    //      app.UseExceptionHandler();
    internal sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);
            httpContext.Response.Redirect("/Home/Error");
            return true;
        }
    }
}
