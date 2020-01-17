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
            var today = DateTime.Today;
            var r = MongoService<SignInSuccessiveRecord>.Get(p => p.EndDate > today);
            MongoService<SignInSuccessiveRecord>.DeleteMany(r);

            Console.WriteLine("Completed");
            Console.ReadKey();
        }
    }
}
