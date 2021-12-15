using RandomizerMod.Extensions;
using RandomizerMod.FsmStateActions;
using UnityEngine;

namespace RandomizerMod.Actions
{
    public class ReplaceBasinVesselWithShiny : RandomizerAction
    {
        private string _shinyName;
        private GameObject _parent;

        public ReplaceBasinVesselWithShiny(string shinyName)
        {
            _shinyName = shinyName;
        }

        public override ActionType Type => ActionType.PlayMakerFSM;

        public override void Process(string scene, Object changeObj)
        {
            if (!(scene == "Abyss_04" && changeObj is PlayMakerFSM fsm && fsm.gameObject.name == "Wishing_Well_anims" && fsm.FsmName == "Fountain Control"))
            {
                return;
            }
            _parent = fsm.gameObject;
            fsm.GetState("Appear").Actions[1] = new RandomizerExecuteLambda(ActivateShiny);
            fsm.GetState("Already Paid").Actions[1] = new RandomizerExecuteLambda(ActivateShiny);
        }

        private void ActivateShiny()
        {
            _parent.transform.Find(_shinyName).gameObject.SetActive(true);
        }
    }
}