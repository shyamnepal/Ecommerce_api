using DataAcess.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ShoesRepository.GenreicRepo
{
    public class GenricRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly ShoesEcommerceContext _context;
        public GenricRepository(ShoesEcommerceContext context)
        {
            _context = context;
            
        }
        public TEntity Add(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public async Task AddRangeAsync(List<TEntity> entities)
        {
            try
            {
                 _context.Set<TEntity>().AddRange(entities);
                 _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                // Handle the exception as needed (e.g., rethrow, return a result, etc.)
                throw;
            }
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate,
                                    params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>().Where(predicate);

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query.ToList();
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _context.Set<TEntity>().ToList();
        }

        public TEntity GetById(int id)
        {
            return _context.Set<TEntity>().Find(id);
        }

        public void Remove(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
            _context.SaveChanges();
        }

        public void Update(TEntity entity)
        {
            var entry = _context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                _context.Set<TEntity>().Attach(entity);
                entry.State = EntityState.Modified;
            }
            _context.SaveChanges();
        }
    }
}
