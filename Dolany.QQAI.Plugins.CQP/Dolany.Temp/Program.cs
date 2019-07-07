using System;
using System.IO;
using Dolany.Ai.Core.Ai.Game.Pet;
using Dolany.Database;

namespace Dolany.Temp
{
    class Program
    {
        static void Main(string[] args)
        {
            var records = MongoService<PetRecord>.Get(p => p.PicPath == "./images/Pet/Neptune/Default.jpg");
            foreach (var record in records)
            {
                var aimPath = $"./images/Custom/Pet/{record.QQNum}.jpg";
                File.Copy("c:/AI/Server/images/Pet/Neptune/Default.jpg", $"c:/AI/Server/images/Custom/Pet/{record.QQNum}.jpg");

                record.PicPath = aimPath;
                record.Update();
            }

            Console.WriteLine("Completed");
            Console.ReadKey();
        }
    }
}
