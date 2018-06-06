using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using Flexlive.CQP.Framework;
using System.Xml;
using System.Xml.Linq;

namespace AILib
{
    [AI(Name = "CartoonSayingAI", Description = "AI for Cartoon Sayings.", IsAvailable = true)]
    public class CartoonSayingAI : AIBase
    {
        private string xmlFilePath = @"./AI/CartoonSaying/Saying.xml";

        private List<SayingInfo> SayingList
        {
            get
            {
                XElement root = XElement.Load(xmlFilePath);

                List<SayingInfo> list = new List<SayingInfo>();
                foreach (XElement node in root.Nodes())
                {
                    SayingInfo s = new SayingInfo()
                    {
                        Cartoon = node.Attribute("Cartoon").Value,
                        Charactor = node.Attribute("Charactor").Value,
                        Sayings = node.Value
                    };

                    list.Add(s);
                }

                return list;
            }
        }

        public CartoonSayingAI(AIConfigDTO ConfigDTO)
            : base(ConfigDTO)
        {

        }

        public override void Work()
        {
            
        }

        public override void OnPrivateMsgReceived(PrivateMsgDTO MsgDTO)
        {
            base.OnPrivateMsgReceived(MsgDTO);

            ProcceedMsg(MsgDTO.fromQQ, MsgDTO.msg, CQ.SendPrivateMessage);
        }

        public override void OnGroupMsgReceived(GroupMsgDTO MsgDTO)
        {
            base.OnGroupMsgReceived(MsgDTO);

            ProcceedMsg(MsgDTO.fromGroup, MsgDTO.msg, CQ.SendGroupMessage);
        }

        private void ProcceedMsg(long fromQQ, string msg, Action<long, string> sendAction)
        {
            SayingInfo info = SayingInfo.Parse(msg);
            if (info != null)
            {
                string smsg = SaveSaying(info) ? "语录录入成功！" : "语录录入失败！";
                sendAction(fromQQ, smsg);
                return;
            }

            if (msg.Trim().Equals("语录"))
            {
                string ranSaying = GetRanSaying();
                if (string.IsNullOrEmpty(ranSaying))
                {
                    return;
                }

                sendAction(fromQQ, ranSaying);
                return;
            }

            if(msg.StartsWith("语录 "))
            {
                string keyword = msg.Replace("语录 ", "").Trim();
                string ranSaying = GetRanSaying(keyword);
                if (string.IsNullOrEmpty(ranSaying))
                {
                    return;
                }

                sendAction(fromQQ, ranSaying);
                return;
            }
        }

        private bool SaveSaying(SayingInfo info)
        {
            try
            {
                XElement root = XElement.Load(xmlFilePath);
                XElement node = new XElement("Saying", info.Sayings);
                node.SetAttributeValue("Cartoon", info.Cartoon);
                node.SetAttributeValue("Charactor", info.Charactor);
                root.Add(node);
                root.Save(xmlFilePath);

                return true;
            }
            catch(Exception ex)
            {
                Common.SendMsgToDeveloper(ex);
                return false;
            }
        }

        private string GetRanSaying(string keyword = null)
        {
            try
            {
                var list = SayingList;
                if (!string.IsNullOrEmpty(keyword))
                {
                    var query = from saying in list
                                where saying.Cartoon.Contains(keyword) || saying.Charactor.Contains(keyword)
                                select saying;
                    if(query == null)
                    {
                        return string.Empty;
                    }
                    list = query.ToList();
                }

                Random random = new Random();
                int randIdx = random.Next(list.Count);
                
                return GetShownSaying(list[randIdx]);
            }
            catch (Exception ex)
            {
                Common.SendMsgToDeveloper(ex);
                return string.Empty;
            }
        }

        private string GetShownSaying(SayingInfo s)
        {
            string shownSaying = $@"
    {s.Sayings}
    ——《{s.Cartoon}》 {s.Charactor}
";

            return shownSaying;
        }

        [AIDebug(EntrancePoint = "CartoonSayingAI_SayingTotalCount")]
        public string Debug_SayingTotalCount
        {
            get
            {
                return $@"当前记录语录 {SayingList.Count}条";
            }
        }
    }
}
