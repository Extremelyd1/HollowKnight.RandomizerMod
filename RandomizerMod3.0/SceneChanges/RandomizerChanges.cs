using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using RandomizerMod.Components;
using RandomizerMod.Extensions;
using RandomizerMod.FsmStateActions;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace RandomizerMod.SceneChanges
{
    internal static partial class SceneEditor
    {
        /*
         * Changes that prevent clawless softlocks, mostly
         */

        public static void ExtraPlatforms(Scene newScene)
        {
            if (!RandomizerMod.Instance.Settings.ExtraPlatforms) return;

            switch (newScene.name)
            {
                // Platforms to climb out from basin wanderer's journal
                case SceneNames.Abyss_02:
                    {
                        GameObject platform = ObjectCache.SmallPlatform;
                        platform.transform.SetPosition2D(128.3f, 7f);
                        platform.SetActive(true);
                    }
                    {
                        GameObject platform = ObjectCache.SmallPlatform;
                        platform.transform.SetPosition2D(128.3f, 11f);
                        platform.SetActive(true);
                    }
                    break;

                // Platforms to climb up to tram in basin from left with no items
                case SceneNames.Abyss_03 when !RandomizerMod.Instance.Settings.RandomizeRooms:
                    {
                        GameObject platform = ObjectCache.SmallPlatform;
                        platform.transform.SetPosition2D(34f, 7f);
                        platform.SetActive(true);
                    }
                    break;

                // Platform to climb out of Abyss with only wings
                case SceneNames.Abyss_06_Core:
                    {
                        GameObject platform = ObjectCache.SmallPlatform;
                        platform.transform.SetPosition2D(88.6f, 263f);
                        platform.SetActive(true);
                    }
                    break;

                // Platforms to climb back up from pale ore with no items
                case SceneNames.Abyss_17:
                    {
                        GameObject platform = ObjectCache.SmallPlatform;
                        platform.transform.SetPosition2D(164.7f, 30f);
                        platform.SetActive(true);
                    }
                    {
                        GameObject platform = ObjectCache.SmallPlatform;
                        platform.transform.SetPosition2D(99.5f, 12.5f);
                        platform.SetActive(true);
                    }
                    {
                        GameObject platform = ObjectCache.SmallPlatform;
                        platform.transform.SetPosition2D(117.7f, 18.8f);
                        platform.SetActive(true);
                    }
                    {
                        GameObject platform = ObjectCache.SmallPlatform;
                        platform.transform.SetPosition2D(114.3f, 23f);
                        platform.SetActive(true);
                    }
                    {
                        GameObject platform = ObjectCache.SmallPlatform;
                        platform.transform.SetPosition2D(117.7f, 7f);
                        platform.SetActive(true);
                    }
                    {
                        GameObject platform = ObjectCache.SmallPlatform;
                        platform.transform.SetPosition2D(117.7f, 10.8f);
                        platform.SetActive(true);
                    }
                    break;
                // Platforms to remove softlock with wings at simple key in basin
                case SceneNames.Abyss_20:
                    {
                        GameObject platform = ObjectCache.SmallPlatform;
                        platform.transform.SetPosition2D(26.5f, 13f);
                        platform.transform.SetScaleX(.8f);
                        platform.SetActive(true);
                    }
                    break;

                // Platform for returning to Gorb landing
                case SceneNames.Cliffs_02:
                    {
                        GameObject plat = ObjectCache.SmallPlatform;
                        plat.transform.position = new Vector3(32.3f, 27.7f);
                        plat.SetActive(true);
                    }
                    break;

                // Platform to return from Deepnest mimic grub room
                case SceneNames.Deepnest_01b when !RandomizerMod.Instance.Settings.RandomizeRooms:
                    {
                        GameObject platform = ObjectCache.SmallPlatform;
                        platform.transform.SetPosition2D(48.3f, 40f);
                        platform.SetActive(true);
                    }
                    break;

                // Platforms to return from the Deepnest_02 geo rocks without vertical
                case SceneNames.Deepnest_02:
                    {
                        GameObject platform = ObjectCache.SmallPlatform;
                        platform.transform.SetPosition2D(26f, 12f);
                        platform.SetActive(true);
                    }
                    {
                        GameObject platform = ObjectCache.SmallPlatform;
                        platform.transform.SetPosition2D(26f, 16f);
                        platform.SetActive(true);
                    }
                    break;

                case SceneNames.Deepnest_36 when RandomizerMod.Instance.Settings.RandomizeMimics:
                    {
                        GameObject platform = ObjectCache.SmallPlatform;
                        platform.transform.SetPosition2D(26f, 11f);
                        platform.SetActive(true);
                    }
                    break;

                // Platforms to climb back up from Mantis Lords with only wings
                case SceneNames.Fungus2_15 when !RandomizerMod.Instance.Settings.RandomizeTransitions:
                    {
                        GameObject[] platforms = new GameObject[2];
                        for (int i = 0; i < 2; i++)
                        {
                            platforms[i] = ObjectCache.SmallPlatform;
                            platforms[i].transform.SetPosition2D(48f + 2 * i, 10f * i + 15f);
                            platforms[i].SetActive(true);
                        }
                    }
                    break;

                // Platforms to prevent softlock on lever on the way to love key. Didn't need as many as I expected
                case SceneNames.Fungus3_05:
                    for (int i = 0; i < 2; i++)
                    {
                        GameObject platform = ObjectCache.SmallPlatform;
                        platform.transform.SetPosition2D(65.7f, 11f + 4.5f * i);
                        platform.SetActive(true);
                    }
                    break;

                // Move the load in colo downward to prevent bench soft lock
                case SceneNames.Room_Colosseum_02 when !RandomizerMod.Instance.Settings.RandomizeTransitions:
                    GameObject coloTransition1 = GameObject.Find("top1");
                    coloTransition1.transform.SetPositionY(coloTransition1.transform.position.y - 9f);
                    break;

                // Platform to escape from the geo rock above Lemm
                case SceneNames.Ruins1_05 + "c":
                    {
                        GameObject platform = ObjectCache.SmallPlatform;
                        platform.transform.SetPosition2D(26.6f, 73.2f);
                        platform.SetActive(true);
                    }
                    break;

                // Platforms to climb back up to King's Pass with no items
                case SceneNames.Town when !RandomizerMod.Instance.Settings.RandomizeTransitions && RandomizerMod.Instance.Settings.StartName == "King's Pass":
                    {
                        GameObject[] platforms = new GameObject[6];
                        for (int i = 0; i < 6; i++)
                        {
                            platforms[i] = ObjectCache.SmallPlatform;
                            platforms[i].transform.SetPosition2D(20f - 2 * (i%2), 5f * i + 15f);
                            platforms[i].SetActive(true);
                        }
                    }
                    break;

                // Platforms to prevent itemless softlock when checking left waterways
                case SceneNames.Waterways_04 when
                !RandomizerMod.Instance.Settings.RandomizeTransitions
                && RandomizerMod.Instance.Settings.StartName != "West Waterways":
                    {
                        GameObject[] platforms = new GameObject[2];
                        platforms[0] = ObjectCache.SmallPlatform;
                        platforms[0].transform.SetPosition2D(148f, 23.1f);
                        platforms[0].SetActive(true);

                        platforms[1] = ObjectCache.SmallPlatform;
                        platforms[1].transform.SetPosition2D(139f, 32f);
                        platforms[1].SetActive(true);
                    }

                    if (!RandomizerMod.Instance.Settings.RandomizeSwim || RandomizerMod.Instance.Settings.GetBool(name: "canSwim"))
                    {
                        GameObject[] platforms = new GameObject[2];
                        platforms[0] = ObjectCache.SmallPlatform;
                        platforms[0].transform.SetPosition2D(107f, 10f);
                        platforms[0].SetActive(true);

                        platforms[1] = ObjectCache.SmallPlatform;
                        platforms[1].transform.SetPosition2D(107f, 15f);
                        platforms[1].SetActive(true);
                    }
                    break;
            }
        }


        /*
         * Room changes required for the randomizer to function on any mode
         * For example, removing certain vanilla item locations which can't be handled by RandomizerAction
         */

        public static void ApplyRandomizerChanges(Scene newScene)
        {
            switch (newScene.name)
            {
                // Prevent Grimm encounter which gives Grimmchild
                case SceneNames.Grimm_Main_Tent:
                    PlayerData.instance.metGrimm = true;
                    break;

                /* We don't need to disable the lore tablets here because we're doing so in RandomizerAction
                
                // Prevent reading focus tablet when focus is randomized
                // With lore tablets randomized we skip this because we're destroying the tablet completely
                case SceneNames.Tutorial_01 when RandomizerMod.Instance.Settings.RandomizeFocus && !RandomizerMod.Instance.Settings.RandomizeLoreTablets:
                    GameObject.Find("Tut_tablet_top").LocateMyFSM("Inspection").GetState("Init").ClearTransitions();
                    break;

                // Prevent reading tablet which gives completion percentage
                // With lore tablets randomized we skip this because we're destroying the tablet completely
                case SceneNames.Room_Final_Boss_Atrium when RandomizerMod.Instance.Settings.RandomizeDreamers && !RandomizerMod.Instance.Settings.RandomizeLoreTablets:
                    GameObject.Find("Tut_tablet_top").LocateMyFSM("Inspection").GetState("Init").ClearTransitions();
                    break;
                */

                // Removes the prompt to donate to the 3000 geo fountain in Basin
                case SceneNames.Abyss_04 when !RandomizerMod.Instance.Settings.NPCItemDialogue:
                    Object.Destroy(GameObject.Find("Fountain Donation"));
                    break;

                // Opens lifeblood door in Abyss with any amount of blue health
                case SceneNames.Abyss_06_Core:
                    if (PlayerData.instance.healthBlue > 0 || PlayerData.instance.joniHealthBlue > 0 || GameManager.instance.entryGateName == "left1")
                    {
                        PlayerData.instance.SetBoolInternal("blueVineDoor", true);
                        PlayMakerFSM BlueDoorFSM = GameObject.Find("Blue Door").LocateMyFSM("Control");
                        BlueDoorFSM.GetState("Init").RemoveTransitionsTo("Got Charm");
                    }
                    break;

                // Removes trigger for Void Heart sequence
                case SceneNames.Abyss_15:
                    GameObject.Find("Dream Enter Abyss").LocateMyFSM("Control").GetState("Init").RemoveTransitionsTo("Idle");
                    GameObject.Find("Dream Enter Abyss").LocateMyFSM("Control").GetState("Init").AddTransition("FINISHED", "Inactive");
                    break;

                /* We'll unlock godseeker in startsavechanges and remove the simple key in AddYNDialogueToShiny
                // Automatically unlock Godseeker and add an action to the Godtuner spot to remove simple key on purchase
                case SceneNames.GG_Waterways:
                    PlayerData.instance.SetBool("godseekerUnlocked", true);
                    if (GameObject.Find("Randomizer Shiny") != null)
                    {
                        PlayMakerFSM godtuner = GameObject.Find("Randomizer Shiny").LocateMyFSM("Shiny Control");
                        godtuner.GetState(godtuner.GetState("Charm?").Transitions.First(t => t.EventName == "YES").ToState).AddFirstAction(new RandomizerExecuteLambda(() => PlayerData.instance.DecrementInt("simpleKeys")));
                    }
                    break;
                */

                // Spawns mawlek shard out of bounds and moves it inbounds when mawlek is killed
                case SceneNames.Crossroads_09:
                    if (GameObject.Find("Randomizer Shiny") is GameObject mawlekShard)
                    {
                        mawlekShard.transform.SetPositionY(100f);
                        IEnumerator mawlekDead()
                        {
                            yield return new WaitUntil(() => PlayerData.instance.killedMawlek);
                            mawlekShard.transform.SetPositionY(10f);
                            mawlekShard.transform.SetPositionX(61.5f);
                        }
                        GameManager.instance.StartCoroutine(mawlekDead());
                    }
                    break;

                // Removes Grubfather rewards corresponding to randomizer items
                case SceneNames.Crossroads_38:
                    Object.Destroy(GameObject.Find("Reward 5"));  //Mask
                    Object.Destroy(GameObject.Find("Reward 10")); //Charm
                    Object.Destroy(GameObject.Find("Reward 16")); //Rancid Egg
                    Object.Destroy(GameObject.Find("Reward 23")); //Relic
                    Object.Destroy(GameObject.Find("Reward 31")); //Pale Ore
                    Object.Destroy(GameObject.Find("Reward 38")); //Relic
                    Object.Destroy(GameObject.Find("Reward 46")); //Charm
                    break;

                // Remove gate from Ancestral Mound
                case SceneNames.Crossroads_ShamanTemple:
                    Object.Destroy(GameObject.Find("Bone Gate"));

                    // Destroy planks in cursed nail mode because we can't slash them
                    if (RandomizerMod.Instance.Settings.CursedNail && RandomizerMod.Instance.Settings.StartName == "Ancestral Mound")
                    {
                        DestroyAllObjectsNamed("Plank");
                    }
                    break;

                // Remove Beast's Den hardsave, allow rear access from entrance, destroy Herrah
                case SceneNames.Deepnest_Spider_Town:
                    SavePersistentBoolItemState(new PersistentBoolData
                    {
                        sceneName = "Deepnest_Spider_Town",
                        id = "Collapser Small (12)",
                        activated = true,
                        semiPersistent = false
                    });

                    FsmState denHardSave = GameObject.Find("RestBench Spider").LocateMyFSM("Fade").GetState("Land");
                    denHardSave.RemoveActionsOfType<CallMethodProper>();
                    denHardSave.RemoveActionsOfType<SendMessage>();
                    denHardSave.RemoveActionsOfType<SetPlayerDataBool>();

                    Object.Destroy(GameObject.Find("Dreamer Hegemol"));
                    Object.Destroy(GameObject.Find("Dream Enter"));
                    Object.Destroy(GameObject.Find("Dream Impact"));
                    Object.Destroy(GameObject.Find("Shield"));
                    break;



                // Edits Dream Nail location to change scene to seer and
                // re-enable minion charms
                case SceneNames.Dream_Nailcollection:
                    FsmState finish = FSMUtility.LocateFSM(GameObject.Find("Randomizer Shiny"), "Shiny Control").GetState("Finish");
                    finish.AddAction(new RandomizerChangeScene("RestingGrounds_07", "right1"));
                    finish.AddAction(new RandomizerExecuteLambda(() => {
                        FSMUtility.LocateFSM(HeroController.instance.gameObject, "ProxyFSM").Fsm.GetFsmBool("No Charms").Value = false;
                    }));
                    break;

                // Edit Hornet room to open gates after boss fight, and removes dreamer cutscene
                case SceneNames.Fungus1_04:
                    foreach (PlayMakerFSM childFSM in GameObject.Find("Cloak Corpse")
                        .GetComponentsInChildren<PlayMakerFSM>(true))
                    {
                        if (childFSM.FsmName == "Shiny Control")
                        {
                            SendEvent openGate = new SendEvent
                            {
                                eventTarget = new FsmEventTarget
                                {
                                    target = FsmEventTarget.EventTarget.BroadcastAll,
                                    excludeSelf = true
                                },
                                sendEvent = FsmEvent.FindEvent("BG OPEN"),
                                delay = 0,
                                everyFrame = false
                            };
                            childFSM.GetState("Destroy").AddFirstAction(openGate);
                            childFSM.GetState("Finish").AddFirstAction(openGate);

                            break;
                        }
                    }

                    ObjectDestroyer.Destroy("Dreamer Scene 1");
                    ObjectDestroyer.Destroy("Hornet Saver");
                    ObjectDestroyer.Destroy("Cutscene Dreamer");
                    ObjectDestroyer.Destroy("Dream Scene Activate");

                    if (!Ref.PD.hornet1Defeated)
                    {
                        Object.Destroy(FSMUtility.LocateFSM(GameObject.Find("Camera Locks Boss"), "FSM"));
                    }
                    break;

                // On easy difficulty, item/area rando, drop the vine platform blocking progress to the main part of GP
                // I think logically separating out the waterfall bench from GP is unpalatable as it makes seeds less
                // diverse; this pogo could probably be easy but people might not like that. I think this is probably
                // the least bad option
                case SceneNames.Fungus1_06: 
                    if (RandomizerMod.Instance.Settings.CursedNail
                        && !RandomizerMod.Instance.Settings.MildSkips
                        && !RandomizerMod.Instance.Settings.RandomizeRooms)
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Fungus1_06",
                            id = "Vine Platform (1)",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                
                // Make the PoP end cutscene not give a journal entry when those are randomized
                case SceneNames.White_Palace_20 when RandomizerMod.Instance.Settings.RandomizeJournalEntries:
                    var popFadeout = FSMUtility.LocateFSM(GameObject.Find("End Scene"), "Conversation Control").GetState("Fade Out");
                    popFadeout.ClearTransitions();
                    popFadeout.AddTransition("FINISHED", "New Scene");
                    break;

                // Destroy the Mantis Claw pickup when playing with split claw
                case SceneNames.Fungus2_14 when RandomizerMod.Instance.Settings.RandomizeClawPieces:
                    Object.Destroy(newScene.FindGameObject("Shiny Item Stand"));
                    break;

                // Make city crest gate openable infinite times and not hard save
                case SceneNames.Fungus2_21:
                    FSMUtility.LocateFSM(GameObject.Find("City Gate Control"), "Conversation Control")
                        .GetState("Activate").RemoveActionsOfType<SetPlayerDataBool>();

                    FsmState gateSlam = FSMUtility.LocateFSM(GameObject.Find("Ruins_gate_main"), "Open")
                        .GetState("Slam");
                    gateSlam.RemoveActionsOfType<SetPlayerDataBool>();
                    gateSlam.RemoveActionsOfType<CallMethodProper>();
                    gateSlam.RemoveActionsOfType<SendMessage>();
                    break;

                // Removes Leg Eater dialogue tree, preventing him from dying
                case SceneNames.Fungus2_26:
                    PlayMakerFSM legEater = FSMUtility.LocateFSM(GameObject.Find("Leg Eater"), "Conversation Control");
                    FsmState legEaterChoice = legEater.GetState("Convo Choice");
                    legEaterChoice.RemoveTransitionsTo("Convo 1");
                    legEaterChoice.RemoveTransitionsTo("Convo 2");
                    legEaterChoice.RemoveTransitionsTo("Convo 3");
                    legEaterChoice.RemoveTransitionsTo("Infected Crossroad");
                    legEaterChoice.RemoveTransitionsTo("Bought Charm");
                    legEaterChoice.RemoveTransitionsTo("Gold Convo");
                    legEaterChoice.RemoveTransitionsTo("All Gold");
                    legEaterChoice.RemoveTransitionsTo("Ready To Leave");
                    legEater.GetState("All Gold?").RemoveTransitionsTo("No Shop");
                    Ref.PD.legEaterLeft = false;
                    break;

                // Destroy Monomon and remove Quirrel encounter
                case SceneNames.Fungus3_archive_02:
                    PlayerData.instance.SetBool("summonedMonomon", true);
                    Object.Destroy(GameObject.Find("Quirrel Wounded"));
                    Object.Destroy(GameObject.Find("Quirrel"));
                    Object.Destroy(GameObject.Find("Monomon"));
                    Object.Destroy(GameObject.Find("Dream Enter"));
                    Object.Destroy(GameObject.Find("Dream Impact"));
                    Object.Destroy(GameObject.Find("Shield"));
                    break;

                // Destroys original lurker key. Moves new shiny out of bounds if lurker is alive and moves it inbounds when lurker is killed
                case "GG_Lurker":
                    if (PlayerData.instance.killedPaleLurker)
                    {
                        Object.Destroy(GameObject.Find("Shiny Item Key"));
                    }
                    else
                    {
                        newScene.FindGameObject("New Shiny").transform.SetPositionY(200f);
                        IEnumerator LurkerKilled()
                        {
                            yield return new WaitUntil(() => PlayerData.instance.killedPaleLurker || GameManager.instance.sceneName != "GG_Lurker");
                            yield return new WaitUntil(() => GameObject.Find("Shiny Item Key") is GameObject lurkerKey || GameManager.instance.sceneName != "GG_Lurker");
                            if (GameManager.instance.sceneName == "GG_Lurker")
                            {
                                Object.Destroy(GameObject.Find("Shiny Item Key"));
                                GameObject lurkerCorpse = Object.FindObjectsOfType<GameObject>().First(obj => obj.name.StartsWith("Corpse Pale Lurker")); // Corpse Pale Lurker(Clone)
                                GameObject.Find("New Shiny").transform.SetPosition2D(lurkerCorpse.transform.position);
                            }
                        }
                        GameManager.instance.StartCoroutine(LurkerKilled());
                    }
                    break;

                case SceneNames.GG_Waterways when RandomizerMod.Instance.Settings.RandomizeJunkPitChests:
                    // Simply replacing the object with shiny causes the shiny not to be flung
                    Object.Destroy(newScene.FindGameObject("lamp_bug_escape"));
                    break;

                case SceneNames.Hive_03 when RandomizerMod.Instance.Settings.StartName == "Hive":
                    GameObject hivePlatform = ObjectCache.SmallPlatform;
                    hivePlatform.transform.SetPosition2D(58.5f, 134f);
                    hivePlatform.SetActive(true);
                    // Extra platform in easy difficulty because the pogo is a mild skip
                    if (!RandomizerMod.Instance.Settings.MildSkips && !RandomizerMod.Instance.Settings.RandomizeStartItems)
                    {
                        GameObject hivePlatformEasy = ObjectCache.SmallPlatform;
                        hivePlatformEasy.transform.SetPosition2D(58.5f, 138.5f);
                        hivePlatformEasy.SetActive(true);
                    }
                    break;
                
                case SceneNames.Fungus1_13:
                    switch (RandomizerMod.Instance.Settings.StartName)
                    {
                        // Platforms for open mode
                        case "Far Greenpath":
                            {
                                GameObject leftGPQGplat = ObjectCache.SmallPlatform;
                                leftGPQGplat.transform.SetPosition2D(45f, 16.5f);
                                leftGPQGplat.SetActive(true);
                                GameObject rightGPQGplat = ObjectCache.SmallPlatform;
                                rightGPQGplat.transform.SetPosition2D(64f, 16.5f);
                                rightGPQGplat.SetActive(true);
                            }
                            SavePersistentBoolItemState(new PersistentBoolData
                            {
                                sceneName = "Fungus1_13",
                                id = "Vine Platform (1)",
                                activated = true,
                                semiPersistent = false
                            });
                            SavePersistentBoolItemState(new PersistentBoolData
                            {
                                sceneName = "Fungus1_13",
                                id = "Vine Platform (2)",
                                activated = true,
                                semiPersistent = false
                            });
                            break;
                        // With the Lower Greenpath start, getting to the rest of Greenpath requires
                        // cutting the vine to the right of the vessel fragment.
                        case "Lower Greenpath" when RandomizerMod.Instance.Settings.CursedNail:
                            SavePersistentBoolItemState(new PersistentBoolData
                            {
                                sceneName = "Fungus1_13",
                                id = "Vine Platform",
                                activated = true,
                                semiPersistent = false
                            });
                            break;
                    }
                    
                    break;

                // Bounce shrooms to prevent softlock for Fungal Core start in open mode without claw
                case SceneNames.Fungus2_30:
                    {
                        GameObject bounceShroom = GameObject.Find("Bounce Shroom C");

                        GameObject s0 = Object.Instantiate(bounceShroom);
                        s0.transform.SetPosition3D(12.5f, 26f, 0f);
                        s0.SetActive(true);

                        GameObject s1 = Object.Instantiate(bounceShroom);
                        s1.transform.SetPosition3D(12.5f, 54f, 0f);
                        s1.SetActive(true);

                        GameObject s2 = Object.Instantiate(bounceShroom);
                        s2.transform.SetPosition3D(21.7f, 133f, 0f);
                        s2.SetActive(true);
                    }
                    break;

                // We have to get rid of the Enraged Guardian Boss Geo reward if CG2 isn't present in the scene
                case SceneNames.Mines_32 when RandomizerMod.Instance.Settings.RandomizeBossGeo:
                    if (!Ref.PD.GetBool(nameof(Ref.PD.defeatedMegaBeamMiner)))
                    {
                        Object.Destroy(newScene.FindGameObject("New Shiny Boss Geo"));
                    }
                    break;

                // Make tolls always interactable, in the rare case that lantern is not randomized but RG access through the dark room is expected
                case SceneNames.Mines_33:
                    if (RandomizerMod.Instance.Settings.DarkRooms && !RandomizerMod.Instance.Settings.RandomizeKeys)
                    {
                        GameObject[] tolls = new GameObject[] { GameObject.Find("Toll Gate Machine"), GameObject.Find("Toll Gate Machine (1)") };
                        foreach (GameObject toll in tolls)
                        {
                            Object.Destroy(FSMUtility.LocateFSM(toll, "Disable if No Lantern"));
                        }
                    }
                    break;

                // Move Seer back to make room for items, and remove essence rewards
                case SceneNames.RestingGrounds_07:
                    GameObject.Find("Dream Moth").transform.Translate(new Vector3(-5f, 0f));

                    PlayMakerFSM moth = FSMUtility.LocateFSM(GameObject.Find("Dream Moth"), "Conversation Control");

                    PlayerData.instance.dreamReward1 = true;
                    moth.FsmVariables.GetFsmBool("Got Reward 1").Value = true;  //Relic
                    PlayerData.instance.dreamReward3 = true;
                    moth.FsmVariables.GetFsmBool("Got Reward 3").Value = true;  //Pale Ore
                    PlayerData.instance.dreamReward4 = true;
                    moth.FsmVariables.GetFsmBool("Got Reward 4").Value = true;  //Charm
                    PlayerData.instance.dreamReward5 = true;
                    moth.FsmVariables.GetFsmBool("Got Reward 5").Value = true;  //Vessel Fragment
                    PlayerData.instance.dreamReward5b = true;
                    moth.FsmVariables.GetFsmBool("Got Reward 5b").Value = true; //Skill
                    PlayerData.instance.dreamReward6 = true;
                    moth.FsmVariables.GetFsmBool("Got Reward 6").Value = true;  //Relic
                    PlayerData.instance.dreamReward7 = true;
                    moth.FsmVariables.GetFsmBool("Got Reward 7").Value = true;  //Mask Shard
                    PlayerData.instance.dreamReward8 = true;
                    moth.FsmVariables.GetFsmBool("Got Reward 8").Value = true;  //Skill
                    break;

                // Destroy lever in ApplyRandomizerChanges so we can make EditStagStations an FSM function
                case SceneNames.RestingGrounds_09 when RandomizerMod.Instance.Settings.RandomizeStags:
                    Object.Destroy(newScene.FindGameObject("Ruins Lever"));
                    break;

                // Make Sly pickup send Sly back upstairs -- warps player out to prevent resulting softlock from trying to enter the shop from a missing transition 
                case SceneNames.Room_Sly_Storeroom when !RandomizerMod.Instance.Settings.NPCItemDialogue:
                    FsmState slyFinish = FSMUtility.LocateFSM(GameObject.Find("Randomizer Shiny"), "Shiny Control").GetState("Finish");
                    slyFinish.AddAction(new RandomizerSetBool("SlyCharm", true));
                    slyFinish.AddAction(new RandomizerChangeScene("Town", "door_sly"));
                    break;

                // Destroy lever in ApplyRandomizerChanges so we can make EditStagStations an FSM function
                case SceneNames.Room_Town_Stag_Station when RandomizerMod.Instance.Settings.RandomizeStags:
                    foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
                    {
                        if (go.name.StartsWith("Gate Switch")) Object.Destroy(go);
                    }
                    break;

                // Many changes to make the desolate dive pickup work properly
                case SceneNames.Ruins1_24:
                    // Stop spell container from destroying itself
                    PlayMakerFSM quakePickup = FSMUtility.LocateFSM(GameObject.Find("Quake Pickup"), "Pickup");
                    quakePickup.GetState("Idle").RemoveActionsOfType<IntCompare>();
                    foreach (PlayMakerFSM childFSM in quakePickup.gameObject.GetComponentsInChildren<PlayMakerFSM>(true))
                    {
                        if (childFSM.FsmName == "Shiny Control")
                        {
                            // Make spell container spawn shiny instead
                            quakePickup.GetState("Appear").GetActionsOfType<ActivateGameObject>()[1].gameObject
                                .GameObject.Value = childFSM.gameObject;

                            // Make shiny open gates on pickup/destroy
                            SendEvent openGate = new SendEvent
                            {
                                eventTarget = new FsmEventTarget
                                {
                                    target = FsmEventTarget.EventTarget.BroadcastAll,
                                    excludeSelf = true
                                },
                                sendEvent = FsmEvent.FindEvent("BG OPEN"),
                                delay = 0,
                                everyFrame = false
                            };
                            childFSM.GetState("Destroy").AddFirstAction(openGate);
                            childFSM.GetState("Finish").AddFirstAction(openGate);
                            break;
                        }
                    }

                    // Stop the weird invisible floor from appearing if dive has been obtained
                    if (Ref.PD.quakeLevel > 0)
                    {
                        Object.Destroy(GameObject.Find("Roof Collider Battle"));
                    }

                    // Change battle gate to be destroyed if Soul Master is dead instead of it the player has quake
                    FsmState checkQuake = FSMUtility.LocateFSM(GameObject.Find("Battle Gate (1)"), "Destroy if Quake").GetState("Check");
                    checkQuake.RemoveActionsOfType<FsmStateAction>();
                    checkQuake.AddAction(new RandomizerBoolTest(nameof(PlayerData.killedMageLord), null, "DESTROY", true));
                    break;

                // Prevent simple key softlocks
                case SceneNames.Ruins2_04:
                    FsmState hotSpringsKey = GameObject.Find("Inspect").LocateMyFSM("Conversation Control").GetState("Got Key?");
                    hotSpringsKey.RemoveActionsOfType<IntCompare>();
                    hotSpringsKey.AddAction(new RandomizerExecuteLambda(() =>
                    {
                        if (GameManager.instance.GetPlayerDataInt("simpleKeys") > 1 || (PlayerData.instance.openedWaterwaysManhole && GameManager.instance.GetPlayerDataInt("simpleKeys") > 0)) PlayMakerFSM.BroadcastEvent("YES");
                        else PlayMakerFSM.BroadcastEvent("NO");
                    }));
                    break;

                // Destroy Lurien
                case SceneNames.Ruins2_Watcher_Room:
                    Object.Destroy(GameObject.Find("Dreamer Lurien"));
                    Object.Destroy(GameObject.Find("Dream Enter"));
                    Object.Destroy(GameObject.Find("Dream Impact"));
                    Object.Destroy(GameObject.Find("Shield"));
                    break;

                // Open all colosseum trials
                case SceneNames.Room_Colosseum_01:
                    PlayerData.instance.colosseumBronzeOpened = true;
                    PlayerData.instance.colosseumSilverOpened = true;
                    PlayerData.instance.colosseumGoldOpened = true;
                    GameObject.Find("Silver Trial Board").LocateMyFSM("Conversation Control").GetState("Hero Anim").ClearTransitions();
                    GameObject.Find("Silver Trial Board").LocateMyFSM("Conversation Control").GetState("Hero Anim").AddTransition("FINISHED", "Box Up YN");
                    GameObject.Find("Gold Trial Board").LocateMyFSM("Conversation Control").GetState("Hero Anim").ClearTransitions();
                    GameObject.Find("Gold Trial Board").LocateMyFSM("Conversation Control").GetState("Hero Anim").AddTransition("FINISHED", "Box Up YN");
                    break;

                // Destroy Grey Mourner after the flower has been delivered
                case SceneNames.Room_Mansion:
                    if (PlayerData.instance.xunFlowerGiven)
                    {
                        PlayerData.instance.xunRewardGiven = true;
                    }
                    break;

                // Removes King's Brand cutscene trigger
                case SceneNames.Room_Wyrm:
                    Object.Destroy(GameObject.Find("Avalanche End"));
                    break;

                // Open Colosseum gates after picking up resp. items
                case SceneNames.Room_Colosseum_Bronze:
                    GameObject.Find("Colosseum Manager").LocateMyFSM("Geo Pool").GetState("Open Gates").AddFirstAction(new RandomizerSetBool("colosseumBronzeCompleted", true, true));
                    break;
                case SceneNames.Room_Colosseum_Silver:
                    GameObject.Find("Colosseum Manager").LocateMyFSM("Geo Pool").GetState("Open Gates").AddFirstAction(new RandomizerSetBool("colosseumSilverCompleted", true, true));
                    break;

                // Prevent simple key softlocks
                case SceneNames.Town:
                    FsmState jijiKey = GameObject.Find("Jiji Door").LocateMyFSM("Conversation Control").GetState("Key?");
                    jijiKey.RemoveActionsOfType<GetPlayerDataInt>();
                    jijiKey.RemoveActionsOfType<IntCompare>();
                    jijiKey.AddAction(new RandomizerExecuteLambda(() =>
                    {
                        if (GameManager.instance.GetPlayerDataInt("simpleKeys") > 1 || (PlayerData.instance.openedWaterwaysManhole && GameManager.instance.GetPlayerDataInt("simpleKeys") > 0)) PlayMakerFSM.BroadcastEvent("KEY");
                        else PlayMakerFSM.BroadcastEvent("NOKEY");
                    }));
                    break;

            }
        }

        // Break various dive floors with totems or transitions randomized to prevent soul-based locks
        public static void BreakDiveFloors(Scene newScene)
        {
            if (Ref.PD.quakeLevel < 1) return;

            switch (newScene.name)
            {
                // Break the first two dive floors on the way to the 420 geo rock if the player has dive and rooms are randomized to prevent soul-based locks
                case SceneNames.Deepnest_East_17:
                    if (RandomizerMod.Instance.Settings.RandomizeRooms)
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Deepnest_East_17",
                            id = "Quake Floor",
                            activated = true,
                            semiPersistent = false
                        });
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Deepnest_East_17",
                            id = "Quake Floor (1)",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;

                // Break the second Oro dive floor if the player has dive and both transitions AND soul totems are randomized to prevent soul-based locks
                case SceneNames.Deepnest_East_14:
                    if (RandomizerMod.Instance.Settings.RandomizeSoulTotems && RandomizerMod.Instance.Settings.RandomizeRooms
                        && GameManager.instance.entryGateName == "top2")
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Deepnest_East_14",
                            id = "Quake Floor (1)",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;

                case SceneNames.Fungus2_21:
                    if (RandomizerMod.Instance.Settings.RandomizeTransitions && GameManager.instance.entryGateName == "right1")
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Fungus2_21",
                            id = "Quake Floor",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;

                // Break the Peak entrance dive floor if the player has dive and transitions are randomized to prevent soul-based locks
                case SceneNames.Mines_01:
                    if (RandomizerMod.Instance.Settings.RandomizeTransitions && GameManager.instance.entryGateName == "left1")
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Mines_01",
                            id = "mine_1_quake_floor",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;

                // Break the Crystallized Mound dive floor if the player has dive and transitions or soul totems are randomized to prevent soul-based locks
                case SceneNames.Mines_35:
                    if (RandomizerMod.Instance.Settings.RandomizeTransitions || RandomizerMod.Instance.Settings.RandomizeSoulTotems)
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Mines_35",
                            id = "mine_1_quake_floor",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;

                // Break the Crypts dive floor if the player has dive and soul totems are randomized to prevent soul-based locks
                case SceneNames.RestingGrounds_05:
                    if (RandomizerMod.Instance.Settings.RandomizeSoulTotems)
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "RestingGrounds_05",
                            id = "Quake Floor",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;

                // Break the Goam journal entry dive floor if the player has dive and rooms are randomized to prevent soul-based locks
                case SceneNames.Crossroads_52:
                    if (RandomizerMod.Instance.Settings.RandomizeRooms)
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Crossroads_52",
                            id = "Quake Floor",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;

                // Break the Dung Defender dive floor if the player has dive and transitions are randomized to prevent soul-based locks
                case SceneNames.Waterways_05:
                    if (RandomizerMod.Instance.Settings.RandomizeRooms)
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Waterways_05",
                            id = "Quake Floor",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
            }
        }

        public static void EditStagStations(PlayMakerFSM fsm)
        {
            if (!RandomizerMod.Instance.Settings.RandomizeStags) return;

            switch (fsm.gameObject.scene.name)
            {
                case SceneNames.Crossroads_47:
                case SceneNames.Fungus1_16_alt:
                case SceneNames.Fungus2_02:
                case SceneNames.Fungus3_40:
                case SceneNames.Ruins1_29:
                case SceneNames.Ruins2_08:
                case SceneNames.Deepnest_09:
                case SceneNames.Abyss_22:
                case SceneNames.RestingGrounds_09:
                    if (fsm.gameObject.name.Contains("Station Bell") && fsm.FsmName == "Stag Bell")
                    {
                        fsm.GetState("Init").RemoveActionsOfType<PlayerDataBoolTest>();
                        fsm.GetState("Init").AddTransition("FINISHED", "Opened");
                    }
                    else if (fsm.gameObject.name.Contains("Stag") && fsm.FsmName == "Stag Control")
                    {
                        // For Hidden Station, we're messing with the Hidden Station check to decide where to show the Travel Prompt,
                        // so we need to fix it
                        if (fsm.gameObject.scene.name == SceneNames.Abyss_22)
                        {
                            fsm.GetState("Hidden Station?").RemoveActionsOfType<IntCompare>();
                        }

                        fsm.GetState("Open Grate").RemoveActionsOfType<SetPlayerDataBool>();
                        fsm.GetState("Open Grate").RemoveActionsOfType<SetBoolValue>();

                        // We'll use a bool set by the UI List fsm to cancel travel if the player decides not to travel
                        FsmBool cancelTravel = new FsmBool(name: "Cancel Travel");
                        cancelTravel.Value = false;

                        // Add our cancelTravel bool to the bool variables list
                        // We'll add this even if the player has the stag so there's no issue with the UI List FSM changing 
                        // its value (even though I'm pretty sure it doesn't matter)
                        FsmBool[] boolVariables = new FsmBool[fsm.FsmVariables.BoolVariables.Length + 1];
                        Array.Copy(fsm.FsmVariables.BoolVariables, boolVariables, fsm.FsmVariables.BoolVariables.Length);
                        boolVariables[fsm.FsmVariables.BoolVariables.Length] = cancelTravel;
                        fsm.FsmVariables.BoolVariables = boolVariables;

                        if (!PlayerData.instance.GetBool(fsm.FsmVariables.StringVariables.First(v => v.Name == "Station Opened Bool").Value))
                        {
                            fsm.FsmVariables.IntVariables.First(v => v.Name == "Station Position Number").Value = 0;
                            fsm.GetState("Current Location Check").RemoveActionsOfType<IntCompare>();

                            fsm.GetState("Check Result").AddFirstAction(new RandomizerExecuteLambda(() =>
                            {
                                if (fsm.FsmVariables.BoolVariables.First(v => v.Name == "Cancel Travel").Value == true)
                                {
                                    fsm.SendEvent("CANCEL");
                                }
                            }));
                            fsm.GetState("Check Result").AddTransition("CANCEL", "HUD Return");
                        }

                        fsm.GetState("HUD Return").AddFirstAction(new SetBoolValue
                        {
                            boolVariable = cancelTravel,
                            boolValue = false
                        });
                    }
                    else if (fsm.gameObject.name == "UI List Stag" && fsm.FsmName == "ui_list")
                    {
                        fsm.GetState("Selection Made Cancel").AddFirstAction(new RandomizerExecuteLambda(() => 
                        {
                            GameObject.Find("Stag").LocateMyFSM("Stag Control").FsmVariables
                                .BoolVariables.First(v => v.Name == "Cancel Travel").Value = true;
                        }));
                    }
                    break;

                default:
                    return;
            }
        }

        // Using the "ReplaceObjectWithShiny" method doesn't work because the layers are messed up.
        public static void DestroyLoreTablets(Scene newScene)
        {
            if (RandomizerMod.Instance.Settings.RandomizeLoreTablets)
            {
                switch (newScene.name)
                {
                    default:
                        break;

                    case SceneNames.Ruins1_02:
                        Object.Destroy(GameObject.Find("Lore_0017_city_01"));
                        break;
                    case SceneNames.Ruins_Elevator:
                        Object.Destroy(GameObject.Find("rec_elev_0017_poster"));
                        break;
                    case SceneNames.Ruins1_23:
                        Object.Destroy(GameObject.Find("Glow Response Mage Computer"));
                        break;
                    case SceneNames.Ruins1_32:
                        Object.Destroy(GameObject.Find("Glow Response Mage Computer"));
                        break;
                    case SceneNames.Ruins2_Watcher_Room:
                        Object.Destroy(GameObject.Find("ruins_watcher_desk_0001_3"));
                        break;
                    case SceneNames.Fungus3_archive_02:
                        Object.Destroy(GameObject.Find("fung_temple_extra_0001_lore_machine1 (2)"));        // Upper tablet
                        Object.Destroy(GameObject.Find("fung_temple_extra_0001_lore_machine1"));            // Left tablet
                        Object.Destroy(GameObject.Find("fung_temple_extra_0001_lore_machine1 (1)"));        // Right tablet
                        break;
                    case SceneNames.Crossroads_11_alt:
                        Object.Destroy(GameObject.Find("pilgrim_path_sign"));
                        break;
                    case SceneNames.Fungus2_21:
                        Object.Destroy(GameObject.Find("pilgrim_path_sign"));
                        break;
                    case SceneNames.Fungus2_12:
                        Object.Destroy(GameObject.Find("mantis_sign_01"));
                        break;
                    case SceneNames.Fungus2_14:
                        Object.Destroy(GameObject.Find("mantis_sign_01"));
                        break;
                    case SceneNames.Fungus1_17:
                        Object.Destroy(GameObject.Find("green_path_lore_tabs_0000_4"));
                        break;
                    case SceneNames.Fungus1_30:
                        Object.Destroy(GameObject.Find("green_path_lore_tabs_0002_2"));
                        break;
                    case SceneNames.Fungus1_32:
                        Object.Destroy(GameObject.Find("green_path_lore_tabs_0001_3"));
                        break;
                    case SceneNames.Fungus1_21:
                        Object.Destroy(GameObject.Find("green_path_lore_tabs_0000_4"));
                        break;
                    case SceneNames.Fungus1_13:
                        Object.Destroy(GameObject.Find("green_path_lore_tabs_0001_3"));
                        break;
                    case SceneNames.Fungus1_19:
                        Object.Destroy(GameObject.Find("green_path_lore_tabs_0000_4"));
                        break;
                    case SceneNames.Waterways_07:
                        Object.Destroy(GameObject.Find("Dung_Def_Sign"));
                        break;
                    case SceneNames.Fungus2_20:
                        Object.Destroy(GameObject.Find("fung_well_tab_03"));
                        break;
                    case SceneNames.Fungus2_07:
                        Object.Destroy(GameObject.Find("fung_well_tab_01"));
                        break;
                    case SceneNames.Fungus2_04:
                        Object.Destroy(GameObject.Find("fung_well_tab_02"));
                        break;
                    case SceneNames.Fungus2_30:
                        Object.Destroy(GameObject.Find("fung_well_tab_03"));
                        break;
                    case SceneNames.Abyss_06_Core:
                        Object.Destroy(GameObject.Find("Tut_tablet_Abyss"));
                        break;
                    case SceneNames.Tutorial_01:
                        Object.Destroy(GameObject.Find("Tut_tablet_top"));          // Focus tablet
                        Object.Destroy(GameObject.Find("Tut_tablet_top (2)"));      // Fury tablet
                        Object.Destroy(GameObject.Find("Tut_tablet_top (1)"));      // Exit tablet
                        break;
                    case SceneNames.Room_Final_Boss_Atrium:
                        Object.Destroy(GameObject.Find("Tut_tablet_top"));
                        break;
                    case SceneNames.Cliffs_01:
                        Object.Destroy(GameObject.Find("Tut_tablet_top"));
                        break;
                    // Both objects refer to the same lore tablet
                    case SceneNames.Deepnest_East_17:
                        Object.Destroy(GameObject.Find("tut_tab_lit"));
                        Object.Destroy(GameObject.Find("unlit_tablet (1)"));
                        break;
                }
            }

            if (RandomizerMod.Instance.Settings.RandomizePalaceTablets)
            {
                switch (newScene.name)
                {
                    default:
                        break;

                    // Both objects refer to the same lore tablet
                    case SceneNames.White_Palace_08:
                        Object.Destroy(GameObject.Find("wp_workshop_book"));
                        Object.Destroy(GameObject.Find("Glow Response Object"));
                        break;
                    // Both objects refer to the same lore tablet
                    case SceneNames.White_Palace_09:
                        Object.Destroy(GameObject.Find("White_Palace_throne_room_top_0000_2"));
                        Object.Destroy(GameObject.Find("Glow Response floor_ring large2 (1)"));
                        break;
                    case SceneNames.White_Palace_18:
                        Object.Destroy(GameObject.Find("wp_plaque_new"));
                        break;
                }
            }
        }

        private static void DestroyAllObjectsNamed(string name)
        {
            foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
            {
                if (go.name.StartsWith(name))
                {
                    Object.Destroy(go);
                }
            }
        }

        public static void EditCorniferAndIselda(Scene newScene)
        {
            if (!RandomizerMod.Instance.Settings.RandomizeMaps) return;

            switch (newScene.name)
            {
                case SceneNames.Crossroads_06:
                    foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
                    {
                        if (go.name.StartsWith("Set NPC Leave"))
                        {
                            Object.Destroy(go);
                        }
                    }
                    break;
                case SceneNames.Crossroads_33:
                case SceneNames.Fungus1_06:
                case SceneNames.Fungus3_25:
                case SceneNames.Fungus2_18:
                case SceneNames.Deepnest_01b:
                case SceneNames.Fungus2_25:
                case SceneNames.Abyss_04:
                case SceneNames.Deepnest_East_03:
                case SceneNames.Ruins1_31:
                case SceneNames.Waterways_09:
                case SceneNames.Cliffs_01:
                case SceneNames.Mines_30:
                case SceneNames.Fungus1_24:
                    DestroyAllObjectsNamed(RandomizerMod.Instance.Settings.NPCItemDialogue ? "Cornifer Card" : "Cornifer");
                    break;
                case SceneNames.RestingGrounds_09:
                    DestroyAllObjectsNamed("Cornifer");
                    break;
            }
        }

        public static void DeleteCollectorGrubs(Scene newScene)
        {
            if (!RandomizerMod.Instance.Settings.RandomizeGrubs && !RandomizerMod.Instance.Settings.RandomizeMimics) return;

            switch (newScene.name)
            {
                case SceneNames.Ruins2_11:
                    Object.Destroy(GameObject.Find("Grubs Folder"));
                    foreach (GameObject g in Object.FindObjectsOfType<GameObject>())
                    {
                        if (g.name.Contains("Grub Bottle")) Object.Destroy(g);
                    }
                    break;
            }
        }
        public static void DestroyMimicObjects(Scene newScene)
        {
            if (!RandomizerMod.Instance.Settings.RandomizeMimics) return;

            switch (newScene.name)
            {
                case SceneNames.Deepnest_36:
                    Object.Destroy(newScene.FindGameObject("Grub Mimic Top"));
                    Object.Destroy(newScene.FindGameObject("Grub Mimic Top (1)"));
                    Object.Destroy(newScene.FindGameObject("Grub Mimic Top (2)"));
                    Object.Destroy(newScene.FindGameObject("Grub Mimic Bottle"));
                    Object.Destroy(newScene.FindGameObject("Grub Mimic Bottle (1)"));
                    Object.Destroy(newScene.FindGameObject("Grub Mimic Bottle (2)"));
                    break;

                case SceneNames.Mines_16:
                    Object.Destroy(newScene.FindGameObject("Grub Mimic Top"));
                    Object.Destroy(newScene.FindGameObject("Grub Mimic Bottle"));
                    break;
            }
        }

        public static void DisableInfectedCrossroads(PlayMakerFSM fsm)
        {
            if (!RandomizerMod.Instance.Settings.RandomizeRooms) return;
            if (fsm.FsmName != "Activate Infected") return;
            if (!new List<string> { SceneNames.Crossroads_03, SceneNames.Crossroads_06, SceneNames.Crossroads_10, SceneNames.Crossroads_19 }
                .Contains(fsm.gameObject.scene.name)) return;
            
            FsmState checkState = fsm.GetState("Check");
            checkState.RemoveActionsOfType<PlayerDataBoolTest>();
            checkState.ClearTransitions();
            checkState.AddTransition("FINISHED", "False");
        }
    }
}
