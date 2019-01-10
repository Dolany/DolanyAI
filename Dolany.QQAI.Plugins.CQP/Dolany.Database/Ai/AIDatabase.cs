namespace Dolany.Database.Ai
{
    using Microsoft.EntityFrameworkCore;

    public sealed class AIDatabase : DbContext
    {
        private const string ConnectionString = @"Server=172_16_0_2\SQLEXPRESS;Database=C:\AIDB\AIDATABASE.MDF;Trusted_Connection=True;uid=sa;pwd=2160727;";
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }

        public DbSet<ActiveOffGroups> ActiveOffGroups { get; set; }
        public DbSet<AIConfig> AIConfig { get; set; }
        public DbSet<AISeal> AISeal { get; set; }
        public DbSet<AlermClock> AlermClock { get; set; }
        public DbSet<AlertContent> AlertContent { get; set; }
        public DbSet<AlertRegistedGroup> AlertRegistedGroup { get; set; }
        public DbSet<BlackList> BlackList { get; set; }
        public DbSet<CharactorSetting> CharactorSetting { get; set; }
        public DbSet<DirtyWord> DirtyWord { get; set; }
        public DbSet<FortuneItem> FortuneItem { get; set; }
        public DbSet<Hello> Hello { get; set; }
        public DbSet<HolyLightBless> HolyLightBless { get; set; }
        public DbSet<KanColeGirlVoice> KanColeGirlVoice { get; set; }
        public DbSet<MemberRoleCache> MemberRoleCache { get; set; }
        public DbSet<MsgRecievedCache> MsgRecievedCache { get; set; }
        public DbSet<MsgSendCache> MsgSendCache { get; set; }
        public DbSet<PlusOneAvailable> PlusOneAvailable { get; set; }
        public DbSet<PraiseRec> PraiseRec { get; set; }
        public DbSet<RandomFortune> RandomFortune { get; set; }
        public DbSet<RepeaterAvailable> RepeaterAvailable { get; set; }
        public DbSet<Saying> Saying { get; set; }
        public DbSet<SayingSeal> SayingSeal { get; set; }
        public DbSet<TarotFortuneData> TarotFortuneData { get; set; }
        public DbSet<TarotFortuneRecord> TarotFortuneRecord { get; set; }
        public DbSet<TouhouCardRecord> TouhouCardRecord { get; set; }
        public DbSet<MsgCommand> MsgCommand { get; set; }
        public DbSet<MsgInformation> MsgInformation { get; set; }
        public DbSet<TempAuthorize> TempAuthorize { get; set; }
        public DbSet<MajFortune> MajFortune { get; set; }
        public DbSet<InitInfo> InitInfo { get; set; }
    }
}
