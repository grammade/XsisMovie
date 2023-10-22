using Microsoft.EntityFrameworkCore;
using XsisMovie.Entities;

namespace XsisMovie.Persistence {
    public interface IContext {
        DbSet<User> Users { get; set; }
        DbSet<Password> Passwords { get; set; }
        DbSet<Movie> Movies { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
