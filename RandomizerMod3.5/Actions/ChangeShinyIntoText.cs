using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using RandomizerMod.Extensions;
using RandomizerMod.FsmStateActions;
using TMPro;
using UnityEngine;

namespace RandomizerMod.Actions
{
    public class ChangeShinyIntoText : RandomizerAction
    {
        public enum TextType
        {
            LeftLore,         // Some lore tablets (the Lurien tablet) have their text left aligned
            Lore,             // Normal Lore tablet (text is top-centre - applies to most, but not all, of the tablets)
            MajorLore         // "Major" Lore tablet (bring up the lore background, etc)
        }

        private readonly string _item;
        private readonly string _fsmName;
        private readonly string _objectName;
        private readonly string _sceneName;
        private readonly string _location;
        private readonly string _key;
        private readonly string _sheetTitle;
        private readonly TextType _textType;

        public ChangeShinyIntoText(string sceneName, string objectName, string fsmName, string key, string sheetTitle, 
            TextType textType, string item, string location)
        {
            _sceneName = sceneName;
            _objectName = objectName;
            _fsmName = fsmName;
            _item = item;
            _location = location;
            _key = key;
            _sheetTitle = sheetTitle;
            _textType = textType;
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
            FsmState getCharm = fsm.GetState("Get Charm");

            // Remove actions that stop shiny from spawning
            pdBool.RemoveActionsOfType<PlayerDataBoolTest>();
            pdBool.RemoveActionsOfType<StringCompare>();

            // Add our own check to stop the shiny from being grabbed twice
            pdBool.AddAction(
                new RandomizerExecuteLambda(() => fsm.SendEvent(
                    RandomizerMod.Instance.Settings.CheckLocationFound(_location) ? "COLLECTED" : null
                    )));

            // The "Charm?" state is a bad entry point for our lore showing
            charm.ClearTransitions();
            charm.AddTransition("FINISHED", "Get Charm");
            // The "Get Charm" state is a good entry point for our lore showing
            getCharm.RemoveActionsOfType<SetPlayerDataBool>();
            getCharm.RemoveActionsOfType<IncrementPlayerDataInt>();
            getCharm.RemoveActionsOfType<SendMessage>();

            getCharm.AddAction(new RandomizerExecuteLambda(() => GiveItemActions.GiveItem(GiveItemActions.GiveAction.None, _item, _location)));
            getCharm.ClearTransitions();

            // Begin showing lore state
            FsmState startReading = new FsmState(fsm.GetState("Idle"))
            {
                Name = "Lore Start Reading"
            };
            startReading.ClearTransitions();
            startReading.RemoveActionsOfType<FsmStateAction>();

            if (_textType == TextType.MajorLore)
            {
                startReading.AddAction(new RandomizerExecuteLambda(() => {
                    AudioSource audio = fsm.gameObject.GetComponent<AudioSource>();
                    audio.PlayOneShot(ObjectCache.LoreSound);
                }));

                startReading.AddAction(new RandomizerExecuteLambda(() => PlayMakerFSM.BroadcastEvent("LORE PROMPT UP")));
            }
            else
            {
                startReading.AddAction(new RandomizerExecuteLambda(() => GameObject.Find("DialogueManager")
                    .LocateMyFSM("Box Open").SendEvent("BOX UP")));
            }
            startReading.AddAction(new Wait()
            {
                time = _textType == TextType.MajorLore ? 0.85f : 0.3f,
                finishEvent = FsmEvent.Finished
            });

            // Reading
            FsmState loreReading = new FsmState(fsm.GetState("Idle"))
            {
                Name = "Lore Reading"
            };
            loreReading.ClearTransitions();
            loreReading.RemoveActionsOfType<FsmStateAction>();

            loreReading.AddAction(new RandomizerCallStaticMethod(GetType(), nameof(ShowLoreDialogue), fsm.gameObject,
                _key, _sheetTitle, _textType));

            // Finished Reading
            FsmState finishReading = new FsmState(fsm.GetState("Idle"))
            {
                Name = "Lore Finish Reading"
            };
            finishReading.ClearTransitions();
            finishReading.RemoveActionsOfType<FsmStateAction>();

            finishReading.AddAction(new RandomizerCallStaticMethod(GetType(), nameof(HideLoreDialogue), _textType));
            if (_textType == TextType.MajorLore) finishReading.AddAction(new Wait()
            {
                time = 0.5f,
                finishEvent = FsmEvent.Finished
            });

            // Once we're done we have to reset the text box
            fsm.GetState("Flash").AddFirstAction(new RandomizerCallStaticMethod(GetType(), nameof(ResetTextBox)));

            // Cancel Reading (Hero Damaged)
            FsmState cancelReading = new FsmState(fsm.GetState("Idle"))
            {
                Name = "Lore Cancel Reading"
            };
            cancelReading.ClearTransitions();
            cancelReading.RemoveActionsOfType<FsmStateAction>();

            cancelReading.AddAction(new RandomizerCallStaticMethod(GetType(), nameof(HideLoreDialogue), _textType));
            cancelReading.AddAction(new RandomizerCallStaticMethod(GetType(), nameof(ResetTextBox)));
            // The code in "Finish" doesn't yeet the inspect region - we can do it here just by doing this
            cancelReading.AddAction(new RandomizerExecuteLambda(() => Object.Destroy(fsm.gameObject)));

            // Adding states
            getCharm.AddTransition("FINISHED", startReading.Name);
            startReading.AddTransition("FINISHED", loreReading.Name);
            startReading.AddTransition("HERO DAMAGED", cancelReading.Name);
            loreReading.AddTransition("CONVO_FINISH", finishReading.Name);
            loreReading.AddTransition("HERO DAMAGED", cancelReading.Name);
            finishReading.AddTransition("FINISHED", "Flash");
            finishReading.AddTransition("HERO DAMAGED", cancelReading.Name);
            cancelReading.AddTransition("FINISHED", "Finish");

            fsm.AddState(startReading);
            fsm.AddState(loreReading);
            fsm.AddState(finishReading);
            fsm.AddState(cancelReading);

        }


        private static void ShowLoreDialogue(GameObject shiny, string key, string sheetTitle, TextType textType)
        {
            GameObject dialogueManager = GameObject.Find("DialogueManager");
            GameObject textObj = dialogueManager.transform.Find("Text").gameObject;
            textObj.LocateMyFSM("Dialogue Page Control").FsmVariables.GetFsmGameObject("Requester").Value = shiny;

            // Set position of text box
            if (textType == TextType.MajorLore)
            {
                textObj.transform.SetPositionY(2.44f);
                dialogueManager.transform.Find("Stop").gameObject.transform.SetPositionY(-0.23f);
                dialogueManager.transform.Find("Arrow").gameObject.transform.SetPositionY(-0.3f);
            }

            switch (textType)
            {
                default:
                case TextType.LeftLore:
                    break;

                case TextType.Lore:
                case TextType.MajorLore:
                    textObj.GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.Top;
                    break;
            }

            textObj.GetComponent<DialogueBox>().StartConversation(key, sheetTitle);
        }

        private static void HideLoreDialogue(TextType textType)
        {
            GameObject dialogueManager = GameObject.Find("DialogueManager");
            switch (textType)
            {
                default:
                case TextType.LeftLore:
                case TextType.Lore:
                    dialogueManager.LocateMyFSM("Box Open").SendEvent("BOX DOWN");
                    break;

                case TextType.MajorLore:
                    PlayMakerFSM.BroadcastEvent("LORE PROMPT DOWN");
                    break;
            } 
        }

        private static void ResetTextBox()
        {
            GameObject dialogueManager = GameObject.Find("DialogueManager");

            GameObject textObj = dialogueManager.transform.Find("Text").gameObject;
            textObj.GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.TopLeft;
            dialogueManager.transform.Find("Arrow").gameObject.transform.SetPositionY(1.695f);
            textObj.transform.SetPositionY(4.49f);
            dialogueManager.transform.Find("Stop").gameObject.transform.SetPositionY(1.695f);
        }
    }
}
