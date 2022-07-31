using System.Linq.Expressions;

namespace BuildSharp.Tools.Interfaces;

public interface IRepository<TSource>
{
    IQueryable<TSource> Where(
        Expression<Func<TSource, bool>> predicate = null,
        IList<string> includes = null,
        bool isTracking = false);

    Task<TSource> FirstOrDefaultAsync(
        Expression<Func<TSource, bool>> predicate = null,
        IList<string> includes = null,
        bool isTracking = false);

    Task<TSource> AddAsync(TSource entity);
    Task AddRangeAsync(IList<TSource> entities);
    Task<TSource> UpdateAsync(TSource entity);
    Task RemoveRangeAsync(IList<TSource> entities);
    Task<TSource> RemoveAsync(TSource entity);
}

