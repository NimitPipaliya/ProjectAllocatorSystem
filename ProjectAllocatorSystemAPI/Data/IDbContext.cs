using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;

namespace ProjectAllocatorSystemAPI.Data
{
    public interface IDbContext : IDisposable
    {
        DatabaseFacade Database { get; }
        ChangeTracker ChangeTracker { get; }
        IModel Model { get; }
        EntityEntry Add(object entity);
        EntityState GetEntryState<TEntity>(TEntity entity) where TEntity : class;
        void SetEntryState<TEntity>(TEntity entity, EntityState entityState) where TEntity : class;
        EntityEntry<TEntity> Add<TEntity>(TEntity entity) where TEntity : class;
        ValueTask<EntityEntry> AddAsync(object entity, CancellationToken cancellationToken = default);
        ValueTask<EntityEntry<TEntity>> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class;
        void AddRange(params object[] entities);
        Task AddRangeAsync(IEnumerable<object> entities, CancellationToken cancellationToken = default);
        Task AddRangeAsync(params object[] entities);
        EntityEntry<TEntity> Attach<TEntity>(TEntity entity) where TEntity : class;
        EntityEntry Attach(object entity);
        void AttachRange(params object[] entities);
        void AttachRange(IEnumerable<object> entities);
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        EntityEntry Entry(object entity);
        bool Equals(object obj);
        object Find(Type entityType, params object[] keyValues);
        TEntity Find<TEntity>(params object[] keyValues) where TEntity : class;
        ValueTask<TEntity> FindAsync<TEntity>(params object[] keyValues) where TEntity : class;
        ValueTask<object> FindAsync(Type entityType, object[] keyValues, CancellationToken cancellationToken);
        ValueTask<TEntity> FindAsync<TEntity>(object[] keyValues, CancellationToken cancellationToken) where TEntity : class;
        ValueTask<object> FindAsync(Type entityType, params object[] keyValues);
        int GetHashCode();
        EntityEntry Remove(object entity);
        EntityEntry<TEntity> Remove<TEntity>(TEntity entity) where TEntity : class;
        void RemoveRange(IEnumerable<object> entities);
        void RemoveRange(params object[] entities);
        int SaveChanges(bool acceptAllChangesOnSuccess);
        int SaveChanges();
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        string ToString();
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        EntityEntry Update(object entity);
        EntityEntry<TEntity> Update<TEntity>(TEntity entity) where TEntity : class;
        void UpdateRange(params object[] entities);
        void UpdateRange(IEnumerable<object> entities);
    }
}
