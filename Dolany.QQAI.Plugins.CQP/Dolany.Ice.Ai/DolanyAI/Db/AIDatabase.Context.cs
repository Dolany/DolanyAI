﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Dolany.Ice.Ai.DolanyAI.Db
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class AIDatabase : DbContext
    {
        public AIDatabase()
            : base("name=AIDatabase")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AISeal> AISeal { get; set; }
        public virtual DbSet<AlermClock> AlermClock { get; set; }
        public virtual DbSet<AlertContent> AlertContent { get; set; }
        public virtual DbSet<AlertRegistedGroup> AlertRegistedGroup { get; set; }
        public virtual DbSet<BlackList> BlackList { get; set; }
        public virtual DbSet<CharactorSetting> CharactorSetting { get; set; }
        public virtual DbSet<DirtyWord> DirtyWord { get; set; }
        public virtual DbSet<FishingRecord> FishingRecord { get; set; }
        public virtual DbSet<FishItem> FishItem { get; set; }
        public virtual DbSet<Hello> Hello { get; set; }
        public virtual DbSet<MemberRoleCache> MemberRoleCache { get; set; }
        public virtual DbSet<MsgRecievedCache> MsgRecievedCache { get; set; }
        public virtual DbSet<MsgSendCache> MsgSendCache { get; set; }
        public virtual DbSet<PlusOneAvailable> PlusOneAvailable { get; set; }
        public virtual DbSet<RandomFortune> RandomFortune { get; set; }
        public virtual DbSet<RepeaterAvailable> RepeaterAvailable { get; set; }
        public virtual DbSet<SayingSeal> SayingSeal { get; set; }
        public virtual DbSet<FortuneItem> FortuneItem { get; set; }
        public virtual DbSet<KanColeGirlVoice> KanColeGirlVoice { get; set; }
        public virtual DbSet<PraiseRec> PraiseRec { get; set; }
        public virtual DbSet<Saying> Saying { get; set; }
        public virtual DbSet<TohouSign> TohouSign { get; set; }
    }
}
