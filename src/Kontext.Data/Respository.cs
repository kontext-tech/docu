using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Kontext.Data
{
    /// <summary>
    /// Generic repository
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext context;
        protected readonly DbSet<TEntity> entities;

        public IQueryable<TEntity> Entities => entities;

        public Repository(DbContext context)
        {
            this.context = context;
            entities = context.Set<TEntity>();
        }

        public virtual void Add(TEntity entity)
        {
            entities.Add(entity);
        }

        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            this.entities.AddRange(entities);
        }


        public virtual void Update(TEntity entity)
        {
            entities.Update(entity);
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities)
        {
            this.entities.UpdateRange(entities);
        }



        public virtual void Remove(TEntity entity)
        {
            entities.Remove(entity);
        }

        public virtual void RemoveRange(IEnumerable<TEntity> entities)
        {
            this.entities.RemoveRange(entities);
        }


        public virtual int Count()
        {
            return entities.Count();
        }


        public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return entities.Where(predicate);
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await entities.Where(predicate).ToListAsync();
        }

        public async Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await entities.Where(predicate).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> predicate, params string[] includeProperties)
        {
            var query = entities.Where(predicate);
            if (includeProperties != null)
            {
                foreach (var prop in includeProperties)
                {
                    query = query.Include(prop);
                }
            }
            return await query.ToListAsync();
        }

        public async Task<TEntity> QueryOneAsync(Expression<Func<TEntity, bool>> predicate, params string[] includeProperties)
        {
            var query = entities.Where(predicate);
            if (includeProperties != null)
            {
                foreach (var prop in includeProperties)
                {
                    query = query.Include(prop);
                }
            }
            return await query.FirstOrDefaultAsync();
        }

        public virtual TEntity GetSingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return entities.SingleOrDefault(predicate);
        }

        public virtual TEntity Get(int id)
        {
            return entities.Find(id);
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return entities.ToList();
        }
    }
}

