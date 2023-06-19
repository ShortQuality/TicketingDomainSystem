using Mailjet.Client;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Channels;
using System.Xml;
using TicketingDomainSystem.Handlers;
using TicketingSystem.BL;
using TicketingSystem.DAL.Entities;
using TicketingSystem.DAL.Interfaces;

namespace TicketingDomainSystem.Controllers
{
    [ApiController]
    [Route("[payments]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public PaymentsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            // Create a connection to RabbitMQ
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };
            _connection = factory.CreateConnection();

            // Create a channel to communicate with RabbitMQ
            _channel = _connection.CreateModel();

            // Create an instance of the NotificationHandler class
            var mailjetClient = new MailjetClient("API_KEY", "API_SECRET");
            var notificationHandler = new NotificationHandler(_channel, mailjetClient);

            // Register the notification handler as a consumer on the channel
            _channel.BasicConsume("notifications", false, notificationHandler);

        }
        public IActionResult SendEmailNotification(string email)
        {
            // Create a message to send to the notification handler
            var message = Encoding.UTF8.GetBytes(email);

            // Publish the message to the "email_notifications" queue
            _channel.BasicPublish(exchange: "", routingKey: "email_notifications", basicProperties: null, body: message);

            return Ok();
        }

        [HttpGet("payments/{paymentId}")]
        public async Task<ActionResult<IEnumerable<Seat>>> GetPayment(int paymentId)
        {
            var payment = (await _unitOfWork.PaymentsRepository.GetAsync(
                filter: s => s.Id == paymentId)).FirstOrDefault();
            return Ok(payment.Status);
        }

        [HttpPost("{paymentId}/complete")]
        public async Task<ActionResult> CompletePayment(int paymentId)
        {
            var payment = (await _unitOfWork.PaymentsRepository
                .GetAsync(p => p.Id == paymentId, includeProperties: p => p.Cart.Tickets)).FirstOrDefault();

            if (payment == null)
            {
                return NotFound();
            }
            payment.Status = (int)PaymentStatus.Completed;
            foreach (var ticket in payment.Cart.Tickets)
            {
                ticket.Seat.SeatState = (int)SeatState.Sold;
            }
            await _unitOfWork.SaveAsync();

            string email = "Payment Complete";
            SendEmailNotification(email);
            return Ok();
        }

        [HttpPost("{paymentId}/failed")]
        public async Task<ActionResult> FailPayment(int paymentId)
        {
            var payment = (await _unitOfWork.PaymentsRepository
                .GetAsync(p => p.Id == paymentId, includeProperties: p => p.Cart.Tickets)).FirstOrDefault();

            if (payment == null)
            {
                return NotFound();
            }
            payment.Status = (int)PaymentStatus.Failed;
            foreach (var ticket in payment.Cart.Tickets)
            {
                ticket.Seat.SeatState = (int)SeatState.Available;
            }
            await _unitOfWork.SaveAsync();
            return Ok();
        }
    }
}
