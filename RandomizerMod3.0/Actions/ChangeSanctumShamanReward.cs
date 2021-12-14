using HutongGames.PlayMaker.Actions;
using RandomizerMod.Extensions;
using RandomizerMod.FsmStateActions;
using UnityEngine;
using static RandomizerMod.GiveItemActions;

namespace RandomizerMod.Actions
{
    public class ChangeSanctumShamanReward : RandomizerAction
    {
        private readonly string _fsmName;
        private readonly string _objectName;
        private readonly string _sceneName;
        private readonly GiveAction _action;
        private readonly string _item;
        private readonly string _location;

        public ChangeSanctumShamanReward(string sceneName, string objectName, string fsmName, GiveAction action, string item, string location)
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
            if (!(scene == _sceneName && changeObj is PlayMakerFSM fsm))
            {
                return;
            }
            // The logic for checking and granting Shade Soul is split between two different game
            // objects.
            if (fsm.FsmName == _fsmName && fsm.gameObject.name == _objectName)
            {
                var gotSpell = fsm.GetState("Got Spell?");
                gotSpell.RemoveActionsOfType<IntCompare>();
                gotSpell.AddAction(new RandomizerExecuteLambda(() => fsm.SendEvent(RandomizerMod.Instance.Settings.CheckLocationFound(_location) ? "ACTIVATED" : null)));
            }
            else if (fsm.FsmName == "Get Fireball" && fsm.gameObject.name == "Knight Get Fireball Lv2")
            {
                var get = fsm.GetState("Get PlayerData");
                get.ClearTransitions();
                get.AddTransition("FINISHED", "Get Up Anim");
                get.RemoveActionsOfType<SetPlayerDataInt>();
                get.AddAction(new RandomizerExecuteLambda(() => {
                    ShowEffectiveItemPopup(_item);
                    GiveItem(_action, _item, _location);
                }));
            }
        }
    }
}