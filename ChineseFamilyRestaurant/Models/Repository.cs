using ChineseFamilyRestaurant.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace ChineseFamilyRestaurant.Models
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected ApplicationDbContext _context { get; set; }
        private DbSet<T> _dbSet { get; set; }
        private readonly ILogger<Repository<T>> _logger;
        private readonly string _primaryKeyName;

        public Repository(ApplicationDbContext context, ILogger<Repository<T>> logger)
        {
            _context = context;
            _dbSet = context.Set<T>();
            _logger = logger;

            var key = _context.Model.FindEntityType(typeof(T)).FindPrimaryKey()?.Properties.FirstOrDefault();
            _primaryKeyName = key?.Name ?? throw new InvalidOperationException($"No primary key defined for {typeof(T).Name}");
        }

        public async Task AddAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null) return;

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id, QueryOptions<T> options)
        {
            try
            {
                IQueryable<T> query = _dbSet;

                if (options.HasWhere)
                    query = query.Where(options.Where);

                if (options.HasOrderBy)
                    query = query.OrderBy(options.OrderBy);

                foreach (string include in options.GetIncludes())
                    query = query.Include(include);

                return await query.SingleOrDefaultAsync(e => EF.Property<int>(e, _primaryKeyName) == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetByIdAsync for entity {typeof(T).Name}: {ex.Message}");
                throw new InvalidOperationException($"An error occurred while retrieving the entity: {ex.Message}", ex);
            }
        }

        public async Task UpdateAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
