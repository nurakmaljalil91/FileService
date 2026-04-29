using Mediator;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviours;

/// <summary>
/// Provides logging behavior for MediatR pipeline requests and responses.
/// Logs request handling start and completion with elapsed time.
/// </summary>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for logging request and response information.</param>
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles the MediatR pipeline request by logging the start and completion with elapsed time.
    /// </summary>
    /// <param name="request">The request instance being handled.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <param name="next">The delegate representing the next handler in the pipeline.</param>
    /// <returns>The response from the next handler in the pipeline.</returns>
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var requestName = typeof(TRequest).Name;    
        var start = DateTime.UtcNow;

        _logger.LogInformation("Handling {RequestName}", requestName);

        var response = await next().ConfigureAwait(false);
        var elapsed = (DateTime.UtcNow - start).TotalMilliseconds;
        _logger.LogInformation("Handled {RequestName} in {Elapsed:0.000} ms", requestName, elapsed);
        return response;
    }
}
