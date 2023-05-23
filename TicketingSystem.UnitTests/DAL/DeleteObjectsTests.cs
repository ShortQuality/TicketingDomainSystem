using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketingSystem.DAL;
using TicketingSystem.DAL.Entities;
using TicketingSystem.DAL.Repositories;

namespace TicketingSystem.UnitTests.DAL
{
    public class DeleteObjectsTests
    {
        [Fact]
        public async Task DeleteAsync_Removes_Cart_From_Database()
        {
            // Arrange
            var cartId = 1;
            var totalAmount = 100;

            // Add a new Cart entity to the in-memory database
            using (var context = new TicketingSystemContext())
            {
                context.Carts.Add(new Cart { Id = cartId, TotalAmount = totalAmount });
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new TicketingSystemContext())
            {
                var repository = new Repository<Cart>(context.Set<Cart>());
                var cartToDelete = new Cart { Id = cartId };
                repository.Delete(cartToDelete);
            }

            // Assert
            using (var context = new TicketingSystemContext())
            {
                var cart = await context.Carts.FindAsync(cartId);
                Assert.Null(cart);
            }
        }
    }
}
