using RandomizerMod.Extensions;
using RandomizerMod.FsmStateActions;
using UnityEngine;

namespace RandomizerMod.Actions
{
    public class ReplaceVengefulSpiritWithShiny : RandomizerAction
    {
        private string _sceneName;
        private string _shinyName;
        private string _location;

        public ReplaceVengefulSpiritWithShiny(string sceneName, string shinyName, string location)
        {
            _sceneName = sceneName;
            _shinyName = shinyName;
            _location = location;
        }

        public override ActionType Type => ActionType.PlayMakerFSM;

        public override void Process(string scene, Object changeObj)
        {
            if (!(scene == _sceneName && changeObj is PlayMakerFSM fsm && fsm.FsmName == "Conversation Control"))
            {
                return;
            }

            if (fsm.gameObject.name == "Shaman Meeting")
            {
                var checkActive = fsm.GetState("Check Active");
                checkActive.Actions[2] = new RandomizerExecuteLambda(() => fsm.SendEvent(RandomizerMod.Instance.Settings.CheckLocationFound(_location) ? null : "FINISHED"));
                checkActive.AddFirstAction(new RandomizerExecuteLambda(() => {
                    if (!RandomizerMod.Instance.Settings.CheckLocationFound(_location) && Ref.PD.GetInt("shaman") >= 1)
                    {
                        ActivateShiny();
                    }
                }));

                var spellAppear = fsm.GetState("Spell Appear");
                spellAppear.Actions[8] = new RandomizerExecuteLambda(ActivateShiny);
                spellAppear.Actions[9] = new RandomizerExecuteLambda(() => {});
            }
            else if (fsm.gameObject.name == "Shaman Trapped")
            {
                fsm.GetState("Check Active").Actions[2] = new RandomizerExecuteLambda(() => fsm.SendEvent(RandomizerMod.Instance.Settings.CheckLocationFound(_location) ? null : "DESTROY"));
            }
        }

        private void ActivateShiny()
        {
            // Can't use GameObject.Find because our shiny is inactive by default (as it replaces the
            // VS pickup)
            foreach (var obj in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (obj.name == _shinyName)
                {
                    obj.SetActive(true);
                }
            }
        }
    }
}