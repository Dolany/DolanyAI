using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database.Ai;
using Dolany.Game.OnlineStore;

namespace Dolany.Game.Alchemy
{
    public class AlCombineNeed
    {
        public Dictionary<string, int> AlItemNeed { get; set; } = new Dictionary<string, int>();

        public Dictionary<string, int> MagicDirtNeed { get; set; } = new Dictionary<string, int>();

        public Dictionary<string, int> NormalItemNeed { get; set; } = new Dictionary<string, int>();

        public int GoldsNeed { get; set; }

        public bool CheckNeed(AlPlayer player, OSPerson osPerson, DriftItemRecord record, out string msg)
        {
            msg = "";
            var msgList = new List<string>();
            var isEnough = true;

            if (!AlItemNeed.IsNullOrEmpty())
            {
                foreach (var alitemNeed in AlItemNeed)
                {
                    var (name, count) = alitemNeed;
                    if (!player.AlItems.Keys.Contains(name))
                    {
                        msgList.Add($"{name}*{count}(0)");
                        isEnough = false;
                    }
                    else
                    {
                        msgList.Add($"{name}*{count}({player.AlItems[name]})");
                        if (count > player.AlItems[name])
                        {
                            isEnough = false;
                        }
                    }
                }
            }

            if (!MagicDirtNeed.IsNullOrEmpty())
            {
                foreach (var dirtNeed in MagicDirtNeed)
                {
                    var (name, count) = dirtNeed;
                    if (!player.MagicDirt.Keys.Contains(name))
                    {
                        msgList.Add($"{name}*{count}(0)");
                        isEnough = false;
                    }
                    else
                    {
                        msgList.Add($"{name}*{count}({player.MagicDirt[name]})");
                        if (count > player.MagicDirt[name])
                        {
                            isEnough = false;
                        }
                    }
                }
            }

            if (!NormalItemNeed.IsNullOrEmpty())
            {
                foreach (var item in NormalItemNeed)
                {
                    var (name, count) = item;
                    var di = record.ItemCount.FirstOrDefault(i => i.Name == name);
                    if (di == null)
                    {
                        msgList.Add($"{name}*{count}(0)");
                        isEnough = false;
                    }
                    else
                    {
                        msgList.Add($"{name}*{count}({di.Count})");
                        if (count > di.Count)
                        {
                            isEnough = false;
                        }
                    }
                }
            }

            if (GoldsNeed > 0)
            {
                if (osPerson.Golds < GoldsNeed)
                {
                    isEnough = false;
                }

                msgList.Add($"金币{GoldsNeed}({osPerson.Golds})");
            }

            msg += string.Join("，", msgList);
            return isEnough;
        }

        public void DoConsume(AlPlayer player, OSPerson osPerson, DriftItemRecord record)
        {
            foreach (var alitem in AlItemNeed)
            {
                var (key, value) = alitem;
                player.ItemConsume(key, value);
            }

            foreach (var magicDirt in MagicDirtNeed)
            {
                var (key, value) = magicDirt;
                player.MagicDirtConsume(key, value);
            }

            foreach (var nitem in NormalItemNeed)
            {
                var (key, value) = nitem;
                record.ItemConsume(key, value);
            }

            osPerson.Golds -= GoldsNeed;
        }
    }
}
