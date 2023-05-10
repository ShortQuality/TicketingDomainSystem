using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TicketingSystem.DAL.Interfaces;

namespace TicketingSystem.DAL.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {

        private DbSet<TEntity> _entities { get; }

        public Repository(DbSet<TEntity> entities)
        {
            _entities = entities;
        }

        public virtual void AddAsync(TEntity entity)
        {
            _entities.AddAsync(entity);
        }

        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = _entities;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual ValueTask<TEntity> FirstOrDefaultAsync(int id)
        {
            return _entities.FindAsync(id);
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return _entities.AsQueryable();
        }

        public virtual void Update(TEntity entity)
        {
            _entities.Update(entity);
        }
        public virtual void Delete(TEntity entity)
        {
            _entities.Remove(entity);
        }
    }
}
