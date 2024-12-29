using ChineseFamilyRestaurant.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ChineseFamilyRestaurant.Models
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected ApplicationDbContext Context { get; }
        private readonly DbSet<T> DbSet;
        private readonly ILogger<Repository<T>> Logger;
        private readonly string PrimaryKeyName;

        public Repository(ApplicationDbContext context, ILogger<Repository<T>> logger)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            DbSet = Context.Set<T>();
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var key = Context.Model.FindEntityType(typeof(T))
                ?.FindPrimaryKey()
                ?.Properties.FirstOrDefault();
            PrimaryKeyName = key?.Name ?? throw new InvalidOperationException($"No primary key defined for {typeof(T).Name}");
        }

        public async Task AddAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            try
            {
                await DbSet.AddAsync(entity);
                await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error adding entity of type {EntityType}: {Message}", typeof(T).Name, ex.Message);
                throw new InvalidOperationException("An error occurred while adding the entity.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                // Find the entity by its primary key (ID)
                var entity = await DbSet.FindAsync(id);

                // If the entity is not found, return without doing anything
                if (entity == null)
                {
                    Logger.LogWarning("Attempt to delete entity of type {EntityType} with ID {Id} failed: entity not found.", typeof(T).Name, id);
                    return; // Or you can throw an exception if you prefer
                }

                //Remove the entity from the DbSet (mark it for deletion)
                DbSet.Remove(entity);

                //Save changes to persist the deletion in the database
                await Context.SaveChangesAsync();

                Logger.LogInformation("Entity of type {EntityType} with ID {Id} successfully deleted.", typeof(T).Name, id);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error deleting entity of type {EntityType} with ID {Id}: {Message}", typeof(T).Name, id, ex.Message);
                throw new InvalidOperationException("An error occurred while deleting the entity.", ex);
            }
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            try
            {
                return await DbSet.Where(predicate).ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error finding entities of type {EntityType}: {Message}", typeof(T).Name, ex.Message);
                throw new InvalidOperationException("An error occurred while finding entities.", ex);
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                return await DbSet.ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error retrieving all entities of type {EntityType}: {Message}", typeof(T).Name, ex.Message);
                throw new InvalidOperationException("An error occurred while retrieving entities.", ex);
            }
        }

        public async Task<T?> GetByIdAsync(int id, QueryOptions<T> options)
        {
            try
            {
                IQueryable<T> query = DbSet;

                if (options.HasWhere)
                    query = query.Where(options.Where);

                if (options.HasOrderBy)
                    query = query.OrderBy(options.OrderBy);

                foreach (string include in options.GetIncludes())
                    query = query.Include(include);

                return await query.SingleOrDefaultAsync(e => EF.Property<int>(e, PrimaryKeyName) == id);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error retrieving entity of type {EntityType} with ID {Id}: {Message}", typeof(T).Name, id, ex.Message);
                throw new InvalidOperationException("An error occurred while retrieving the entity.", ex);
            }
        }

        public async Task UpdateAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            try
            {
                DbSet.Update(entity);
                await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error updating entity of type {EntityType}: {Message}", typeof(T).Name, ex.Message);
                throw new InvalidOperationException("An error occurred while updating the entity.", ex);
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            try
            {
                return await DbSet.AnyAsync(e => EF.Property<int>(e, PrimaryKeyName) == id);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error checking existence of entity of type {EntityType} with ID {Id}: {Message}", typeof(T).Name, id, ex.Message);
                throw new InvalidOperationException("An error occurred while checking the entity's existence.", ex);
            }
        }
    }
}
