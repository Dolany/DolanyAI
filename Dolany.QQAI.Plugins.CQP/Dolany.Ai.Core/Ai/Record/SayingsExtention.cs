using System;

namespace Dolany.Ai.Core.Ai.Record
{
    using Dolany.Ai.Core.Db;

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
                var parts = Msg.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 3)
                {
                    return null;
                }

                var si = new Saying
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
            catch (Exception)
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
