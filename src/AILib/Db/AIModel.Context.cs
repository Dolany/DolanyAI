﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace AILib.Db
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
    
        public virtual DbSet<FishingRecord> FishingRecord { get; set; }
        public virtual DbSet<FishItem> FishItem { get; set; }
        public virtual DbSet<PlusOneAvailable> PlusOneAvailable { get; set; }
        public virtual DbSet<RandomFortune> RandomFortune { get; set; }
        public virtual DbSet<RepeaterAvailable> RepeaterAvailable { get; set; }
        public virtual DbSet<CharactorSetting> CharactorSetting { get; set; }
        public virtual DbSet<SayingSeal> SayingSeal { get; set; }
        public virtual DbSet<AlermClock> AlermClock { get; set; }
    }
}
