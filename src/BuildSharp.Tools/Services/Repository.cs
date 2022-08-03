using BuildSharp.Tools.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BuildSharp.Tools.Services;

public abstract class Repository<TSource, TDbContext> : IRepository<TSource> 
    where TDbContext : DbContext, 
    new() where TSource : class
{
    protected readonly TDbContext dbContext;
    protected readonly DbSet<TSource> dbSet;
    public Repository(TDbContext dbContext)
    {
        this.dbContext = dbContext;
        this.dbSet = this.dbContext.Set<TSource>();
    }
    
    /// <summary>
    /// Get all rows using predicate
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="includes"></param>
    /// <param name="isTracking"></param>
    /// <returns></returns>
    public virtual IQueryable<TSource> Where(
        Expression<Func<TSource, bool>> predicate = null, 
        IList<string> includes = null,
        bool isTracking = false)
    {
        IQueryable<TSource> entities = this.dbSet;

        if (predicate is not null)
            entities = entities.Where(predicate);

        if (includes is not null)
            foreach (var include in includes)
                entities.Include(include);

        return isTracking ?
            entities :
            entities.AsNoTracking();
    }

    /// <summary>
    /// Get first element of collection with predicate
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="includes"></param>
    /// <param name="isTracking"></param>
    /// <returns></returns>
    public virtual Task<TSource> FirstOrDefaultAsync(
        Expression<Func<TSource, bool>> predicate = null, 
        IList<string> includes = null,
        bool isTracking = false) =>
            this.Where(predicate, includes, isTracking)
                .FirstOrDefaultAsync();

    /// <summary>
    /// Add element 
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public virtual async Task<TSource> AddAsync(
        TSource entity)
    {
        var entry = await this.dbSet.AddAsync(entity);
        await this.dbContext.SaveChangesAsync();

        return entry.Entity;
    }

    /// <summary>
    /// Add elements
    /// </summary>
    /// <param name="entities"></param>
    public virtual async Task AddRangeAsync(
        IList<TSource> entities)
    {
        await this.dbSet.AddRangeAsync(entities);
        await this.dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Update element
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public virtual async Task<TSource> UpdateAsync(
        TSource entity)
    {
        var entry = this.dbSet.Update(entity);
        await this.dbContext.SaveChangesAsync();

        return entry.Entity;
    }
    
    /// <summary>
    /// Remove element
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public virtual async Task<TSource> RemoveAsync(
        TSource entity)
    {
        var entry = this.dbSet.Remove(entity);
        await this.dbContext.SaveChangesAsync();

        return entry.Entity;
    }

    /// <summary>
    /// Remove range of elements from collection
    /// </summary>
    /// <param name="entities"></param>
    public virtual async Task RemoveRangeAsync(
        IList<TSource> entities)
    {
        this.dbSet.RemoveRange(entities);
        await this.dbContext.SaveChangesAsync();
    }   
}