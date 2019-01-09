namespace Dolany.Database.Incantation
{
    using Microsoft.EntityFrameworkCore;

    public sealed class IncaDatabase : DbContext
    {
        private const string ConnectionString = @"Server=172_16_0_2\SQLEXPRESS;Database=C:\AIDB\AIDATABASE.MDF;Trusted_Connection=True;uid=sa;pwd=2160727;";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }

        public DbSet<IncaCharactor> IncaCharactor { get; set; }

        public DbSet<IncaMagic> IncaMagic { get; set; }

        public DbSet<IncaLine> IncaLine { get; set; }

        public DbSet<IncaEffect> IncaEffect { get; set; }
    }
}
