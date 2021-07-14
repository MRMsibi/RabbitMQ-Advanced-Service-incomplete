namespace RabbitMQService.Configuration
{
    /// An options model that "contains" sections for producing and consuming connections of a RabbitMQ clients.
    public class RabbitMqConnectionOptions
    {
        
        /// Producer connection.
        public RabbitMqServiceOptions? ProducerOptions { get; set; }

        /// Consumer connection.
        public RabbitMqServiceOptions? ConsumerOptions { get; set; }
    }
}