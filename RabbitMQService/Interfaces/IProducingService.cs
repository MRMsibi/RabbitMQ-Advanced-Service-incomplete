using System;
using System.Threading.Tasks;

namespace RabbitMQService.Interfaces
{
    /// Custom RabbitMQ producing service interface.
    public interface IProducingService : IRabbitMqService
    {
        /// RabbitMQ producing connection.
        IConnection? Connection { get; }

        /// RabbitMQ producing channel.
        IModel? Channel { get; }

        
        /// Send a message.
        void SendString(string message, string exchangeName, string routingKey);

        /// Send a delayed message.
        void SendString(string message, string exchangeName, string routingKey, int millisecondsDelay);

        /// Send a delayed message asynchronously.
        Task SendStringAsync(string message, string exchangeName, string routingKey);
                
        /// Send a delayed message asynchronously.
        Task SendStringAsync(string message, string exchangeName, string routingKey, int millisecondsDelay);
        
    }
}