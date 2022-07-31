using BuildSharp.Tools.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace BuildSharp.Tools.Services;

public class Repository<TSource, TDbContext> : IRepository<TSource> 
    where TDbContext : DbContext, 
    new() where TSource : class
{
    private readonly TDbContext dbContext;
    private readonly DbSet<TSource> dbSet;

    public Repository(TDbContext dbContext)
    {
        this.dbContext = dbContext;
        this.dbSet = this.dbContext.Set<TSource>();
    }
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

    public virtual Task<TSource> FirstOrDefaultAsync(
        Expression<Func<TSource, bool>> predicate = null, 
        IList<string> includes = null,
        bool isTracking = false) =>
            this.Where(predicate, includes, isTracking)
                .FirstOrDefaultAsync();

    public virtual async Task<TSource> AddAsync(
        TSource entity)
    {
        var entry = await this.dbSet.AddAsync(entity);
        await this.dbContext.SaveChangesAsync();

        return entry.Entity;
    }

    public virtual async Task AddRangeAsync(
        IList<TSource> entities)
    {
        await this.dbSet.AddRangeAsync(entities);
        await this.dbContext.SaveChangesAsync();
    }

    public virtual async Task<TSource> UpdateAsync(
        TSource entity)
    {
        var entry = this.dbSet.Update(entity);
        await this.dbContext.SaveChangesAsync();

        return entry.Entity;
    }

    public virtual async Task<TSource> RemoveAsync(
        TSource entity)
    {
        var entry = this.dbSet.Remove(entity);
        await this.dbContext.SaveChangesAsync();

        return entry.Entity;
    }

    public virtual async Task RemoveRangeAsync(
        IList<TSource> entities)
    {
        this.dbSet.RemoveRange(entities);
        await this.dbContext.SaveChangesAsync();
    }   
}