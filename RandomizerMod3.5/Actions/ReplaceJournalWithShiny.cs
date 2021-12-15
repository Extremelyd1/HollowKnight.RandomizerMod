using RandomizerMod.Extensions;
using RandomizerMod.FsmStateActions;
using UnityEngine;

namespace RandomizerMod.Actions
{
    public class ReplaceJournalWithShiny : RandomizerAction
    {
        private string _shinyName;
        private GameObject _parent;

        public ReplaceJournalWithShiny(string shinyName)
        {
            _shinyName = shinyName;
        }

        public override ActionType Type => ActionType.PlayMakerFSM;

        public override void Process(string scene, Object changeObj)
        {
            if (!(scene == "Fungus1_08" && changeObj is PlayMakerFSM fsm && fsm.gameObject.name == "Hunter Eyes"))
            {
                return;
            }
            _parent = fsm.gameObject;
            switch (fsm.FsmName)
            {
                case "Conversation Control":
                    fsm.GetState("Spit").Actions[2] = new RandomizerExecuteLambda(ActivateShiny);
                    break;
                case "Check Journal Placement":
                    fsm.GetState("Check Journal").Actions[0] = new RandomizerExecuteLambda(() => fsm.SendEvent(Ref.PD.GetBool("metHunter") && !RandomizerMod.Instance.Settings.CheckLocationFound("Hunter's_Journal") ? "PLACE" : null));
                    fsm.GetState("Place").Actions[1] = new RandomizerExecuteLambda(ActivateShiny);
                    break;
            }
        }

        private void ActivateShiny()
        {
            var obj = _parent.transform.Find(_shinyName).gameObject;
            // Make the Hunter listenable again after picking up the item, just like
            // in vanilla.
            var fsm = obj.LocateMyFSM("Shiny Control");
            fsm.GetState("Hero Up").AddFirstAction(
                new RandomizerExecuteLambda(() => PlayMakerFSM.BroadcastEvent("SHINY ITEM GET"))
            );
            // Force the replacement shiny to fling.
            var fling = fsm.GetState("Fling?");
            fling.ClearTransitions();
            fling.AddTransition("FINISHED", "Fling L");
            obj.SetActive(true);
        }
    }
}