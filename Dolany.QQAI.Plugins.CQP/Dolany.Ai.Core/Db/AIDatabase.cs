using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Ai.Core.Db
{
    using Microsoft.EntityFrameworkCore;
    using static Dolany.Ai.Core.Common.Utility;

    public sealed partial class AIDatabase : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(GetConfig("ConnectionString"));
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
    }
}
