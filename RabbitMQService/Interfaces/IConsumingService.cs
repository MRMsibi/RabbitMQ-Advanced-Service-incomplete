using RabbitMQ.Client.Events;

namespace RabbitMQService.Interfaces
{
    /// Custom RabbitMQ consuming service interface.
    public interface IConsumingService : IRabbitMqService
    {
        /// RabbitMQ consuming connection.
        IConnection? Connection { get; }
    
        /// RabbitMQ consuming channel.
        IModel? Channel { get; }
        
        /// Asynchronous consumer. 
        AsyncEventingBasicConsumer? Consumer { get; }
    
        /// Start consuming (getting messages).
        void StartConsuming();
    
        /// Stop consuming (getting messages).
        void StopConsuming();

        /// Specify a consumer instance that will be used by the service.
        void UseConsumer(AsyncEventingBasicConsumer consumer);
    }
}