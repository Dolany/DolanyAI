﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Cache;
using Dolany.Database;

namespace Dolany.Ai.Core.Common.PicReview
{
    public class PicReviewer
    {
        public static PicReviewer Instance { get; } = new PicReviewer();

        private const string CachePath = "./images/Cache/";

        private readonly Dictionary<string, Action<PicReviewRecord>> ReviewCallBack = new Dictionary<string, Action<PicReviewRecord>>();

        private PicReviewer()
        {

        }

        public void Register(string key, Action<PicReviewRecord> callbackAction)
        {
            ReviewCallBack.Add(key, callbackAction);
        }

        public void Review(MsgInformationEx MsgDTO)
        {
            var record = MongoService<PicReviewRecord>.Get(p => p.Status == PicReviewStatus.Waiting).OrderBy(p => p.CreateTime).FirstOrDefault();
            if (record == null)
            {
                MsgSender.PushMsg(MsgDTO, "暂无待审核的图片！");
                return;
            }

            var msg = $"{CodeApi.Code_Image_Relational($"{CachePath}{record.PicName}")}\r";
            msg += $"来自 {GroupSettingMgr.Instance[record.GroupNum].Name} 的 {record.QQNum}\r";
            msg += $"用途：{record.Usage}\r";
            msg += "是否通过？";
            var option = Waiter.Instance.WaitForOptions(MsgDTO.FromGroup, MsgDTO.FromQQ, msg, new[] {"通过", "不通过", "取消"}, MsgDTO.BindAi);
            if (option < 0 || option == 2)
            {
                MsgSender.PushMsg(MsgDTO, "操作取消！");
                return;
            }

            record.Status = option == 0 ? PicReviewStatus.Passed : PicReviewStatus.Refused;
            record.ReviewTime = DateTime.Now;
            record.Update();

            ReviewCallBack[record.Usage](record);

            msg = record.Status == PicReviewStatus.Passed
                ? $"恭喜你，你在{record.CreateTime:yyyy-MM-dd HH:mm:ss}提交的用于{record.Usage}的图片审核通过！"
                : $"很遗憾，你在{record.CreateTime:yyyy-MM-dd HH:mm:ss}提交的用于{record.Usage}的图片未能审核通过！";
            MsgSender.PushMsg(record.GroupNum, record.QQNum, msg, GroupSettingMgr.Instance[record.GroupNum].BindAi);

            var picTempFile = new FileInfo($"{CachePath}{record.PicName}");
            picTempFile.Delete();
        }

        public void AddReview(PicReviewRecord record)
        {
            record.Insert();

            var count = MongoService<PicReviewRecord>.Count(p => p.Status == PicReviewStatus.Waiting);
            var msg = $"有新的待审核图片！\r来自 {GroupSettingMgr.Instance[record.GroupNum].Name} 的 {record.QQNum}\r当前剩余 {count} 张图片待审核！";
            MsgSender.PushMsg(0, Global.DeveloperNumber, msg, Global.DefaultConfig.MainAi);
        }
    }
}
