using System;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database;
using Dolany.Database.Ai;
using Dolany.WorldLine.Standard.Ai.Game.SignIn;

namespace Dolany.Temp
{
    class Program
    {
        static void Main(string[] args)
        {
            var r = MongoService<SignInSuccessiveRecord>.Get();
            MongoService<SignInSuccessiveRecord>.DeleteMany(r);

            var oldSigns = MongoService<SignInPersonRecord>.Get();
            foreach (var oldSign in oldSigns.Where(oldSign => !oldSign.GroupInfos.IsNullOrEmpty()))
            {
                foreach (var (groupNum, info) in oldSign.GroupInfos)
                {
                    if (info.LastSignInDate == null)
                    {
                        continue;
                    }

                    var newSign = new SignInSuccessiveRecord()
                    {
                        GroupNum = long.Parse(groupNum),
                        QQNum = oldSign.QQNum,
                        EndDate = info.LastSignInDate.Value.ToLocalTime().Date,
                        StartDate = info.LastSignInDate.Value.ToLocalTime().Date.AddDays(info.SuccessiveDays - 1)
                    };
                    MongoService<SignInSuccessiveRecord>.Insert(newSign);
                }
            }

            Console.WriteLine("Completed");
            Console.ReadKey();
        }
    }
}
