using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TicketingDomainSystem.Controllers;
using TicketingSystem.DAL.Entities;
using TicketingSystem.DAL.Interfaces;
using TicketingSystem.DAL.Repositories;

namespace TicketingSystem.UnitTests.Controllers
{
    public class EventsControllerTests
    {
        [Fact]
        public async Task GetEvents_Returns_Ok_With_Events()
        {
            // Arrange
            var events = new List<Event>
            {
                new Event { Id = 1, Name = "Event 1" },
                new Event { Id = 2, Name = "Event 2" },
                new Event { Id = 3, Name = "Event 3" }
            };
            var mockRepository = new Mock<Repository<Event>>();
            
            mockRepository.Setup(uof => uof.GetAsync(
            It.IsAny<Expression<Func<Event, bool>>>(),
            It.IsAny<Func<IQueryable<Event>, IOrderedQueryable<Event>>>()))
            .ReturnsAsync(events);

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uof => uof.EventsRepository).Returns(mockRepository.Object);

            var controller = new EventsController(mockUnitOfWork.Object);

            // Act
            var result = await controller.GetEvents();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedEvents = Assert.IsAssignableFrom<IEnumerable<Event>>(okResult.Value);
            Assert.Equal(events.Count, returnedEvents.Count());
        }
    }
}
