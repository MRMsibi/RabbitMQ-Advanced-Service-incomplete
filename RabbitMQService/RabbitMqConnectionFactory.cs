using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using RabbitMQService.Configuration;
using RabbitMQService.Exceptions;
using RabbitMQService.Interfaces;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace RabbitMQService
{
    public class RabbitMqConnectionFactory : IRabbitMqConnectionFactory
    {
        public IConnection? CreateRabbitMqConnection(RabbitMqServiceOptions? options)
        {
            if (options is null)
            {
                return null;
            }

            var factory = new ConnectionFactory
            {
                Port = options.Port,
                UserName = options.UserName,
                Password = options.Password,
                AutomaticRecoveryEnabled = options.AutomaticRecoveryEnabled,
                TopologyRecoveryEnabled = options.TopologyRecoveryEnabled,
                RequestedConnectionTimeout = options.RequestedConnectionTimeout,
                RequestedHeartbeat = options.RequestedHeartbeat,
                DispatchConsumersAsync = true
            };

            return CreateConnection(options, factory)
        }

        
        public AsyncEventingBasicConsumer CreateConsumer(IModel channel) => new AsyncEventingBasicConsumer(channel);

        private static IConnection CreateConnection(RabbitMqServiceOptions options, ConnectionFactory factory)
        {
            if (options.HostNames.Any())
            {
                return TryToCreateConnection(() => factory.CreateConnection(options.HostNames.ToList()), options.InitialConnectionRetries, options.InitialConnectionRetryTimeoutMilliseconds);
            }

            factory.HostName = options.HostName;
            return TryToCreateConnection(factory.CreateConnection, options.InitialConnectionRetries, options.InitialConnectionRetryTimeoutMilliseconds);
        }

        private static IConnection TryToCreateConnection(Func<IConnection> connectionFunction, int numberOfRetries, int timeoutMilliseconds)
        {
            ValidateArguments(numberOfRetries, timeoutMilliseconds);

            var attempts = 0;
            BrokerUnreachableException? latestException = null;
            while (attempts < numberOfRetries)
            {
                try
                {
                    if (attempts > 0)
                    {
                        Thread.Sleep(timeoutMilliseconds);
                    }

                    return connectionFunction();
                }
                catch (BrokerUnreachableException exception)
                {
                    attempts++;
                    latestException = exception;
                }
            }

            throw new InitialConnectionException($"Could not establish an initial connection in {numberOfRetries} retries", latestException)
            {
                NumberOfRetries = attempts
            };
        }

        private static void ValidateArguments(int numberOfRetries, int timeoutMilliseconds)
        {
            if (numberOfRetries < 1)
            {
                throw new ArgumentException("Number of retries should be a positive number.", nameof(numberOfRetries));
            }

            if (timeoutMilliseconds < 1)
            {
                throw new ArgumentException("Initial reconnection timeout should be a positive number.", nameof(timeoutMilliseconds));
            }
        }
    }
    
}
