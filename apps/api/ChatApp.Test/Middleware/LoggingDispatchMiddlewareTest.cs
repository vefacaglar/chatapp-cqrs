using ChatApp.Application.Middleware;
using ChatApp.Infrastructure;
using CustomDispatcher.Abstractions.Pipelines;
using Microsoft.Extensions.Logging;
using Moq;

namespace ChatApp.Test.Middleware
{
    public class LoggingDispatchMiddlewareTest
    {
        [Fact]
        public async Task HandleAsync_SavesRequestToEventStore()
        {
            var loggerMock = new Mock<ILogger<string>>();
            var eventStoreMock = new Mock<IEventStore>();

            var middleware = new LoggingDispatchMiddleware<string, string>(
                loggerMock.Object,
                eventStoreMock.Object);

            var request = "test-request";
            DispatchContinuation<string> next = () => Task.FromResult("result");

            await middleware.HandleAsync(request, next, CancellationToken.None);

            eventStoreMock.Verify(
                x => x.Save(request, "String"),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_CallsNextAndReturnsResult()
        {
            var loggerMock = new Mock<ILogger<string>>();
            var eventStoreMock = new Mock<IEventStore>();

            var middleware = new LoggingDispatchMiddleware<string, string>(
                loggerMock.Object,
                eventStoreMock.Object);

            var request = "test-request";
            DispatchContinuation<string> next = () => Task.FromResult("expected-result");

            var result = await middleware.HandleAsync(request, next, CancellationToken.None);

            Assert.Equal("expected-result", result);
        }

        [Fact]
        public async Task HandleAsync_LogsStartAndEnd()
        {
            var loggerMock = new Mock<ILogger<string>>();
            var eventStoreMock = new Mock<IEventStore>();

            var middleware = new LoggingDispatchMiddleware<string, string>(
                loggerMock.Object,
                eventStoreMock.Object);

            var request = "test-request";
            DispatchContinuation<string> next = () => Task.FromResult("result");

            await middleware.HandleAsync(request, next, CancellationToken.None);

            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("[START]")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);

            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("[END]")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
