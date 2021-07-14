using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using RabbitMQService.Interfaces;
using RabbitMQ.Client.Events;

namespace RabbitMQService
{
    public class ConsumingService : IConsumingService, IDisposable
    {
        public IConnection? Connection { get; private set; }

        public IModel? Channel { get; private set; }
        
        public AsyncEventingBasicConsumer? Consumer { get; private set; }

        private bool _consumingStarted;

        private readonly IEnumerable<RabbitMqExchange> _exchanges;

        private IEnumerable<string> _consumerTags = new List<string>();

        public ConsumingService(IEnumerable<RabbitMqExchange> exchanges)
        {
            _exchanges = exchanges;
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

        public void UseConsumer(AsyncEventingBasicConsumer consumer)
        {
            Consumer = consumer;
        }

        public void StartConsuming()
        {
            Channel.EnsureIsNotNull();
            Consumer.EnsureIsNotNull();

            if (_consumingStarted)
            {
                return;
            }
            
            Consumer.Received += ConsumerOnReceived;
            _consumingStarted = true;

            var consumptionExchanges = _exchanges.Where(x => x.IsConsuming);
            _consumerTags = consumptionExchanges.SelectMany(
                    exchange => exchange.Options.Queues.Select(
                        queue => Channel.BasicConsume(queue: queue.Name, autoAck: false, consumer: Consumer)))
                .Distinct()
                .ToList();
        }

        public void StopConsuming()
        {
            Channel.EnsureIsNotNull();
            Consumer.EnsureIsNotNull();

            if (!_consumingStarted)
            {
                return;
            }

            Consumer.Received -= ConsumerOnReceived;
            _consumingStarted = false;
            foreach (var tag in _consumerTags)
            {
                Channel.BasicCancel(tag);
            }
        }
/*
        private void AckAction(BasicDeliverEventArgs eventArgs) => Channel.EnsureIsNotNull().BasicAck(eventArgs.DeliveryTag, false);

        private async Task ConsumerOnReceived(object sender, BasicDeliverEventArgs eventArgs)
        {
            var exchangeOptions = _exchanges.FirstOrDefault(x => string.Equals(x.Name, eventArgs.Exchange)).EnsureIsNotNull().Options;
            var context = new MessageHandlingContext(eventArgs, AckAction, exchangeOptions.DisableAutoAck);
            await _messageHandlingPipelineExecutingService.Execute(context);
        }
        */
    }
}