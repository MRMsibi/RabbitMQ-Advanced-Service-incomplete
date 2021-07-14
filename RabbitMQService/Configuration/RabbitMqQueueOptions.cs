using System.Collections.Generic;

namespace RabbitMQService.Configuration
{
    /// Queue options.
    public class RabbitMqQueueOptions
    {
        /// Queue name.
        public string Name { get; set; } = string.Empty;

        /// Durable option.
        public bool Durable { get; set; } = true;

        /// Exclusive option.
        public bool Exclusive { get; set; }

        /// AutoDelete option.
        public bool AutoDelete { get; set; }

        /// Routing keys collection that queue "listens".
        public HashSet<string> RoutingKeys { get; set; } = new HashSet<string>();

        /// Additional arguments.
        public IDictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();
    }
}