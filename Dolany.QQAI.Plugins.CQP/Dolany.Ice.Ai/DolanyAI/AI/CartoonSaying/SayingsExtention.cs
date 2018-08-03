﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dolany.Ice.Ai.DolanyAI.Db;

namespace Dolany.Ice.Ai.DolanyAI
{
    public static class SayingsExtention
    {
        public static Saying Parse(string Msg)
        {
            if (string.IsNullOrEmpty(Msg))
            {
                return null;
            }

            try
            {
                string[] parts = Msg.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 3)
                {
                    return null;
                }

                Saying si = new Saying()
                {
                    Id = Guid.NewGuid().ToString(),
                    Cartoon = parts[0],
                    Charactor = parts[1],
                    Content = parts[2]
                };
                if (si.IsValid())
                {
                    return si;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public static bool IsValid(this Saying saying)
        {
            return !string.IsNullOrEmpty(saying.Cartoon) && !string.IsNullOrEmpty(saying.Charactor) && !string.IsNullOrEmpty(saying.Content);
        }

        public static bool Contains(this Saying saying, string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return true;
            }

            return saying.Cartoon.Contains(keyword) || saying.Charactor.Contains(keyword) || saying.Content.Contains(keyword);
        }
    }
}