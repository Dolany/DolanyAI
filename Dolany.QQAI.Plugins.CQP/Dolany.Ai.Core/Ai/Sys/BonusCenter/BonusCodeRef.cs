using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Database;

namespace Dolany.Ai.Core.Ai.Sys.BonusCenter
{
    public class BonusCodeRef : DbBaseEntity
    {
        public string Code { get; set; }
        public string Ref { get; set; }
        public DateTime? ExpiryTime { get; set; }
        public bool IsDrawed { get; set; }

        public static BonusCodeRef GetRandBonusCode(string Ref)
        {
            var codeRef = MongoService<BonusCodeRef>.GetOnly(p => p.Ref == Ref && !p.IsDrawed);
            return codeRef ?? GenerateCodes(Ref).First();
        }

        public static BonusCodeRef Get(string Code)
        {
            return MongoService<BonusCodeRef>.GetOnly(p => p.Code == Code);
        }

        public void Update()
        {
            MongoService<BonusCodeRef>.Update(this);
        }

        public void Remove()
        {
            MongoService<BonusCodeRef>.Delete(this);
        }

        private static List<BonusCodeRef> GenerateCodes(string Ref)
        {
            var list = new List<BonusCodeRef>();
            for (var i = 0; i < 10; i++)
            {
                list.Add(new BonusCodeRef()
                {
                    Code = Guid.NewGuid().ToString("N"),
                    Ref = Ref
                });
            }

            MongoService<BonusCodeRef>.InsertMany(list);
            return list;
        }
    }
}
