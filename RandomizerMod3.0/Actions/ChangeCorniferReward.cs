using System;
using HutongGames.PlayMaker;
using RandomizerMod.Extensions;
using RandomizerMod.FsmStateActions;
using static RandomizerMod.GiveItemActions;
using Object = UnityEngine.Object;

namespace RandomizerMod.Actions
{
    public class ChangeCorniferReward : RandomizerAction
    {
        private readonly string _fsmName;
        private readonly string _objectName;
        private readonly string _sceneName;
        private readonly GiveAction _action;
        private readonly string _item;
        private readonly string _location;

        public ChangeCorniferReward(string sceneName, string objectName, string fsmName, GiveAction action, string item, string location)
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

            var deepnest = _objectName == "Cornifer Deepnest";

            fsm.GetState("Check Active").Actions[0] = new RandomizerExecuteLambda(() => fsm.SendEvent(RandomizerMod.Instance.Settings.CheckLocationFound(_location) ? "DESTROY" : null));
            fsm.GetState("Convo Choice").Actions[1] = new RandomizerExecuteLambda(() => fsm.SendEvent(RandomizerMod.Instance.Settings.CheckLocationFound(_location) ? "BOUGHT" : null));

            var get = fsm.GetState("Geo Pause and GetMap");
            RemoveLastActions(get, deepnest ? 1 : 5);
            get.AddAction(new RandomizerExecuteLambda(() => {
                // The popup should be shown before GiveItem so that grub pickups and additive items
                // appear correctly.
                ShowEffectiveItemPopup(_item);
                GiveItem(_action, _item, _location);
            }));
            get.ClearTransitions();
            get.AddTransition("FINISHED", deepnest ? "Box Down Event 2" : "Box Up 3");
            if (deepnest) {
                // Bypass the extra check that disables one of the Deepnest Cornifer locations
                // if the other one has been visited.
                var check = fsm.GetState("Not At Deepnest");
                check.ClearTransitions();
                check.AddTransition("FINISHED", "Check Active");
            }
        }

        private static void RemoveLastActions(FsmState s, int n)
        {
            var newActions = new FsmStateAction[s.Actions.Length - n];
            Array.Copy(s.Actions, newActions, newActions.Length);
            s.Actions = newActions;
        }
    }
}