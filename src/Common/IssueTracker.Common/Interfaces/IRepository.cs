using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IssueTracker.Common.Interfaces
{
    public interface IRepository<T>
    {
        Task<T> Insert(T model);
        Task<T> AddAsync(T model);
        Task InsertRange(IEnumerable<T> list);
        Task<T> Update(T model);
        Task<T> UpdateAsync(T model);
        Task<int> UpdateRange(IEnumerable<T> models);
        void UpdateAll(IEnumerable<T> models);
        Task Delete(Guid id);
        Task DeleteAsync(Guid id);
        Task DeleteRange(IEnumerable<T> models);
        void DeleteAll(IEnumerable<T> models);
        Task<T> Get(Guid id);
        Task<T> Get(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> List();
        Task<IEnumerable<T>> List(Expression<Func<T, bool>> predicate);
        Task<int> Count();
        Task<int> Count(Expression<Func<T, bool>> predicate);
        Task<bool> Any(Expression<Func<T, bool>> predicate);
        void ApplySelectedValues(T entity, params Expression<Func<T, object>>[] properties);
    }
}