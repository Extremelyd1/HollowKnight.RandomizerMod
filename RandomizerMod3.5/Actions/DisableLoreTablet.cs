using HutongGames.PlayMaker;
using RandomizerMod.Extensions;
using UnityEngine;

namespace RandomizerMod.Actions
{
    public class DisableLoreTablet : RandomizerAction
    {
        private readonly string _fsmName;
        private readonly string _objectName;
        private readonly string _sceneName;

        public DisableLoreTablet(string sceneName, string objectName, string fsmName)
        {

            _sceneName = sceneName;
            _objectName = objectName;
            _fsmName = fsmName;
        }

        public override ActionType Type => ActionType.PlayMakerFSM;

        public override void Process(string scene, Object changeObj)
        {
            if (!(scene == _sceneName && changeObj is PlayMakerFSM fsm && fsm.FsmName == _fsmName && fsm.gameObject.name == _objectName))
            {
                return;
            }

            fsm.GetState("Init").ClearTransitions();

            // Big text avoids the Init state
            if (fsm.GetState("Inert") is FsmState inertState) inertState.ClearTransitions();
        }
    }
}
