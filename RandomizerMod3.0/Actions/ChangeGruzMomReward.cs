using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using RandomizerMod.Extensions;
using RandomizerMod.FsmStateActions;
using UnityEngine;

namespace RandomizerMod.Actions
{
    public class ChangeGruzMomReward : RandomizerAction
    {
        private readonly string _shinyParentName;

        public ChangeGruzMomReward(string shinyParentName)
        {
            _shinyParentName = shinyParentName;
        }

        public override ActionType Type => ActionType.PlayMakerFSM;

        public override void Process(string scene, Object changeObj)
        {
            if (scene != SceneNames.Crossroads_04 || !(changeObj is PlayMakerFSM fsm) || fsm.FsmName != "burster" ||
                !fsm.gameObject.name.StartsWith("Corpse Big Fly Burster") || !(GameObject.Find(_shinyParentName) is GameObject parent))
            {
                return;
            }

            // FSM contained in sharedassets40.assets

            FsmState geoState = fsm.GetState("Geo");
            geoState.RemoveActionsOfType<FlingObjectsFromGlobalPool>();
            geoState.AddAction(new RandomizerExecuteLambda(() =>
            {
                foreach (Transform t in parent.transform)
                {
                    t.SetPosition2D(fsm.gameObject.transform.position);
                    t.gameObject.SetActive(true);
                }

                if (parent.name.Contains("Randomizer Shiny"))
                {
                    parent.transform.DetachChildren();
                    Object.Destroy(parent);
                }
            }));
        }
    }
}
