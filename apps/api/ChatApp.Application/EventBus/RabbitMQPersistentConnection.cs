using ChatApp.Infrastructure;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;

namespace ChatApp.Application.EventBus
{
    public class RabbitMQPersistentConnection : IPersistentConnection<IChannel>
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<RabbitMQPersistentConnection> _logger;
        private readonly int _retryCount;
        private readonly object sync_root = new object();
        private IConnection? _connection;
        private bool _disposed;

        public RabbitMQPersistentConnection(
            IConnectionFactory connectionFactory,
            ILogger<RabbitMQPersistentConnection> logger,
            int retryCount = 5
            )
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _retryCount = retryCount;
        }

        public bool IsConnected
        {
            get
            {
                return _connection != null && _connection.IsOpen && !_disposed;
            }
        }

        public IChannel CreateChannel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }

            return _connection!.CreateChannelAsync().GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            try
            {
                _connection?.Dispose();
            }
            catch (IOException ex)
            {
                _logger.LogCritical(ex.ToString());
            }
        }

        public bool TryConnect()
        {
            _logger.LogInformation("RabbitMQ Client is trying to connect");

            lock (sync_root)
            {
                var policy = RetryPolicy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                    {
                        _logger.LogWarning(ex.ToString());
                    }
                );

                policy.Execute(() =>
                {
                    _connection = _connectionFactory
                          .CreateConnectionAsync()
                          .GetAwaiter()
                          .GetResult();
                });

                if (IsConnected)
                {
                    _connection!.ConnectionShutdownAsync += OnConnectionShutdown;
                    _connection!.CallbackExceptionAsync += OnCallbackException;
                    _connection!.ConnectionBlockedAsync += OnConnectionBlocked;

                    _logger.LogInformation($"RabbitMQ persistent connection acquired a connection {_connection!.Endpoint.HostName} and is subscribed to failure events");

                    return true;
                }
                else
                {
                    _logger.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");

                    return false;
                }
            }
        }

        private Task OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (_disposed) return Task.CompletedTask;

            _logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");

            TryConnect();
            return Task.CompletedTask;
        }

        private Task OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (_disposed) return Task.CompletedTask;

            _logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");

            TryConnect();
            return Task.CompletedTask;
        }

        private Task OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (_disposed) return Task.CompletedTask;

            _logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");

            TryConnect();
            return Task.CompletedTask;
        }
    }
}
