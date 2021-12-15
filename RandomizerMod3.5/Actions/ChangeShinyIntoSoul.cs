using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using RandomizerMod.Extensions;
using RandomizerMod.FsmStateActions;
using UnityEngine;

namespace RandomizerMod.Actions
{
    public class ChangeShinyIntoSoul : RandomizerAction
    {
        private readonly string _item;
        private readonly string _fsmName;
        private readonly string _objectName;
        private readonly string _sceneName;
        private readonly string _location;

        public ChangeShinyIntoSoul(string sceneName, string objectName, string fsmName, string item, string location)
        {
            _sceneName = sceneName;
            _objectName = objectName;
            _fsmName = fsmName;
            _item = item;
            _location = location;
        }

        public override ActionType Type => ActionType.PlayMakerFSM;

        public override void Process(string scene, Object changeObj)
        {
            if (scene != _sceneName || !(changeObj is PlayMakerFSM fsm) || fsm.FsmName != _fsmName ||
                fsm.gameObject.name != _objectName)
            {
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

            // The "Charm?" state is a bad entry point for our soul spawning
            charm.ClearTransitions();
            charm.AddTransition("FINISHED", "Get Charm");
            // The "Get Charm" state is a good entry point for our soul spawning
            getCharm.RemoveActionsOfType<SetPlayerDataBool>();
            getCharm.RemoveActionsOfType<IncrementPlayerDataInt>();
            getCharm.RemoveActionsOfType<SendMessage>();

            getCharm.AddAction(new RandomizerExecuteLambda(() => GiveItemActions.GiveItem(GiveItemActions.GiveAction.None, _item, _location)));
            getCharm.AddAction(new RandomizerAddSoul(fsm.gameObject));

            // Skip all the other type checks
            getCharm.ClearTransitions();
            getCharm.AddTransition("FINISHED", "Flash");
        }
    }
}
