using HutongGames.PlayMaker.Actions;
using RandomizerMod.Extensions;
using UnityEngine;

namespace RandomizerMod.Actions
{
    public class PreventSelfDestruct : RandomizerAction
    {
        private readonly string _sceneName;
        private readonly string _objectName;
        private readonly string _fsmName;

        public PreventSelfDestruct(string sceneName, string objectName, string fsmName)
        {
            _sceneName = sceneName;
            _objectName = objectName;
            _fsmName = fsmName;
        }

        public override ActionType Type => ActionType.PlayMakerFSM;

        public override void Process(string scene, Object changeObj)
        {
            if (scene != _sceneName || !(changeObj is PlayMakerFSM fsm) || fsm.FsmName != _fsmName ||
                fsm.gameObject.name != _objectName)
            {
                return;
            }

            if (_objectName == "Shaman Meeting" && _fsmName == "Conversation Control")
            {
                // VS needs a special case
                fsm.GetState("Check Active").RemoveActionsOfType<DestroySelf>();
                fsm.GetState("Check Active").ClearTransitions();
            }
            else
            {
                fsm.GetState("Destroy").RemoveActionsOfType<DestroySelf>();
                fsm.GetState("Destroy").RemoveActionsOfType<ActivateGameObject>();
            }
        }
    }
}
