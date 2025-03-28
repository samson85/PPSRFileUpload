using System.Threading;
using System.Threading.Tasks;

namespace Data.Model
{
    public abstract class BaseRepository : IRepository
    {
        protected readonly AppDbContext _db;

        public BaseRepository(AppDbContext db)
        {
            _db = db;
        }

        public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
