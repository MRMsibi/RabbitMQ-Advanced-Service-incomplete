using System;
using System.Collections.Generic;

namespace RabbitMQService.Configuration
{
    
    /// RabbitMQ configuration model.
        public class RabbitMqServiceOptions
    {
        
        /// Collection of RabbitMQ server host names.
        /// It can be used when RabbitMQ HA cluster is running, and you want to connect multiple hosts.
        /// If HostNames collection is null or empty then HostName will be used to create connection.
        /// Otherwise, HostNames collection will be used and HostName property value will be ignored.
        /// If HostNames collection property and HostName property both set then HostNames will be used.
        
        public IList<string> HostNames { get; set; } = new List<string>();

        /// RabbitMQ server.
        public string HostName { get; set; } = "127.0.0.1";

        /// Port.
        public int Port { get; set; } = 5672;
     
        /// UserName that connects to the server.
        public string UserName { get; set; } = "guest";

        /// Password of the chosen user.
        public string Password { get; set; } = "guest";

        /// Automatic connection recovery option.
        public bool AutomaticRecoveryEnabled { get; set; } = true;
        
        /// Topology recovery option.
        public bool TopologyRecoveryEnabled { get; set; } = true;

        /// Timeout for connection attempts.
        public TimeSpan RequestedConnectionTimeout { get; set; } = TimeSpan.FromMilliseconds(60000);

        /// Heartbeat timeout.
        public TimeSpan RequestedHeartbeat { get; set; } = TimeSpan.FromSeconds(60);
        
        /// The number of retries for opening an initial connection.
        public int InitialConnectionRetries { get; set; } = 5;
        
        /// Timeout for initial connection opening retries.
        public int InitialConnectionRetryTimeoutMilliseconds { get; set; } = 200;
    }
}