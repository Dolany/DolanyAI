﻿using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database;
using Dolany.Database.Sqlite;
using Dolany.Database.Sqlite.Model;

namespace Dolany.Game.Alchemy
{
    public class AlPlayer : BaseEntity
    {
        public long QQNum { get; set; }

        public int Level { get; set; }

        public int Exp { get; set; }

        public long MaxHP { get; set; }

        public long CurHP { get; set; }

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
                if (player.CurHP <= 0 && player.IsAlive)
                {
                    player.CurHP = player.MaxHP;
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

        public void MagicDirtConsume(string name, int count = 1)
        {
            MagicDirt[name] -= count;
        }

        public void ItemConsume(string name, int count = 1)
        {
            AlItems[name] -= count;
        }

        public void ItemGain(string name, int count = 1)
        {
            if (AlItems.ContainsKey(name))
            {
                AlItems[name] += count;
            }

            AlItems.Add(name, count);
        }

        public void MagicDirtGain(string name, int count = 1)
        {
            if (MagicDirt.ContainsKey(name))
            {
                MagicDirt[name] += count;
            }

            MagicDirt.Add(name, count);
        }

        public bool IsAlive
        {
            get
            {
                var cache = SCacheService.Get<AlPlayerAliveCache>($"AlPlayerAliveCache-{QQNum}");
                return cache == null;
            }
        }
    }
}
