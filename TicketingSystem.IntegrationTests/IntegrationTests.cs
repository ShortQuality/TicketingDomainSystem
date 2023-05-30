using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Linq;
using System.Net;
using System.Text;
using TicketingSystem.DAL.Entities;
using Assert = NUnit.Framework.Assert;

namespace TicketingSystem.IntegrationTests
{
    [TestFixture]
    public class IntegrationTests
    {
        private HttpClient _client;

        public IntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Test]
        public async Task GetPayment_ReturnsOkResponse()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "payments/1");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task GetCart_ReturnsOkResponse()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/orders/carts/1");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task CreateCart_ReturnsCreatedResponse()
        {
            // Arrange
            var ticket = new Ticket { Event = new Event { Name = "Test Event", Date = new DateTime() }, Price = new Price { Amount = 10 } };
            var cart = new Cart { Id = 1, Tickets = new List<Ticket> { ticket } };
            var content = new StringContent(JsonConvert.SerializeObject(cart), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "orders/carts/1")
            {
                Content = content
            };

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

        [Test]
        public async Task BookCart_ReturnsNoContentResponse()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Put, "orders/carts/1/book");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Test]
        public async Task DeleteCartItem_ReturnsNoContentResponse()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Delete, "carts/1/events/1/seats/1");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}
