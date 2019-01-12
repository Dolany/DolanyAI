using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dolany.Database;
using Dolany.Database.Ai;

namespace DbMigrate
{
    class Program
    {
        static void Main(string[] args)
        {
            Migrate<ActiveOffGroups>();
            Migrate<AIConfig>();
            Migrate<AISeal>();
            Migrate<AlermClock>();
            Migrate<AlertContent>();
            Migrate<AlertRegistedGroup>();
            Migrate<BlackList>();
            Migrate<CharactorSetting>();
            Migrate<DirtyWord>();
            Migrate<FortuneItem>();
            Migrate<Hello>();
            Migrate<HolyLightBless>();
            Migrate<InitInfo>();
            Migrate<KanColeGirlVoice>();
            Migrate<MajFortune>();
            Migrate<MemberRoleCache>();
            Migrate<MsgCommand>();
            Migrate<MsgInformation>();
            Migrate<MsgRecievedCache>();
            Migrate<MsgSendCache>();
            Migrate<PlusOneAvailable>();
            Migrate<PraiseRec>();
            Migrate<RandomFortune>();
            Migrate<RepeaterAvailable>();
            Migrate<Saying>();
            Migrate<SayingSeal>();
            Migrate<TarotFortuneData>();
            Migrate<TarotFortuneRecord>();
            Migrate<TempAuthorize>();
            Migrate<TouhouCardRecord>();

            Console.ReadKey();
        }

        private static void Migrate<T>() where  T: BaseEntity
        {
            try
            {
                using (var sqlDb = new AIDatabase())
                {
                    var type = typeof(AIDatabase);
                    var prop = type.GetProperty(typeof(T).Name);
                    var values = prop.GetValue(sqlDb) as IEnumerable<T>;
                    MongoService<T>.InsertMany((values ?? throw new InvalidOperationException()).ToList());
                }

                Console.WriteLine($"{typeof(T).Name} Completed");

                Thread.Sleep(1000);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(typeof(T).Name);
            }
        }
    }
}
