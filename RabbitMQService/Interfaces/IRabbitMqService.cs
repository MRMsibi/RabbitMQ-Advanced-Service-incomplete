namespace RabbitMQService.Interfaces
{
    /// Custom RabbitMQ service interface.
    public interface IRabbitMqService
    {
        /// Specify the connection that will be used by the service.
        void UseConnection(IConnection connection);

        /// Specify the channel that will be used by the service.
        void UseChannel(IModel channel);
    }
}