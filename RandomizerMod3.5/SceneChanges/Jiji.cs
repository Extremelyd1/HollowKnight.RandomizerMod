using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using RandomizerMod.Extensions;
using RandomizerMod.FsmStateActions;
using RandomizerMod.Randomization;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RandomizerMod.SceneChanges
{
    class Jiji
    {
        public static void JijiSceneEdits(Scene newScene)
        {
            if (newScene.name == SceneNames.Room_Ouiji)
            {
                if (RandomizerMod.Instance.Settings.EggShop)
                {
                    bool uncollectedJijiShinies = false;
                    foreach (string location in LogicManager.GetItemsByPool("EggShopLocation"))
                    {
                        if (!RandomizerMod.Instance.Settings.CheckLocationFound(location)
                            && Ref.PD.jinnEggsSold >= RandomizerMod.Instance.Settings.GetVariableCost(location))
                        {
                            uncollectedJijiShinies = true;
                            break;
                        }
                    }

                    if (uncollectedJijiShinies)
                    {
                        GameObject.Find("Jiji NPC").LocateMyFSM("Approach").GetState("Idle").ClearTransitions();
                    }

                    else
                    {
                        // Restore normal Jiji behaviour after all eggs have been sold
                        if (Ref.PD.jinnEggsSold < RandomizerMod.Instance.Settings.MaxEggCost)
                        {
                            GameObject jiji = GameObject.Find("Jiji NPC");

                            PlayMakerFSM jijiFsm = jiji.LocateMyFSM("Conversation Control");
                            EnableEggShop(jijiFsm);

                            {
                                // Set the "Black Wave" to white
                                // I think it'd be nice to change the particles' color to white, but I couldn't figure out how to :/
                                GameObject blackWave = jiji.FindGameObjectInChildren("Black Wave");
                                blackWave.GetComponent<WaveEffectControl>().blackWave = false;
                                blackWave.GetComponent<SpriteRenderer>().color = Color.white;
                            }
                        }
                    }
                }
                else
                {
                    EnableJijiHints();
                }
            }
            else if (newScene.name == SceneNames.Room_Jinn)
            {
                FixJinn();
            }
        }



        public static void EnableEggShop(PlayMakerFSM jijiFsm)
        {
            #region Convo management
            FsmState convoChoice = jijiFsm.GetState("Convo Choice");
            convoChoice.RemoveActionsOfType<PlayerDataBoolTest>();
            convoChoice.RemoveActionsOfType<BoolTest>();
            convoChoice.RemoveActionsOfType<PlayerDataBoolTrueAndFalse>();
            convoChoice.RemoveActionsOfType<SendEvent>();
            convoChoice.AddTransition("FINISHED", "Greet");
            // Always display the Jiji:GREET convo
            FsmState greet = jijiFsm.GetState("Greet");
            greet.ClearTransitions();
            greet.AddTransition("CONVO_FINISH", "Offer");
            // Always display the Jiji:SHADE_OFFER convo
            // Display the Prompts:JIJI_OFFER YN prompt
            // No: Jiji:DECLINE

            FsmState yesState = jijiFsm.GetState("Yes");
            yesState.RemoveActionsOfType<SetPlayerDataString>();
            yesState.RemoveActionsOfType<SetPlayerDataInt>();
            yesState.RemoveActionsOfType<SetPlayerDataFloat>();
            yesState.RemoveActionsOfType<PlayerDataIntAdd>();
            yesState.AddFirstAction(new RandomizerExecuteLambda(() =>
            {
                int eggs = Ref.PD.GetInt(nameof(PlayerData.rancidEggs));

                if (eggs > RandomizerMod.Instance.Settings.MaxEggCost - Ref.PD.jinnEggsSold)
                {
                    eggs = RandomizerMod.Instance.Settings.MaxEggCost - Ref.PD.jinnEggsSold;
                }

                Ref.PD.IntAdd(nameof(PlayerData.rancidEggs), -eggs);
                Ref.PD.IntAdd(nameof(PlayerData.jinnEggsSold), eggs);
            }));
            // Jiji:RITUAL_BEGIN
            #endregion

            #region Shiny Spawning
            FsmState spawn = jijiFsm.GetState("Spawn");
            spawn.RemoveActionsOfType<CreateObject>();
            spawn.AddAction(new RandomizerExecuteLambda(() =>
            {
                foreach (Transform shinytransform in GameObject.Find("Spawn Point").transform)
                {
                    GameObject shiny = shinytransform.gameObject;

                    if (!shiny.name.StartsWith("Randomizer Shiny ")) continue;
                    string location = shiny.name.Substring(17);

                    if (Ref.PD.jinnEggsSold >= RandomizerMod.Instance.Settings.GetVariableCost(location))
                    {
                        shiny.SetActive(true);
                    }
                }
            }));
            #endregion
        }


        // Enable Jiji hints when the player does not have a shade
        public static void EnableJijiHints()
        {
            if (PlayerData.instance.shadeScene != "None")
            {
                PlayMakerFSM jijiFsm = GameObject.Find("Jiji NPC").LocateMyFSM("Conversation Control");
                FsmState HasShade = jijiFsm.GetState("Has Shade?");
                HasShade.RemoveTransitionsTo("Check Location");
                HasShade.AddTransition("YES", "Offer");
            }
            else if (RandomizerMod.Instance.Settings.Jiji)
            {
                PlayerData.instance.SetString("shadeMapZone", "HIVE");
                PlayMakerFSM jijiFsm = GameObject.Find("Jiji NPC").LocateMyFSM("Conversation Control");
                FsmState BoxUp = jijiFsm.GetState("Box Up");
                BoxUp.ClearTransitions();
                BoxUp.AddFirstAction(jijiFsm.GetState("Convo Choice").GetActionsOfType<GetPlayerDataInt>()[0]);
                BoxUp.AddTransition("FINISHED", "Offer");
                FsmState SendText = jijiFsm.GetState("Send Text");
                SendText.RemoveTransitionsTo("Yes");
                SendText.AddTransition("YES", "Check Location");
                FsmState CheckLocation = jijiFsm.GetState("Check Location");
                CheckLocation.AddFirstAction(BoxUp.GetActionsOfType<SendEventByName>()[0]);
                CheckLocation.AddFirstAction(jijiFsm.GetState("Convo Choice").GetActionsOfType<GetPlayerDataInt>()[0]);
                CheckLocation.AddFirstAction(jijiFsm.GetState("Yes").GetActionsOfType<PlayerDataIntAdd>()[0]);
                CheckLocation.AddFirstAction(jijiFsm.GetState("Yes").GetActionsOfType<SendEventByName>()[0]);
            }
        }

        public static void FixJinn()
        {
            GameObject Jinn = GameObject.Find("Jinn NPC");

            // Destroy Jinn if Jiji isn't bought out
            if (RandomizerMod.Instance.Settings.MaxEggCost > Ref.PD.jinnEggsSold)
            {
                Object.Destroy(Jinn);
            }
            else
            {
                FsmState transaction = Jinn.LocateMyFSM("Conversation Control").GetState("Transaction");
                transaction.RemoveActionsOfType<RandomInt>();
                transaction.RemoveActionsOfType<CallMethodProper>();
                transaction.AddFirstAction(new RandomizerExecuteLambda(() => HeroController.instance.AddGeo(450)));

                // Jinn Sell All
                if (RandomizerMod.Instance.Settings.JinnSellAll)
                {
                    PlayMakerFSM fsm = Jinn.FindGameObjectInChildren("Talk NPC").LocateMyFSM("Conversation Control");
                    fsm.GetState("Talk Finish").AddFirstAction(new RandomizerExecuteLambda(() =>
                    {
                        int n = Ref.PD.GetInt(nameof(Ref.PD.rancidEggs));
                        if (n > 0)
                        {
                            Ref.Hero.AddGeo(450 * n);
                            Ref.PD.SetInt(nameof(Ref.PD.rancidEggs), Ref.PD.GetInt(nameof(Ref.PD.rancidEggs)) - n);
                            Ref.PD.SetInt(nameof(Ref.PD.jinnEggsSold), Ref.PD.GetInt(nameof(Ref.PD.jinnEggsSold)) + n);
                        }
                    }));
                }
            }
        }

        public static void KillTuk(PlayMakerFSM fsm)
        {
            if (!RandomizerMod.Instance.Settings.EggShop) return;

            if (fsm.gameObject.scene.name == SceneNames.Waterways_03 && fsm.FsmName == "Steel Soul")
            {
                fsm.GetState("Pause").RemoveTransitionsTo("Check");

                if (fsm.gameObject.name.StartsWith("Alive"))
                {
                    fsm.GetState("Pause").AddTransition("FINISHED", "Inactive");
                }
            }
        }
    }
}
