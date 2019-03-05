using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database;
using Dolany.Database.Ai;

namespace Dolany.Game.Alchemy
{
    public class AlPlayer : BaseEntity
    {
        public long QQNum { get; set; }

        public int Level { get; set; }

        public int Exp { get; set; }

        public long MaxHP { get; set; }

        public long CurHP { get; set; }

        public bool IsAlive { get; set; }

        public DateTime RebornTime { get; set; }

        public Dictionary<string, int> MagicDirt { get; set; }

        public List<string> MagicBookAvailable { get; set; }

        public string MagicBookLearning { get; set; }

        public List<string> MagicBookLearned { get; set; }

        public Dictionary<string, int> AlItems { get; set; }

        public static AlPlayer GetPlayer(long QQNum)
        {
            var player = MongoService<AlPlayer>.Get(p => p.QQNum == QQNum).FirstOrDefault();
            if (player != null)
            {
                if (!player.IsAlive && player.RebornTime.ToLocalTime() < DateTime.Now)
                {
                    player.IsAlive = true;
                }

                return player;
            }

            player = new AlPlayer()
            {
                QQNum = QQNum,
                MaxHP = 50,
                CurHP = 50,
                MagicDirt = new Dictionary<string, int>(),
                AlItems = new Dictionary<string, int>(),
                IsAlive = true,
                RebornTime = DateTime.Now,
                MagicBookAvailable = new List<string>(),
                MagicBookLearning = "初级炼金手册",
                MagicBookLearned = new List<string>()
            };

            MongoService<AlPlayer>.Insert(player);

            return player;
        }

        public void Update()
        {
            MagicDirt.Remove(d => d == 0);
            AlItems.Remove(i => i == 0);

            MongoService<AlPlayer>.Update(this);
        }

        public bool CheckMatiral(AlCombineNeed combineNeed, DriftItemRecord itemRecord, out string msg)
        {
            msg = "需要材料：";
            var msgList = new List<string>();
            var isEnough = true;

            if (!combineNeed.AlItemNeed.IsNullOrEmpty())
            {
                foreach (var alitemNeed in combineNeed.AlItemNeed)
                {
                    var (name, count) = alitemNeed;
                    if (!AlItems.Keys.Contains(name))
                    {
                        msgList.Add($"{name}*{count}(0)");
                        isEnough = false;
                    }
                    else
                    {
                        msgList.Add($"{name}*{count}({AlItems[name]})");
                        if (count > AlItems[name])
                        {
                            isEnough = false;
                        }
                    }
                }
            }

            if (!combineNeed.MagicDirtNeed.IsNullOrEmpty())
            {
                foreach (var dirtNeed in combineNeed.MagicDirtNeed)
                {
                    var (name, count) = dirtNeed;
                    if (!MagicDirt.Keys.Contains(name))
                    {
                        msgList.Add($"{name}*{count}(0)");
                        isEnough = false;
                    }
                    else
                    {
                        msgList.Add($"{name}*{count}({MagicDirt[name]})");
                        if (count > MagicDirt[name])
                        {
                            isEnough = false;
                        }
                    }
                }
            }

            if (!combineNeed.NormalItemNeed.IsNullOrEmpty())
            {
                foreach (var item in combineNeed.NormalItemNeed)
                {
                    var (name, count) = item;
                    var di = itemRecord.ItemCount.FirstOrDefault(i => i.Name == name);
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

            msg += string.Join("，", msgList);
            return isEnough;
        }

        public void MagicDirtConsume(string name, int count)
        {
            MagicDirt[name] -= count;
        }

        public void ItemConsume(string name, int count)
        {
            AlItems[name] -= count;
        }
    }
}
