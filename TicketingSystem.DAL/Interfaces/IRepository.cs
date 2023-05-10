using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketingSystem.DAL.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        void AddAsync(TEntity entity);
        ValueTask<TEntity> FirstOrDefaultAsync(int id);
        IQueryable<TEntity> GetAll();
        void Update(TEntity entity);
        void Delete(TEntity entity);
    }
}
