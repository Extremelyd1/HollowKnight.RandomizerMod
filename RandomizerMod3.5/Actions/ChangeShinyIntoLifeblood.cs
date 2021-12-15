using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using RandomizerMod.Extensions;
using RandomizerMod.FsmStateActions;
using UnityEngine;

namespace RandomizerMod.Actions
{
    // ReSharper disable once UnusedMember.Global
    public class ChangeShinyIntoLifeblood : RandomizerAction
    {
        private readonly string _item;
        private readonly string _fsmName;
        private readonly int _masksAmount;
        private readonly string _objectName;
        private readonly string _sceneName;
        private readonly string _location;
    

        public ChangeShinyIntoLifeblood(string sceneName, string objectName, string fsmName, int masksAmount, string item, string location)
        {
            _sceneName = sceneName;
            _objectName = objectName;
            _fsmName = fsmName;
            _masksAmount = masksAmount;
            _item = item;
            _location = location;
        }

        public override ActionType Type => ActionType.PlayMakerFSM;

        public override void Process(string scene, Object changeObj)
        {
            if (scene != _sceneName || !(changeObj is PlayMakerFSM fsm) || fsm.FsmName != _fsmName || fsm.gameObject.name != _objectName) {
                return;
            }

            FsmState pdBool = fsm.GetState("PD Bool?");
            FsmState charm = fsm.GetState("Charm?");
            FsmState getCharm = fsm.GetState("Get Charm");

            // Remove actions that stop shiny from spawning
            pdBool.RemoveActionsOfType<PlayerDataBoolTest>();
            pdBool.RemoveActionsOfType<StringCompare>();

            // Add our own check to stop the shiny from being grabbed twice
            pdBool.AddAction(
                new RandomizerExecuteLambda(() => fsm.SendEvent(
                    RandomizerMod.Instance.Settings.CheckLocationFound(_location) ? "COLLECTED" : null
                    )));
            
            // Mimic the way geo spawns work.
            charm.ClearTransitions();
            charm.AddTransition("FINISHED", "Get Charm");
            getCharm.RemoveActionsOfType<SetPlayerDataBool>();
            getCharm.RemoveActionsOfType<IncrementPlayerDataInt>();
            getCharm.RemoveActionsOfType<SendMessage>();

            getCharm.AddAction(new RandomizerExecuteLambda(() => GiveItemActions.GiveItem(GiveItemActions.GiveAction.None, _item, _location)));
            getCharm.AddAction(new RandomizerAddLifeblood(_masksAmount));

            getCharm.ClearTransitions();
            getCharm.AddTransition("FINISHED", "Flash");
        }
    }
}