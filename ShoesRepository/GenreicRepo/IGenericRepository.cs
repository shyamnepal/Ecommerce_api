using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ShoesRepository.GenreicRepo
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        TEntity GetById(int id);
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate,
                               params Expression<Func<TEntity, object>>[] includeProperties);
        TEntity Add(TEntity entity);
        Task AddRangeAsync(List<TEntity> entities);
        void Update(TEntity entity);
        void Remove(TEntity entity);
    }
}
