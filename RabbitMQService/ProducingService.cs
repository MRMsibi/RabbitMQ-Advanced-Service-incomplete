using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RabbitMQService.Exceptions;
using RabbitMQService.Interfaces;

namespace RabbitMQService
{
    public sealed class ProducingService : IProducingService, IDisposable
    {
        public IConnection? Connection { get; private set; }

        public IModel? Channel { get; private set; }

        private readonly IReadOnlyCollection<RabbitMqExchange> _exchanges;
        private readonly object _lock = new object();

        private const int QueueExpirationTime = 60000;

        public ProducingService(IEnumerable<RabbitMqExchange> exchanges)
        {
            _exchanges = exchanges.Where(x => x.IsProducing).ToList();
        }

        public void Dispose()
        {
            if (Channel?.IsOpen == true)
            {
                Channel.Close((int)HttpStatusCode.OK, "Channel closed");
            }

            if (Connection?.IsOpen == true)
            {
                Connection.Close();
            }

            Channel?.Dispose();
            Connection?.Dispose();
        }

        public void UseConnection(IConnection connection)
        {
            Connection = connection;
        }

        public void UseChannel(IModel channel)
        {
            Channel = channel;
        }

        public void Send<T>(T @object, string exchangeName, string routingKey) where T : class
        {
            EnsureProducingChannelIsNotNull();
            ValidateArguments(exchangeName, routingKey);
            var json = JsonConvert.SerializeObject(@object);
            var bytes = Encoding.UTF8.GetBytes(json);
            var properties = CreateProperties();
            Send(bytes, properties, exchangeName, routingKey);
        }

        
        public void SendString(string message, string exchangeName, string routingKey)
        {
            EnsureProducingChannelIsNotNull();
            ValidateArguments(exchangeName, routingKey);
            var bytes = Encoding.UTF8.GetBytes(message);
            Send(bytes, CreateProperties(), exchangeName, routingKey);
        }

        public void Send(ReadOnlyMemory<byte> bytes, IBasicProperties properties, string exchangeName, string routingKey)
        {
            EnsureProducingChannelIsNotNull();
            ValidateArguments(exchangeName, routingKey);
            lock (_lock)
            {
                Channel.BasicPublish(exchange: exchangeName,
                    routingKey: routingKey,
                    basicProperties: properties,
                    body: bytes);
            }
        }

        public async Task SendAsync<T>(T @object, string exchangeName, string routingKey) where T : class =>
            await Task.Run(() => Send(@object, exchangeName, routingKey)).ConfigureAwait(false);

        public async Task SendAsync<T>(T @object, string exchangeName, string routingKey, int millisecondsDelay) where T : class =>
            await Task.Run(() => Send(@object, exchangeName, routingKey, millisecondsDelay)).ConfigureAwait(false);

        public async Task SendStringAsync(string message, string exchangeName, string routingKey) =>
            await Task.Run(() => SendString(message, exchangeName, routingKey)).ConfigureAwait(false);

        public async Task SendAsync(ReadOnlyMemory<byte> bytes, IBasicProperties properties, string exchangeName, string routingKey) =>
            await Task.Run(() => Send(bytes, properties, exchangeName, routingKey)).ConfigureAwait(false);

        private IBasicProperties CreateProperties()
        {
            var properties = Channel.EnsureIsNotNull().CreateBasicProperties();
            properties.Persistent = true;
            return properties;
        }

        private IBasicProperties CreateJsonProperties()
        {
            var properties = Channel.EnsureIsNotNull().CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";
            return properties;
        }

        private void EnsureProducingChannelIsNotNull()
        {
            if (Channel is null)
            {
                throw new ProducingChannelIsNullException($"Producing channel is null. Configure {nameof(IProducingService)} or full functional {nameof(IProducingService)} for producing messages");
            }
        }

        internal void ValidateArguments(string exchangeName, string routingKey)
        {
            if (string.IsNullOrEmpty(exchangeName))
            {
                throw new ArgumentException($"Argument {nameof(exchangeName)} is null or empty.", nameof(exchangeName));
            }
            if (string.IsNullOrEmpty(routingKey))
            {
                throw new ArgumentException($"Argument {nameof(routingKey)} is null or empty.", nameof(routingKey));
            }
        }

        private string DeclareDelayedQueue(string exchange, string deadLetterExchange, string routingKey, int millisecondsDelay)
        {
            var QueueName = $"{routingKey}.delayed.{millisecondsDelay}";
            
            Channel.EnsureIsNotNull();
            Channel.QueueDeclare(
                queue: delayedQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            Channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            return QueueName;
        }
                
    }
}