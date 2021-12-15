using HutongGames.PlayMaker.Actions;
using RandomizerMod.Extensions;
using RandomizerMod.FsmStateActions;
using UnityEngine;
using static RandomizerMod.GiveItemActions;

namespace RandomizerMod.Actions
{
    public class ChangeSlyReward : RandomizerAction
    {
        private readonly string _fsmName;
        private readonly string _objectName;
        private readonly string _sceneName;
        private readonly GiveAction _action;
        private readonly string _item;
        private readonly string _location;

        public ChangeSlyReward(string sceneName, string objectName, string fsmName, GiveAction action, string item, string location)
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

            var start = fsm.GetState("Convo Choice");
            start.Actions[0] = new RandomizerExecuteLambda(() => fsm.SendEvent(
                RandomizerMod.Instance.Settings.CheckLocationFound(_location) ? "REPEAT" : "GIVE"
                ));
            
            var give = fsm.GetState("Give");
            (give.Actions[5] as Wait).time.Value = 2;
            give.Actions = new[] {
                // Retain the visual and sound effects of the shiny, and also the Wait action
                // so that the player can see what they got before they're warped out
                give.Actions[2],
                give.Actions[5],
                give.Actions[6],
                new RandomizerExecuteLambda(() => {
                    ShowEffectiveItemPopup(_item);
                    GiveItem(_action, _item, _location);
                    RandomizerMod.Instance.Settings.SlyCharm = true;
                }),
            };
            // Make Sly pickup send Sly back upstairs -- warps player out to prevent resulting softlock from trying to enter the shop from a missing transition
            fsm.GetState("End").AddFirstAction(new RandomizerChangeScene("Town", "door_sly"));
        }
    }
}