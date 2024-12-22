using Microsoft.EntityFrameworkCore;
using TequilasRestaurant.Data;

namespace TequilasRestaurant.Models
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected ApplicationDbContext _context { get; set; }
        private DbSet<T> _dbSet { get; set; }

        public Repository(ApplicationDbContext context)
        {
            _context = context; // Used to create a conection to the Db
            _dbSet = context.Set<T>(); // Used to specify our tables
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id, QueryOptions<T> options)
        {
            IQueryable<T> query = _dbSet;
            if (options.HasWhere)
            {
                query = query.Where(options.Where);
            }

            if(options.HasOrderBy)
            {
                query = query.OrderBy(options.OrderBy);
            }

            foreach(string include in options.GetIncludes())
            {
                query = query.Include(include);
            }

            //Sets the primary key
            var key = _context.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties.FirstOrDefault();
            //Sets the Name from the primary key
            string primaryKeyName = key?.Name;
            Console.Write("primaryKey = " + primaryKeyName);
            //Returns a link query that equals to the given id
            //In this case, you'd click on the Details link and that would send in the key for you
            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, primaryKeyName) == id);
        }

        public Task AddAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
