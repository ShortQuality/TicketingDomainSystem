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

namespace TicketingSystem.UnitTests.Controllers
{
    public class SectionsControllerTests
    {
        [Fact]
        public async Task GetSections_Returns_Ok_With_Sections_For_Venue()
        {
            // Arrange
            var venueId = 1;
            var sections = new List<Section>
            {
                new Section { Id = 1, Letter = 'A', VenueId = venueId },
                new Section { Id = 2, Letter = 'B', VenueId = venueId },
                new Section { Id = 3, Letter = 'C', VenueId = venueId }
            };

            var mockRepository = new Mock<IRepository<Section>>();
            mockRepository.Setup(uof => uof.GetAsync(
                It.IsAny<Expression<Func<Section, bool>>>(),
                It.IsAny<Func<IQueryable<Section>, IOrderedQueryable<Section>>>()))
                .ReturnsAsync(sections);

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.SectionsRepository).Returns(mockRepository.Object);

            var controller = new VenuesController(mockUnitOfWork.Object);

            // Act
            var result = await controller.GetSections(venueId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedSections = Assert.IsAssignableFrom<IEnumerable<Section>>(okResult.Value);
            Assert.Equal(2, returnedSections.Count());
            Assert.All(returnedSections, section => Assert.Equal(venueId, section.VenueId));
        }
    }
}
