using System;
using System.Linq;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Model;
using Dolany.Ai.Core.OnlineStore;
using Dolany.Database.Ai;

namespace Dolany.Ai.Core.Ai.Sys.BonusCenter
{
    public class Bonus_awsl : BonusBase
    {
        public override string Code { get; } = "awsl";
        public override bool IsExpiried => DateTime.Now > new DateTime(2019, 5, 8);
        public override bool SendBonus(MsgInformationEx MsgDTO)
        {
            var honors = HonorHelper.Instance.HonorList.Where(p => !(p is LimitHonorModel)).Select(p => p.Name).ToList();

            var Infomation = Waiter.Instance.WaitForInformation(MsgDTO, "请输入想要获得的成就名（仅限非限定成就）", p => p.FromQQ == MsgDTO.FromQQ && honors.Contains(p.Msg));
            if (Infomation == null)
            {
                MsgSender.PushMsg(MsgDTO, "操作取消");
                return false;
            }

            var honorName = Infomation.Msg;
            var honor = HonorHelper.Instance.FindHonor(honorName);
            var driftItemRecord = DriftItemRecord.GetRecord(MsgDTO.FromQQ);
            foreach (var item in honor.Items)
            {
                var ic = driftItemRecord.ItemCount.FirstOrDefault(p => p.Name == item.Name);
                if (ic != null)
                {
                    ic.Count += 1;
                }
                else
                {
                    driftItemRecord.ItemCount.Add(new DriftItemCountRecord {Count = 1, Name = item.Name});
                }
            }
            driftItemRecord.Update();

            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            osPerson.Golds += 8000;
            osPerson.Update();

            var personCache = PersonCacheRecord.Get(MsgDTO.FromQQ, "抽奖");
            personCache.Value = int.TryParse(personCache.Value, out var times) ? (times + 2).ToString() : 2.ToString();

            personCache.Update();

            return true;
        }
    }
}
