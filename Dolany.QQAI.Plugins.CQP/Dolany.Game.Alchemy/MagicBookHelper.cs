using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            var lastChar = name.Last();
            if (lastChar >= '0' && lastChar <= '9')
            {
                level = int.Parse(lastChar.ToString());
                name = name.Substring(0, name.Length - 1);
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
    }

    public class MagicBookItemModel
    {
        public IMagicBook MagicBook { get; set; }

        public IAlItem Item { get; set; }

        public int Level { get; set; }
    }
}
