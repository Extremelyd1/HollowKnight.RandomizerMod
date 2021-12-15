using HutongGames.PlayMaker.Actions;
using RandomizerMod.Extensions;
using RandomizerMod.FsmStateActions;
using UnityEngine;
using static RandomizerMod.GiveItemActions;

namespace RandomizerMod.Actions
{
    public class ChangeCrystalShamanReward : RandomizerAction
    {
        private readonly string _fsmName;
        private readonly string _objectName;
        private readonly string _sceneName;
        private readonly GiveAction _action;
        private readonly string _item;
        private readonly string _location;

        public ChangeCrystalShamanReward(string sceneName, string objectName, string fsmName, GiveAction action, string item, string location)
        {
            
            _sceneName = sceneName;
            _objectName = objectName;
            _fsmName = fsmName;
            // GiveItem doesn't support spawning geo, and also there's no shiny to spawn it from anyway.
            if (action == GiveAction.SpawnGeo)
            {
                action = GiveAction.AddGeo;
            }
            _action = action;
            _item = item;
            _location = location;
        }

        public override ActionType Type => ActionType.PlayMakerFSM;

        public override void Process(string scene, Object changeObj)
        {
            if (!(scene == _sceneName && changeObj is PlayMakerFSM fsm && fsm.FsmName == _fsmName && fsm.gameObject.name == _objectName))
            {
                return;
            }

            var init = fsm.GetState("Init");
            init.RemoveActionsOfType<IntCompare>();
            init.AddAction(new RandomizerExecuteLambda(() => fsm.SendEvent(RandomizerMod.Instance.Settings.CheckLocationFound(_location) ? "BROKEN" : null)));

            var get = fsm.GetState("Get PlayerData 2");
            get.ClearTransitions();
            get.AddTransition("FINISHED", "Get Up");
            get.RemoveActionsOfType<SetPlayerDataInt>();
            get.AddAction(new RandomizerExecuteLambda(() => {
                // The popup should be shown before GiveItem so that grub pickups and additive items
                // appear correctly.
                ShowEffectiveItemPopup(_item);
                GiveItem(_action, _item, _location);
            }));
        }
    }
}