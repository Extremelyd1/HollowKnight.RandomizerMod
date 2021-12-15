using System.Linq;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using RandomizerMod.Extensions;
using RandomizerMod.FsmStateActions;
using RandomizerMod.Randomization;
using TMPro;
using UnityEngine;

namespace RandomizerMod.Actions
{
    public class ShowLoreTextInShop : RandomizerAction
    {

        private readonly string _fsmName;
        private readonly string _objectName;
        private readonly string _sceneName;

        public ShowLoreTextInShop(string sceneName, string objectName, string fsmName)
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

            // Begin showing lore state
            FsmState startReading = fsm.GetState("Trink 1");
            startReading.ClearTransitions();
            startReading.RemoveActionsOfType<FsmStateAction>();

            // Yeeting the shop menu probably isn't ideal; however, we can't close it because the shop menu's descendant is carrying 
            // this FSM that's showing the lore. So welcome to spaghetti-land I guess
            startReading.AddAction(new RandomizerExecuteLambda(() => {
                GameObject.Find("Shop Menu").transform.SetPositionY(200);
            }));

            startReading.AddAction(new RandomizerExecuteLambda(() => GameObject.Find("DialogueManager")
                .LocateMyFSM("Box Open").SendEvent("BOX UP")));
            startReading.AddAction(new Wait()
            {
                time = 0.3f,
                finishEvent = FsmEvent.Finished
            });


            // Reading
            FsmState loreReading = new FsmState(fsm.GetState("No"))
            {
                Name = "Lore Reading"
            };
            loreReading.ClearTransitions();
            loreReading.RemoveActionsOfType<FsmStateAction>();

            loreReading.AddAction(new RandomizerExecuteLambda(() => {
                GameObject dialogueManager = GameObject.Find("DialogueManager");
                GameObject textObj = dialogueManager.transform.Find("Text").gameObject;

                // Extract the parameters of the shown lore
                ReqDef loredef = LogicManager.GetItemDef(fsm.FsmVariables.StringVariables.First(v => v.Name == "PD Bool Name").Value.Split('.')[2]);
                string key = loredef.loreKey;
                string sheet = string.IsNullOrEmpty(loredef.loreSheet) ? "Lore Tablets" : loredef.loreSheet;
                TextAlignmentOptions align = loredef.textType == ChangeShinyIntoText.TextType.LeftLore
                    ? TextAlignmentOptions.TopLeft : TextAlignmentOptions.Top;

                textObj.GetComponent<TextMeshPro>().alignment = align;
                textObj.GetComponent<DialogueBox>().StartConversation(key, sheet);
            }));

            
            // Finished Reading
            FsmState finishReading = new FsmState(fsm.GetState("No"))
            {
                Name = "Lore Finish Reading"
            };
            finishReading.ClearTransitions();
            finishReading.RemoveActionsOfType<FsmStateAction>();

            finishReading.AddAction(new RandomizerExecuteLambda(() => {
                GameObject dialogueManager = GameObject.Find("DialogueManager");
                GameObject textObj = dialogueManager.transform.Find("Text").gameObject;
                dialogueManager.LocateMyFSM("Box Open").SendEvent("BOX DOWN");
                textObj.GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.TopLeft;
            }));
            // Add a useless wait here; this is basically just to give the dialogue box time to disappear before returning the shop menu.
            // The time value isn't a special number; I just found that it seemed to work well.
            finishReading.AddAction(new Wait() { 
                time = 0.15f,
                finishEvent = FsmEvent.Finished
            });

            // Return the shop menu to its rightful position
            fsm.GetState("Reset").AddFirstAction(new RandomizerExecuteLambda(() => {
                GameObject.Find("Shop Menu").transform.SetPositionY(0.5f);
            }));

            // Adding states
            startReading.AddTransition("FINISHED", loreReading.Name);
            loreReading.AddTransition("CONVO_FINISH", finishReading.Name);
            finishReading.AddTransition("FINISHED", "Reset");

            fsm.AddState(loreReading);
            fsm.AddState(finishReading);

        }
    }
}
