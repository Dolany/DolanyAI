﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Dolany.Ai.Doremi.Common
{
    public class AIAnalyzer
    {
        private static readonly object Lock = new object();

        public static DateTime Sys_StartTime;

        private static readonly List<CommandAnalyzeDTO> Commands = new List<CommandAnalyzeDTO>();

        private static readonly List<ErrorModel> ErrorList = new List<ErrorModel>();

        public static void AddCommandCount(CommandAnalyzeDTO model)
        {
            lock (Lock)
            {
                Commands.Add(model);
                RecentCommandCache.Cache(model.BindAi);
            }
        }

        public static int GetCommandCount()
        {
            lock (Lock)
            {
                return Commands.Count;
            }
        }

        public static IEnumerable<GroupAnalyzeModel> AnalyzeGroup(string BindAi)
        {
            lock (Lock)
            {
                return Commands.Where(p => p.BindAi == BindAi).GroupBy(c => c.GroupNum).Select(c => new GroupAnalyzeModel()
                {
                    GroupNum = c.Key,
                    CommandCount = c.Count()
                }).OrderByDescending(c => c.CommandCount).Take(10).ToList();
            }
        }

        public static IEnumerable<AIAnalyzeModel> AnalyzeAI(string BindAi)
        {
            lock (Lock)
            {
                return Commands.Where(p => p.BindAi == BindAi).GroupBy(c => c.Ai).Select(c => new AIAnalyzeModel()
                {
                    AIName = c.Key,
                    CommandCount = c.Count()
                }).OrderByDescending(c => c.CommandCount).Take(10).ToList();
            }
        }

        public static IEnumerable<TimeAnalyzeModel> AnalyzeTime(string BindAi)
        {
            lock (Lock)
            {
                return Commands.Where(c => c.BindAi == BindAi && c.Time.AddHours(12) > DateTime.Now).GroupBy(c => c.Time.Hour).Select(c => new TimeAnalyzeModel()
                {
                    Hour = c.Key,
                    CommandCount = c.Count()
                }).ToList();
            }
        }

        public static IEnumerable<CommandAnalyzeModel> AnalyzeCommand(string BindAi)
        {
            lock (Lock)
            {
                return Commands.Where(p => p.BindAi == BindAi).GroupBy(c => c.Command).Select(c => new CommandAnalyzeModel()
                {
                    Command = c.Key,
                    CommandCount = c.Count()
                }).OrderByDescending(c => c.CommandCount).Take(10).ToList();
            }
        }

        public static int GetErrorCount()
        {
            lock (Lock)
            {
                return ErrorList.Count;
            }
        }

        public static void AddError(string msg)
        {
            lock (Lock)
            {
                var model = ErrorList.FirstOrDefault(p => p.Msg == msg);
                if (model == null)
                {
                    ErrorList.Add(new ErrorModel(){Msg = msg, Time = DateTime.Now, Count = 1});
                }
                else
                {
                    ErrorList.Remove(model);
                    model.Count++;
                    ErrorList.Add(model);
                }
            }
        }

        public static string GetErrorMsg(int index)
        {
            lock (Lock)
            {
                if (index < 0 || index >= ErrorList.Count)
                {
                    return string.Empty;
                }

                return $"{ErrorList[index].Time}\r{ErrorList[index].Msg}\r{ErrorList[index].Count}";
            }
        }
    }

    public class ErrorModel
    {
        public DateTime Time { get; set; }

        public string Msg { get; set; }

        public int Count { get; set; }
    }

    public class CommandAnalyzeDTO
    {
        public long GroupNum { get; set; }

        public string Ai { get; set; }

        public string Command { get; set; }

        public DateTime Time { get; set; } = DateTime.Now;

        public string BindAi { get; set; }
    }

    public class GroupAnalyzeModel
    {
        public long GroupNum { get; set; }

        public int CommandCount { get; set; }
    }

    public class AIAnalyzeModel
    {
        public string AIName { get; set; }

        public int CommandCount { get; set; }
    }

    public class TimeAnalyzeModel
    {
        public int Hour { get; set; }

        public int CommandCount { get; set; }
    }

    public class CommandAnalyzeModel
    {
        public string Command { get; set; }

        public int CommandCount { get; set; }
    }
}