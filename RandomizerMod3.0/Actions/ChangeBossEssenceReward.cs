using System;
using HutongGames.PlayMaker;
using RandomizerMod.Extensions;
using RandomizerMod.FsmStateActions;
using static RandomizerMod.GiveItemActions;
using Object = UnityEngine.Object;

namespace RandomizerMod.Actions
{
    public class ChangeBossEssenceReward : RandomizerAction
    {
        private readonly string _fsmName;
        private readonly string _objectName;
        private readonly string _sceneName;
        private readonly GiveAction _action;
        private readonly string _item;
        private readonly string _location;

        public ChangeBossEssenceReward(string sceneName, string objectName, string fsmName, GiveAction action, string item, string location)
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

            if (_fsmName == "Award Orbs")
            {
                ReplaceReward(fsm.GetState("Award"));
            }
            else
            {
                ReplaceReward(fsm.GetState("Get"));
                // This is also needed to prevent the essence counter from appearing
                RemoveLastActions(fsm.GetState("Vanish Burst"), 1);
            }
        }

        private void ReplaceReward(FsmState get)
        {
            // Remove the Essence; not using RemoveActionsOfType because, for Dream Warriors,
            // there are two of type SendEventByName and we only want to remove one of them.
            // For White Defender and GPZ, the last one isn't a SendEventByName but it's a different
            // one that also displays the on-screen Essence counter, so the same procedure
            // is appropriate for both.
            RemoveLastActions(get, 2);
            // Add our custom item
            get.AddAction(new RandomizerExecuteLambda(() => {
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
