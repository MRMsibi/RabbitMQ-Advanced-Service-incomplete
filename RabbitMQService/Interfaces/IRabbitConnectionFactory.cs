using RabbitMQService.Configuration;
using RabbitMQ.Client.Events;

namespace RabbitMQService.Interfaces
{
    /// Interface of the service that is responsible for creating RabbitMQ connections depending on options 
    public interface IRabbitMqConnectionFactory
    {
        /// Create a RabbitMQ connection.
        /// Returns An instance of connection 
        /// If options parameter is null the method return null too.
        IConnection? CreateRabbitMqConnection(RabbitMqServiceOptions? options);

        /// Create a consumer depending on the connection channel.
        AsyncEventingBasicConsumer CreateConsumer(IModel channel);
    }
}