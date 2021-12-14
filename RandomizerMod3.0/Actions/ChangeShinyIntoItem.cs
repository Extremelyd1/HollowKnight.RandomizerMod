using System.Linq;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using RandomizerMod.Extensions;
using RandomizerMod.FsmStateActions;
using UnityEngine;
using static RandomizerMod.GiveItemActions;

namespace RandomizerMod.Actions
{
    // ReSharper disable once UnusedMember.Global
    public class ChangeShinyIntoItem : RandomizerAction
    {
        private readonly string _fsmName;
        private readonly string _objectName;
        private readonly string _sceneName;
        private readonly GiveAction _action;
        private readonly string _item;
        private readonly string _location;
        private readonly string _nameKey;
        private readonly string _spriteName;

        public ChangeShinyIntoItem(string sceneName, string objectName, string fsmName, GiveAction action, string item, string location, string nameKey, string spriteName)
        {
            _sceneName = sceneName;
            _objectName = objectName;
            _fsmName = fsmName;
            _action = action;
            _item = item;
            _location = location;
            _nameKey = nameKey;
            _spriteName = spriteName;
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
            FsmState trinkFlash = fsm.GetState("Trink Flash");
            FsmState giveTrinket = fsm.GetState("Store Key"); // This path works well for our changes

            // Remove actions that stop shiny from spawning
            pdBool.RemoveActionsOfType<PlayerDataBoolTest>();
            pdBool.RemoveActionsOfType<StringCompare>();

            // Add our own check to stop the shiny from being grabbed twice
            pdBool.AddAction(
                new RandomizerExecuteLambda(() => fsm.SendEvent(
                    RandomizerMod.Instance.Settings.CheckLocationFound(_location) ? "COLLECTED" : null
                    )));

            // Force the FSM to follow the path for the correct trinket
            charm.ClearTransitions();
            charm.AddTransition("FINISHED", "Trink Flash");
            trinkFlash.ClearTransitions();
            fsm.GetState("Trinket Type").ClearTransitions();
            trinkFlash.AddTransition("FINISHED", "Store Key");

            giveTrinket.RemoveActionsOfType<SetPlayerDataBool>();
            giveTrinket.AddAction(new RandomizerExecuteLambda(() => GiveItem(_action, _item, _location)));

            // Makes sure the correct icon and text appear
            giveTrinket.GetActionsOfType<GetLanguageString>().First().convName = _nameKey;
            giveTrinket.GetActionsOfType<SetSpriteRendererSprite>().First().sprite = RandomizerMod.GetSprite(_spriteName);
        }
    }
}
