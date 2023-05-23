using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TicketingDomainSystem.Controllers;
using TicketingSystem.BL;
using TicketingSystem.DAL.Entities;
using TicketingSystem.DAL.Interfaces;

namespace TicketingSystem.UnitTests.Controllers
{
    public class PaymentsControllerTests
    {
        [Fact]
        public void GetAsync_Returns_Payment_With_Booked_SeatState()
        {
            //Arrange
            var mockRepository = new Mock<IRepository<Payment>>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            int paymentId = 1;

            var payment = new Payment
            {
                Id = paymentId,
                Status = (int)PaymentStatus.Pending,
                Cart = new Cart {
                    Tickets = new List<Ticket> { 
                        new Ticket {
                            Seat = new Seat {
                                SeatState = (int)SeatState.Booked
                }}}}
            };

            mockRepository.Setup(uof => uof.GetAsync(
                It.IsAny<Expression<Func<Payment, bool>>>(),
                It.IsAny<Func<IQueryable<Payment>, IOrderedQueryable<Payment>>>()))
                .ReturnsAsync(new List<Payment> { payment });

            mockUnitOfWork.Setup(uow => uow.PaymentsRepository).Returns(mockRepository.Object);

            //Act
            var controller = new PaymentsController(mockUnitOfWork.Object);
            var result = controller.FailPayment(paymentId);

            // Assert
            Assert.IsType<OkResult>(result);
            Assert.Equal((int)PaymentStatus.Failed, payment.Status);
            Assert.Equal((int)SeatState.Available, payment.Cart.Tickets.First().Seat.SeatState);
        }
    }
}
