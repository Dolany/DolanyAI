namespace Dolany.Database.Touhou
{
    using Microsoft.EntityFrameworkCore;

    public sealed class GameDatabase : DbContext
    {
        private const string ConnectionString = @"Server=172_16_0_2\SQLEXPRESS;Database=C:\AIDB\AIDATABASE.MDF;Trusted_Connection=True;uid=sa;pwd=2160727;";
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }

        public DbSet<PlayerInfo> PlayerInfo { get; set; }
        public DbSet<TouhouCard> TouhouCard { get; set; }
        public DbSet<HeroLines> HeroLines { get; set; }
    }
}
