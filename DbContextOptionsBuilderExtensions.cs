using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Question
{
    public static class DbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder UseOrderByAll(
            this DbContextOptionsBuilder optionsBuilder)
        {
            var extension = (DbContextOptionsExtension)GetOrCreateExtension(optionsBuilder);
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            return optionsBuilder;
        }

        private static DbContextOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.Options.FindExtension<DbContextOptionsExtension>()
               ?? new DbContextOptionsExtension();
    }
}