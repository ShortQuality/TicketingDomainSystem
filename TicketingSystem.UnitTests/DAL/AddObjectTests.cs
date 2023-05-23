using Microsoft.EntityFrameworkCore;
using Moq;
using System.Xml;
using TicketingSystem.DAL;
using TicketingSystem.DAL.Entities;
using TicketingSystem.DAL.Repositories;
using Xunit.Sdk;

namespace TicketingSystem.UnitTests.DAL
{
    public class AddObjectTests
    {
        [Fact]
        public void AddAsync_ShouldAddCartToDbSet()
        {
            // Arrange
            var entity = new Cart();
            var dbSetMock = new Mock<DbSet<Cart>>();
            var repository = new Repository<Cart>(dbSetMock.Object);

            // Act
            repository.AddAsync(entity);

            // Assert
            dbSetMock.Verify(d => d.AddAsync(entity, CancellationToken.None), Times.Once);
        }

        [Fact]
        public void AddAsync_ShouldAddUserToDbSet()
        {
            // Arrange
            var entity = new User();
            var dbSetMock = new Mock<DbSet<User>>();
            var repository = new Repository<User>(dbSetMock.Object);

            // Act
            repository.AddAsync(entity);

            // Assert
            dbSetMock.Verify(d => d.AddAsync(entity, CancellationToken.None), Times.Once);
        }
    }
}