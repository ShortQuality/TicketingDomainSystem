using System;
using System.Text;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace TicketingDomainSystem.Handlers
{
    public class NotificationHandler : IBasicConsumer
    {
        private readonly IModel _channel;
        private readonly MailjetClient _mailjetClient;
        private readonly Policy _retryPolicy;

        IModel IBasicConsumer.Model => throw new NotImplementedException();

        public NotificationHandler(IModel channel, MailjetClient mailjetClient)
        {
            _channel = channel;
            _mailjetClient = mailjetClient;

            // Create a retry policy using Polly
            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetry(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        event EventHandler<ConsumerEventArgs> IBasicConsumer.ConsumerCancelled
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        public void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
        {
            try
            {
                // Convert the message body to a string
                var message = Encoding.UTF8.GetString(body.ToArray());

                // Send the notification email using MailJet with a retry policy
                _retryPolicy.Execute(() =>
                {
                    var request = new MailjetRequest
                    {
                        Resource = Send.Resource,
                    }
                    .Property(Send.Messages, new JArray {
                    new JObject {
                        {"From", new JObject {
                            {"Email", "sender@example.com"},
                            {"Name", "Sender Name"}
                        }},
                        {"To", new JArray {
                            new JObject {
                                {"Email", "recipient@example.com"},
                                {"Name", "Recipient Name"}
                            }
                        }},
                        {"Subject", "Notification"},
                        {"TextPart", message}
                    }
                    });
                    var response = _mailjetClient.PostAsync(request);
                });

                // Acknowledge the message
                _channel.BasicAck(deliveryTag, false);
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur
                Console.WriteLine($"An error occurred: {ex.Message}");

                // Reject the message and requeue it
                _channel.BasicReject(deliveryTag, true);
            }
        }

        public void HandleBasicCancel(string consumerTag)
        {
            // Handle the cancellation of the consumer
        }

        public void HandleBasicCancelOk(string consumerTag)
        {
            // Handle the acknowledgement of the cancellation
        }

        public void HandleBasicConsumeOk(string consumerTag)
        {
            // Handle the acknowledgement of the consumer registration
        }

        public void HandleModelShutdown(object model, ShutdownEventArgs reason)
        {
            // Handle the shutdown of the model
        }

        void IBasicConsumer.HandleBasicCancel(string consumerTag)
        {
            throw new NotImplementedException();
        }

        void IBasicConsumer.HandleBasicCancelOk(string consumerTag)
        {
            throw new NotImplementedException();
        }

        void IBasicConsumer.HandleBasicConsumeOk(string consumerTag)
        {
            throw new NotImplementedException();
        }

        void IBasicConsumer.HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
        {
            throw new NotImplementedException();
        }

        void IBasicConsumer.HandleModelShutdown(object model, ShutdownEventArgs reason)
        {
            throw new NotImplementedException();
        }
    }
}
