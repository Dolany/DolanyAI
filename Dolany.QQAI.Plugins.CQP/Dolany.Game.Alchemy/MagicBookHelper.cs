using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dolany.Ai.Common;
using Dolany.Game.Alchemy.MagicBook;

namespace Dolany.Game.Alchemy
{
    public class MagicBookHelper
    {
        public static MagicBookHelper Instance { get; } = new MagicBookHelper();

        private readonly List<IMagicBook> MagicBooks;

        private MagicBookHelper()
        {
            var assembly = Assembly.GetAssembly(typeof(IMagicBook));
            MagicBooks = assembly.GetTypes()
                .Where(type => type.IsSubclassOf(typeof(IMagicBook)))
                .Where(type => type.FullName != null)
                .Select(type => assembly.CreateInstance(type.FullName) as IMagicBook).ToList();
        }

        public MagicBookItemModel FindItem(string name)
        {
            var level = 1;
            var strs = name.Split(new[] {"lv"}, StringSplitOptions.RemoveEmptyEntries);
            if (strs.Length >= 2)
            {
                name = strs[0];
                if (!int.TryParse(strs[1], out level) || level <= 0)
                {
                    return null;
                }
            }

            var magicBook = MagicBooks.FirstOrDefault(mb => mb.Items.Any(i => i.Name == name));

            var item = magicBook?.Items.FirstOrDefault(i => i.Name == name && i.MaxLevel >= level);
            if (item == null)
            {
                return null;
            }

            return new MagicBookItemModel()
            {
                MagicBook = magicBook,
                Item = item,
                Level = level
            };
        }

        public List<IAlItem> GetExamQuestions(string bookName, int count)
        {
            var book = MagicBooks.First(mb => mb.Name == bookName);
            var itemArray = book.Items.ToArray();
            itemArray = CommonUtil.RandSort(itemArray);

            return itemArray.Take(count).ToList();
        }
    }

    public class MagicBookItemModel
    {
        public IMagicBook MagicBook { get; set; }

        public IAlItem Item { get; set; }

        public int Level { get; set; }
    }
}
