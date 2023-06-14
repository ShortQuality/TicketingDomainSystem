﻿using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TicketingSystem.DAL.Entities;
using TicketingSystem.DAL.Interfaces;

namespace TicketingSystem.DAL.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {

        private DbSet<TEntity> _entities { get; }
        private TicketingSystemContext _dbContext;

        public Repository(TicketingSystemContext dbContext, DbSet<TEntity> entities)
        {
            _dbContext = dbContext;
            _entities = entities;
        }

        public virtual void AddAsync(TEntity entity)
        {
            _entities.AddAsync(entity);
        }

        public async Task<IEnumerable<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = _entities;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return await query.ToListAsync();
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

        public async Task<bool> LockEntityAsync(int id)
        {
            var entity = await FirstOrDefaultAsync(id);

            if (entity == null)
            {
                return false;
            }

            _dbContext.Entry(entity).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task UnlockEntityAsync(int id)
        {
            var entity= await FirstOrDefaultAsync(id);

            if (entity!= null)
            {
                _dbContext.Entry(entity).State = EntityState.Detached;
            }
        }

    }
}
