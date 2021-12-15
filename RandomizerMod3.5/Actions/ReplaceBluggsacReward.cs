using HutongGames.PlayMaker;
using RandomizerMod.Extensions;
using RandomizerMod.FsmStateActions;
using UnityEngine;

namespace RandomizerMod.Actions
{
    public class ReplaceBluggsacReward : RandomizerAction
    {
        private readonly string _sceneName;
        private readonly string _shinyName;

        public ReplaceBluggsacReward(string sceneName, string shinyName)
        {
            _sceneName = sceneName;
            _shinyName = shinyName;
        }

        public override ActionType Type => ActionType.PlayMakerFSM;

        public override void Process(string scene, Object changeObj)
        {
            if (!(scene == _sceneName && changeObj is PlayMakerFSM fsm && fsm.FsmName == "Control" && fsm.gameObject.name.StartsWith("Corpse Egg Sac")))
            {
                return;
            }

            FsmState init = fsm.GetState("Init");
            init.Actions[1] = new RandomizerExecuteLambda(() =>
            {
                fsm.FsmVariables.GetFsmGameObject("Egg").Value = GameObject.Find(_shinyName + " Parent").FindGameObjectInChildren(_shinyName);
            });
        }
    }
}
