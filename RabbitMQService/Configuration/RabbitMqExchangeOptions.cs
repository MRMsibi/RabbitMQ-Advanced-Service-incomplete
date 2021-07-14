using System.Collections.Generic;

namespace RabbitMQService.Configuration
{
    /// Exchange options.
    public class RabbitMqExchangeOptions
    {
        /// Exchange type.
        public string Type { get; set; } = "direct";

        /// Durable option.
        public bool Durable { get; set; } = true;

        /// AutoDelete option.
        public bool AutoDelete { get; set; }

        /// Default dead-letter-exchange.
        public string DeadLetterExchange { get; set; } = "default.dlx.exchange";

        /// Dead-letter-exchange type.
        public string DeadLetterExchangeType { get; set; } = "direct";

        /// Option to re-queue failed messages.
        public bool RequeueFailedMessages { get; set; } = true;

        /// Re-queue message attempts.
        public int RequeueAttempts { get; set; } = 2;
        
        /// Re-queue timeout in milliseconds.
        public int RequeueTimeoutMilliseconds { get; set; } = 200;

        /// Additional arguments.
        public IDictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();

        /// Collection of queues bound to the exchange.
        public IList<RabbitMqQueueOptions> Queues { get; set; } = new List<RabbitMqQueueOptions>();

        /// Do not auto-ack a received message after processing.
        public bool DisableAutoAck { get; set; } = false;
    }
}