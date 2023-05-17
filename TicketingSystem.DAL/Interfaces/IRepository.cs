using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TicketingSystem.DAL.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        void AddAsync(TEntity entity);
        ValueTask<TEntity> FirstOrDefaultAsync(int id);
        Task<IEnumerable<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        params Expression<Func<TEntity, object>>[] includeProperties);

        void Update(TEntity entity);
        void Delete(TEntity entity);
    }
}
