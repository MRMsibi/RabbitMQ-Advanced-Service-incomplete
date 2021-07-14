using System;

namespace RabbitMQService.Exceptions
{
    /// This exception is thrown when an initial connection could not be established even with retry mechanism.
    public class InitialConnectionException : Exception
    {
        /// The number of retries which has been attempted.
        public int NumberOfRetries { get; set; }

        public InitialConnectionException(string message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}