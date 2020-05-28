using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Cache;

namespace Dolany.WorldLine.Standard.Ai.Game.Archaeology.ArchAdv
{
    public class ArchAdvEngine
    {
        private readonly MsgInformationEx MsgDTO;

        private ArchaeologySceneModel AdvScene;

        private ArchaeologySceneSvc SceneSvc => AutofacSvc.Resolve<ArchaeologySceneSvc>();
        private WaiterSvc WaiterSvc => AutofacSvc.Resolve<WaiterSvc>();
        private ArchAdvSubSceneSvc ArchAdvSubSceneSvc => AutofacSvc.Resolve<ArchAdvSubSceneSvc>();

        public ArchAdvEngine(MsgInformationEx MsgDTO)
        {
            this.MsgDTO = MsgDTO;
        }

        public void StartAdv()
        {
            SelectScene();
            if (AdvScene == null)
            {
                return;
            }

            foreach (var subScene in AdvScene.SubScenes)
            {
                var scene = ArchAdvSubSceneSvc.CreateSubScene(subScene.ArchType, subScene, AdvScene.Level, MsgDTO);
                MsgSender.PushMsg(MsgDTO, $"你遇到了 【{subScene.Name}】！");
                if (!scene.StartAdv())
                {
                    break;
                }
            }

            MsgSender.PushMsg(MsgDTO, "冒险结束！");
        }

        private void SelectScene()
        {
            var dailyScene = ArchDailyScene.Get(MsgDTO.FromQQ);
            var archaeologist = Archaeologist.Get(MsgDTO.FromQQ);

            var Scenes = SceneSvc.GetLevelScene(dailyScene.Scenes, archaeologist.AdvSceneLvlDic);
            var msgList = Scenes.Select(p => $"【{p.Name}】(lv.{p.Level}):{p.Description}").ToArray();
            var optionIdx = WaiterSvc.WaitForOptions(MsgDTO.FromGroup, MsgDTO.FromQQ, "请选择你要进入的副本！", msgList, MsgDTO.BindAi);
            if (optionIdx >= 0)
            {
                AdvScene = Scenes[optionIdx];
                return;
            }

            var asset = ArchAsset.Get(MsgDTO.FromQQ);
            asset.GreenAmbur++;
            asset.Update();

            MsgSender.PushMsg(MsgDTO, "操作取消！退还 【翠绿琥珀】*1");
        }
    }
}
