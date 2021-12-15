using System;
using HutongGames.PlayMaker;
using RandomizerMod.Extensions;
using RandomizerMod.FsmStateActions;
using static RandomizerMod.GiveItemActions;
using Object = UnityEngine.Object;

namespace RandomizerMod.Actions
{
    public class ChangeNailmasterReward : RandomizerAction
    {
        private readonly string _fsmName;
        private readonly string _objectName;
        private readonly string _sceneName;
        private readonly GiveAction _action;
        private readonly string _item;
        private readonly string _location;

        public ChangeNailmasterReward(string sceneName, string objectName, string fsmName, GiveAction action, string item, string location)
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

            // Instead of checking for the Nail Art, check for the item that replaces it.
            var start = fsm.GetState("Convo Choice");
            var nailArtCheck = _objectName == "NM Sheo NPC" ? 2 : 1;
            start.Actions[nailArtCheck] = new RandomizerExecuteLambda(() => fsm.SendEvent(
                RandomizerMod.Instance.Settings.CheckLocationFound(_location) ? null : "REOFFER"
                ));

            // Replace the Nail Art popup and item.
            var getMsg = fsm.GetState("Get Msg");
            getMsg.ClearTransitions();
            getMsg.AddTransition("FINISHED", "Fade Back");
            RemoveLastActions(getMsg, 4);

            fsm.GetState("Fade Back").AddAction(new RandomizerExecuteLambda(() => {
                // The popup should be shown before GiveItem so that grub pickups and additive items
                // appear correctly.
                ShowEffectiveItemPopup(_item);
                GiveItem(_action, _item, _location);
            }));
        }

        private static void RemoveLastActions(FsmState s, int n)
        {
            var newActions = new FsmStateAction[s.Actions.Length - n];
            Array.Copy(s.Actions, newActions, newActions.Length);
            s.Actions = newActions;
        }
    }
}