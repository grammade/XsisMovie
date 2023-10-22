using Microsoft.EntityFrameworkCore;
using XsisMovie.Entities;

namespace XsisMovie.Persistence {
    public class Context : DbContext, IContext {
        public Context(DbContextOptions<Context> options)
            : base(options) {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Password> Passwords { get; set; }
        public DbSet<Movie> Movies { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => base.SaveChangesAsync(cancellationToken);
    }
}
