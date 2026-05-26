using ChatApp.Infrastructure;
using CustomDispatcher.Abstractions.Pipelines;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace ChatApp.Application.Middleware
{
    public class LoggingDispatchMiddleware<TRequest, TResult> : IDispatchMiddleware<TRequest, TResult>
    {
        private readonly ILogger<TRequest> _logger;
        private readonly IEventStore _eventStore;

        public LoggingDispatchMiddleware(
            ILogger<TRequest> logger,
            IEventStore eventStore
            )
        {
            _logger = logger;
            _eventStore = eventStore;
        }

        public async Task<TResult> HandleAsync(
            TRequest request,
            DispatchContinuation<TResult> next,
            CancellationToken cancellationToken = default)
        {
            var requestName = request!.GetType().Name;
            var requestNameWithGuid = $"{requestName} [{Guid.NewGuid()}]";

            _logger.LogInformation($"[START] {requestNameWithGuid}");
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await _eventStore.Save(request, requestName);
                LogRequestWithProps(request, requestNameWithGuid);
                return await next();
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation($"[END] {requestNameWithGuid}; Execution time={stopwatch.ElapsedMilliseconds}ms");
            }
        }

        private void LogRequestWithProps(TRequest request, string requestNameWithGuid)
        {
            try
            {
                _logger.LogInformation($"[PROPS] {requestNameWithGuid} {JsonSerializer.Serialize(request)}");
            }
            catch (NotSupportedException)
            {
                _logger.LogInformation($"[Serialization ERROR] {requestNameWithGuid} Could not serialize the request.");
            }
        }
    }
}
