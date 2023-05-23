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
    public class GetObjectsTests
    {
        [Fact]
        public async Task GetAsync_ShouldReturnEntitiesWithIdAndTotalAmountProperties()
        {
            // Arrange
            var entities = new List<Cart>
        {
            new Cart { Id = 1, TotalAmount = 10 },
            new Cart { Id = 2, TotalAmount = 20 },
            new Cart { Id = 3, TotalAmount = 30 }
        };
            using (var context = new TicketingSystemContext())
            {
                await context.AddRangeAsync(entities);
                await context.SaveChangesAsync();
            }
            using (var context = new TicketingSystemContext())
            {
                var repository = new Repository<Cart>(context.Set<Cart>());

                // Act
                var result = await repository.GetAsync();

                // Assert
                Assert.Equal(entities.Count, result.Count());
                foreach (var entity in entities)
                {
                    var resultEntity = result.FirstOrDefault(e => e.Id == entity.Id);
                    Assert.NotNull(resultEntity);
                    Assert.Equal(entity.TotalAmount, resultEntity.TotalAmount);
                }
            }
        }
    }
}
