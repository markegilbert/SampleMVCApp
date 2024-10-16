using Microsoft.AspNetCore.Diagnostics;

namespace SampleApp.ErrorHandling
{

    // Adapted from Source: https://www.milanjovanovic.tech/blog/global-error-handling-in-aspnetcore-8
    // This requires these two lines be added to Program.cs:
    //      builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    //      builder.Services.AddProblemDetails();
    // and
    //      app.UseExceptionHandler();
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _Logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> Logger)
        {
            if(Logger is null) { throw new ArgumentNullException($"The '{nameof(Logger)}' parameter was null or otherwise invalid"); }
            this._Logger = Logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            this._Logger.LogError(exception, "Exception occurred: {Message}", exception.Message);
            httpContext.Response.Redirect("/Home/Error");
            return true;
        }
    }
}
