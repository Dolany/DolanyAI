using System;
using Dolany.Ai.Common;

namespace Dolany.Temp
{
    class Program
    {
        static void Main(string[] args)
        {
            var participates = new[]
            {
                "子衣不待", "Visca120", "沧月风语", "斯普林菲尔德太太", "Ilovepeaches",
                "舞舞逸", "鸢一啦", "月牙儿", "苏花半笔墨留白", "猹猹猹猹猹", "苏琳",
                "一只呆雪", "我是叮当啊好的", "凌天澄空", "萌希灵", "取名好难不想了 "
            };
            var randArray = CommonUtil.RandSort(participates);
            for (var i = 0; i < randArray.Length; i += 4)
            {
                Console.Write(randArray[i] + ',' + randArray[i + 1] + ',' + randArray[i + 2] + ',' + randArray[i + 3]);
                Console.WriteLine();
            }

            Console.WriteLine("Completed");
            Console.ReadKey();
        }
    }
}
