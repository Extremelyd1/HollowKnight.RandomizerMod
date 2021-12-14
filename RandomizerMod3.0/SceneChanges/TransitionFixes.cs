using HutongGames.PlayMaker.Actions;
using RandomizerMod.Extensions;
using RandomizerMod.FsmStateActions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RandomizerMod.SceneChanges
{
    internal static partial class SceneEditor
    {
        /*
         * Method for enforcing the proper sequence on certain npc quests, etc.
         * This method is currently only used for transition randomizer
         */

        public static void ApplyControlFixes(Scene newScene)
        {
            if (!RandomizerMod.Instance.Settings.RandomizeTransitions) return;

            switch (newScene.name)
            {
                case SceneNames.Grimm_Main_Tent:
                    if (!PlayerData.instance.nightmareLanternLit)
                    {
                        PlayMakerFSM.BroadcastEvent("BG CLOSE");
                    }
                    break;
                case SceneNames.Room_ruinhouse:
                    PlayMakerFSM dazedSly = newScene.FindGameObject("Sly Dazed").LocateMyFSM("Conversation Control");
                    dazedSly.GetState("Active?").RemoveActionsOfType<BoolTest>();
                    dazedSly.GetState("Active?").AddAction(new RandomizerBoolTest("slyRescued", "FINISHED", "DESTROY"));
                    dazedSly.GetState("Convo Choice").RemoveActionsOfType<BoolTest>();
                    dazedSly.GetState("Convo Choice").AddAction(new RandomizerBoolTest("slyRescued", "MEET", "MEET 2"));
                    dazedSly.GetState("Meet").AddAction(new RandomizerSetBool("slyRescued", true));
                    break;
                case SceneNames.Room_shop:
                    if (!RandomizerMod.Instance.Settings.GetBool(false, "slyRescued"))
                    {
                        Object.Destroy(newScene.FindGameObject("Sly Shop"));
                        Object.Destroy(newScene.FindGameObject("Shop Region"));
                        Object.Destroy(newScene.FindGameObject("Basement Open"));
                        Object.Destroy(newScene.FindGameObject("door1"));
                    }
                    break;
            }
        }

        /*
         * Use this method for fixing bugs related to passing through transitions from an unintended direction.
         * Fixes should be appropriate in any randomizer mode.
         * Fixes specific to transition randomizer need bool checks
         * This method is called after the scene is loaded, but before the player enters
         */

        public static void ApplyTransitionFixes(Scene newScene)
        {
            switch (newScene.name)
            {
                case SceneNames.Abyss_06_Core:
                    // Opens door to LBC
                    if (RandomizerMod.Instance.Settings.RandomizeTransitions)
                    {
                        PlayerData.instance.SetBool("blueVineDoor", true);
                        PlayMakerFSM BlueDoorFSM = newScene.FindGameObject("Blue Door").LocateMyFSM("Control");
                        BlueDoorFSM.GetState("Init").RemoveTransitionsTo("Got Charm");
                    }

                    // Opens floor to void heart (bypasses subsequent checks for kingsoul to be equipped)
                    if (GameManager.instance.entryGateName.StartsWith("b"))
                    {
                        PlayerData.instance.SetBool("openedBlackEggPath", true);
                    }
                    if (PlayerData.instance.openedBlackEggPath)
                    {
                        Object.Destroy(newScene.FindGameObject("floor_closed"));
                    }
                    break;
                case SceneNames.Deepnest_41:
                    if (GameManager.instance.entryGateName.StartsWith("left1"))
                    {
                        foreach (Transform t in newScene.FindGameObject("Collapser Small (2)").FindGameObjectInChildren("floor1").transform)
                        {
                            if (t.gameObject.name.StartsWith("msk")) Object.Destroy(t.gameObject);
                        }
                    }
                    break;
                case SceneNames.Deepnest_East_02:
                    if (GameManager.instance.entryGateName.StartsWith("bot2"))
                    {
                        GameObject active = newScene.FindGameObject("Quake Floor").FindGameObjectInChildren("Active");
                        Object.Destroy(active.FindGameObjectInChildren("msk_generic"));
                        Object.Destroy(active.FindGameObjectInChildren("msk_generic (1)"));
                        Object.Destroy(active.FindGameObjectInChildren("msk_generic (2)"));
                        Object.Destroy(active.FindGameObjectInChildren("msk_generic (3)"));
                    }
                    break;
                case SceneNames.Deepnest_East_03:
                    // When entering from one of the other entrances, it's possible that the player will reach Cornifer before the big title popup
                    // appears; this can lead to a hard lock if the player interacts with Cornifer and then the popup appears during the interaction.
                    if (!GameManager.instance.entryGateName.StartsWith("left"))
                    {
                        Ref.PD.SetBool(nameof(PlayerData.visitedOutskirts), true);
                    }
                    break;
                case SceneNames.Fungus2_15:
                    if (GameManager.instance.entryGateName.StartsWith("left"))
                    {
                        Object.Destroy(newScene.FindGameObject("deepnest_mantis_gate").FindGameObjectInChildren("Collider"));
                        Object.Destroy(newScene.FindGameObject("deepnest_mantis_gate"));
                    }
                    break;
                case SceneNames.Fungus2_25:
                    if (GameManager.instance.entryGateName.StartsWith("right"))
                    {
                        Object.Destroy(newScene.FindGameObject("mantis_big_door"));
                    }
                    break;
                case SceneNames.Ruins1_09:
                    if (GameManager.instance.entryGateName.StartsWith("t"))
                    {
                        Object.Destroy(newScene.FindGameObject("Battle Gate"));
                        Object.Destroy(newScene.FindGameObject("Battle Scene"));
                    }
                    break;
                case SceneNames.Waterways_04:
                    if (GameManager.instance.entryGateName.StartsWith("b"))
                    {
                        GameObject[] gs = Object.FindObjectsOfType<GameObject>();
                        foreach (GameObject g in gs)
                        {
                            if (g.name.StartsWith("Mask"))
                            {
                                g.SetActive(false);
                            }
                        }
                    }
                    break;
                case SceneNames.White_Palace_03_hub:
                    {
                        GameObject[] gs = Object.FindObjectsOfType<GameObject>();
                        foreach (GameObject g in gs)
                        {
                            if (g.name.StartsWith("Progress"))
                            {
                                Object.Destroy(g);
                            }
                        }
                    }
                    break;
                case SceneNames.White_Palace_06:
                    if (newScene.FindGameObject("Path of Pain Blocker") != null)
                    {
                        Object.Destroy(newScene.FindGameObject("Path of Pain Blocker"));
                    }
                    break;

                // traverse first room of PoP backwards. Doesn't really belong here, but w/e
                case SceneNames.White_Palace_18:
                    const float SAW = 1.362954f;
                    GameObject saw = newScene.FindGameObject("wp_saw (4)");

                    GameObject topSaw = Object.Instantiate(saw);
                    topSaw.transform.SetPositionX(165f);
                    topSaw.transform.SetPositionY(30.5f);
                    topSaw.transform.localScale = new Vector3(SAW / 1.5f, SAW / 2, SAW);

                    GameObject botSaw = Object.Instantiate(saw);
                    botSaw.transform.SetPositionX(161.4f);
                    botSaw.transform.SetPositionY(21.4f);
                    botSaw.transform.localScale = new Vector3(SAW / 1.5f, SAW / 2, SAW);

                    break;
            }
        }

        // WARNING - the next method is called before the next scene begins to load.

        public static void ApplySaveDataChanges(string sceneName, string entryGateName)
        {
            if (string.IsNullOrEmpty(sceneName)) return;
            entryGateName = entryGateName ?? string.Empty;

            switch (sceneName)
            {
                case SceneNames.Tutorial_01:
                    SavePersistentBoolItemState(new PersistentBoolData
                    {
                        sceneName = "Tutorial_01",
                        id = "Initial Fall Impact",
                        activated = true,
                        semiPersistent = false
                    });
                    if (entryGateName.StartsWith("right"))
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Tutorial_01",
                            id = "Door",
                            activated = true,
                            semiPersistent = false
                        });
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Tutorial_01",
                            id = "Collapser Tute 01",
                            activated = true,
                            semiPersistent = false
                        });
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Tutorial_01",
                            id = "Tute Door 1",
                            activated = true,
                            semiPersistent = false
                        });
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Tutorial_01",
                            id = "Tute Door 2",
                            activated = true,
                            semiPersistent = false
                        });
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Tutorial_01",
                            id = "Tute Door 3",
                            activated = true,
                            semiPersistent = false
                        });
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Tutorial_01",
                            id = "Tute Door 4",
                            activated = true,
                            semiPersistent = false
                        });
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Tutorial_01",
                            id = "Tute Door 5",
                            activated = true,
                            semiPersistent = false
                        });
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Tutorial_01",
                            id = "Tute Door 7",
                            activated = true,
                            semiPersistent = false
                        });
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Tutorial_01",
                            id = "Break Floor 1",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;

                case SceneNames.Abyss_01:
                    if (entryGateName.StartsWith("left1"))
                    {
                        PlayerData.instance.SetBool("dungDefenderWallBroken", true);
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Waterways_05",
                            id = "One Way Wall",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Abyss_03_c:
                    if (entryGateName.StartsWith("r"))
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Abyss_03_c",
                            id = "Breakable Wall",
                            activated = true,
                            semiPersistent = false
                        });
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Abyss_03_c",
                            id = "Mask 1",
                            activated = true,
                            semiPersistent = false
                        });
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Abyss_03_c",
                            id = "Mask 1 (1)",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Abyss_05:
                    if (entryGateName == "right1")
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Abyss_05",
                            id = "Breakable Wall",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Cliffs_01:
                    if (entryGateName.StartsWith("right4"))
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Cliffs_01",
                            id = "Breakable Wall",
                            activated = true,
                            semiPersistent = false
                        });
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Cliffs_01",
                            id = "Breakable Wall grimm",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Crossroads_04:
                    Ref.PD.menderState = 2;
                    Ref.PD.menderDoorOpened = true;
                    Ref.PD.hasMenderKey = true;
                    Ref.PD.menderSignBroken = true;
                    if (entryGateName.StartsWith("d"))
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Crossroads_04",
                            id = "Secret Mask",
                            activated = true,
                            semiPersistent = false
                        });
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Crossroads_04",
                            id = "Secret Mask (1)",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Crossroads_06:
                    // Opens gate in room after False Knight
                    if (entryGateName.StartsWith("l"))
                    {
                        PlayerData.instance.SetBool("shamanPillar", true);
                    }
                    break;
                case SceneNames.Crossroads_07:
                    if (entryGateName.StartsWith("left3"))
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Crossroads_07",
                            id = "Tute Door 1",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Crossroads_08:
                    if (entryGateName == "left2")
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Crossroads_08",
                            id = "Battle Scene",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Crossroads_09:
                    if (entryGateName.StartsWith("r"))
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Crossroads_09",
                            id = "Break Floor 1",
                            activated = true,
                            semiPersistent = false
                        });
                        PlayerData.instance.SetBool("crossroadsMawlekWall", true);
                    }
                    break;
                case SceneNames.Crossroads_21:
                    // Makes room visible entering from gwomb entrance
                    if (entryGateName.StartsWith("t"))
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Crossroads_21",
                            id = "Breakable Wall",
                            activated = true,
                            semiPersistent = false
                        });
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Crossroads_21",
                            id = "Collapser Small",
                            activated = true,
                            semiPersistent = false
                        });
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Crossroads_21",
                            id = "Secret Mask (1)",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Crossroads_33:
                    if (entryGateName.StartsWith("left1"))
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Crossroads_09",
                            id = "Break Floor 1",
                            activated = true,
                            semiPersistent = false
                        });
                        PlayerData.instance.SetBool("crossroadsMawlekWall", true);
                    }
                    // Opens gate in room after False Knight
                    if (entryGateName.StartsWith("right1"))
                    {
                        PlayerData.instance.SetBool("shamanPillar", true);
                    }
                    break;
                case SceneNames.Deepnest_01:
                    if (entryGateName.StartsWith("r"))
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Deepnest_01",
                            id = "Breakable Wall",
                            activated = true,
                            semiPersistent = false
                        });
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Fungus2_20",
                            id = "Breakable Wall Waterways",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Deepnest_02:
                    if (entryGateName.StartsWith("r"))
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Deepnest_02",
                            id = "Breakable Wall",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Deepnest_03:
                    if (entryGateName.StartsWith("left2"))
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Deepnest_03",
                            id = "Breakable Wall",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Deepnest_26:
                    if (entryGateName.StartsWith("left2"))
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Deepnest_26",
                            id = "Inverse Remasker",
                            activated = true,
                            semiPersistent = false
                        });
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Deepnest_26",
                            id = "Secret Mask (1)",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Deepnest_31:
                    if (entryGateName.StartsWith("right2"))
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Deepnest_31",
                            id = "Secret Mask",
                            activated = true,
                            semiPersistent = false
                        });
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Deepnest_31",
                            id = "Secret Mask (1)",
                            activated = true,
                            semiPersistent = false
                        });
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Deepnest_31",
                            id = "Secret Mask (2)",
                            activated = true,
                            semiPersistent = false
                        });
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Deepnest_31",
                            id = "Breakable Wall",
                            activated = true,
                            semiPersistent = false
                        });
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Deepnest_31",
                            id = "Breakable Wall (1)",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Deepnest_East_02:
                    if (entryGateName.StartsWith("r"))
                    {
                        PlayerData.instance.SetBool("outskirtsWall", true);
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Deepnest_East_02",
                            id = "One Way Wall",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Deepnest_East_03:
                    if (entryGateName.StartsWith("left2"))
                    {
                        PlayerData.instance.SetBool("outskirtsWall", true);
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Deepnest_East_02",
                            id = "One Way Wall",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Deepnest_East_16:
                    if (entryGateName.StartsWith("b"))
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Deepnest_East_16",
                            id = "Quake Floor",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Fungus2_20:
                    if (entryGateName.StartsWith("l"))
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Deepnest_01",
                            id = "Breakable Wall",
                            activated = true,
                            semiPersistent = false
                        });
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Fungus2_20",
                            id = "Breakable Wall Waterways",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Fungus3_02:
                    if (entryGateName.StartsWith("right1"))
                    {
                        PlayerData.instance.SetBool("oneWayArchive", true);
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Fungus3_47",
                            id = "One Way Wall",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Fungus3_13:
                    if (entryGateName.StartsWith("left2"))
                    {
                        PlayerData.instance.SetBool("openedGardensStagStation", true);
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Fungus3_40",
                            id = "Gate Switch",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Fungus3_40:
                    if (entryGateName.StartsWith("r"))
                    {
                        PlayerData.instance.SetBool("openedGardensStagStation", true);
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Fungus3_40",
                            id = "Gate Switch",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Fungus3_44:
                    SavePersistentBoolItemState(new PersistentBoolData
                    {
                        sceneName = "Fungus3_44",
                        id = "Secret Mask",
                        activated = true,
                        semiPersistent = false
                    });
                    break;
                case SceneNames.Fungus3_47:
                    if (entryGateName.StartsWith("l"))
                    {
                        PlayerData.instance.SetBool("oneWayArchive", true);
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Fungus3_47",
                            id = "One Way Wall",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Mines_05:
                    // breakable wall leading to Deep Focus
                    if (entryGateName.StartsWith("left2"))
                    {
                        PlayerData.instance.SetBool("brokeMinersWall", true);
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Mines_05",
                            id = "Breakable Wall",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.RestingGrounds_02:
                    if (entryGateName.StartsWith("b"))
                    {
                        PlayerData.instance.SetBool("openedRestingGrounds02", true);
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "RestingGrounds_06",
                            id = "Resting Grounds Slide Floor",
                            activated = true,
                            semiPersistent = false
                        });
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "RestingGrounds_06",
                            id = "Gate Switch",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.RestingGrounds_05:
                    if (entryGateName.StartsWith("right1"))
                    {
                        PlayerData.instance.SetBool("gladeDoorOpened", true);
                        PlayerData.instance.SetBool("dreamReward2", true);
                    }
                    if (entryGateName.StartsWith("b"))
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
                case SceneNames.RestingGrounds_06:
                    if (entryGateName.StartsWith("t"))
                    {
                        PlayerData.instance.SetBool("openedRestingGrounds02", true);
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "RestingGrounds_06",
                            id = "Resting Grounds Slide Floor",
                            activated = true,
                            semiPersistent = false
                        });
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "RestingGrounds_06",
                            id = "Gate Switch",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.RestingGrounds_10:
                    if (entryGateName.StartsWith("l"))
                    {
                        PlayerData.instance.SetBool("restingGroundsCryptWall", true);
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "RestingGrounds_10",
                            id = "One Way Wall",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    if (entryGateName.StartsWith("top2"))
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "RestingGrounds_10",
                            id = "Breakable Wall (5)",
                            activated = true,
                            semiPersistent = false
                        });
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "RestingGrounds_10",
                            id = "Breakable Wall (7)",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Room_Town_Stag_Station:
                    if (entryGateName.StartsWith("left1"))
                    {
                        PlayerData.instance.SetBool("openedTownBuilding", true);
                        PlayerData.instance.SetBool("openedTown", true);
                    }
                    break;
                case SceneNames.Ruins1_05b:
                    if (entryGateName.StartsWith("b"))
                    {
                        PlayerData.instance.SetBool("openedWaterwaysManhole", true);
                    }
                    break;
                case SceneNames.Ruins1_23:
                    if (entryGateName.StartsWith("t"))
                    {
                        PlayerData.instance.SetBool("brokenMageWindow", true);
                        PlayerData.instance.SetBool("brokenMageWindowGlass", true);
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Ruins1_30",
                            id = "Quake Floor Glass (2)",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Ruins1_24:
                    if (entryGateName.StartsWith("right2"))
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Ruins1_24",
                            id = "Secret Mask (1)",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Ruins1_30:
                    if (entryGateName.StartsWith("b"))
                    {
                        PlayerData.instance.SetBool("brokenMageWindow", true);
                        PlayerData.instance.SetBool("brokenMageWindowGlass", true);
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Ruins1_30",
                            id = "Quake Floor Glass (2)",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Ruins1_31:
                    if (entryGateName.StartsWith("left3"))
                    {
                        PlayerData.instance.SetBool("openedMageDoor_v2", true);
                    }
                    if (entryGateName.StartsWith("left2"))
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Ruins1_31",
                            id = "Ruins Lever",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case "Ruins1_31b":
                    if (entryGateName.StartsWith("right1"))
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Ruins1_31",
                            id = "Ruins Lever",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Ruins2_01:
                    if (entryGateName.StartsWith("t"))
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Ruins2_01",
                            id = "Secret Mask",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Ruins2_04:
                    if (entryGateName.StartsWith("door_Ruin_House_03"))
                    {
                        PlayerData.instance.SetBool("city2_sewerDoor", true);
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Ruins_House_03",
                            id = "Ruins Lever",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    else if (entryGateName.StartsWith("door_Ruin_Elevator"))
                    {
                        PlayerData.instance.SetBool("bathHouseOpened", true);
                    }
                    break;
                case SceneNames.Ruins2_10:
                    if (entryGateName.StartsWith("r"))
                    {
                        PlayerData.instance.SetBool("restingGroundsCryptWall", true);
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "RestingGrounds_10",
                            id = "One Way Wall",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Ruins2_10b:
                    if (entryGateName.StartsWith("l"))
                    {
                        PlayerData.instance.SetBool("bathHouseWall", true);
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Ruins_Bathhouse",
                            id = "Breakable Wall",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case "Ruins2_11_b":
                    if (entryGateName.StartsWith("l"))
                    {
                        PlayerData.instance.SetBool("openedLoveDoor", true);
                    }
                    break;
                case SceneNames.Ruins_House_03:
                    if (entryGateName.StartsWith("left1"))
                    {
                        PlayerData.instance.SetBool("city2_sewerDoor", true);
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Ruins_House_03",
                            id = "Ruins Lever",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Ruins_Bathhouse:
                    if (entryGateName.StartsWith("r"))
                    {
                        PlayerData.instance.SetBool("bathHouseWall", true);
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Ruins_Bathhouse",
                            id = "Breakable Wall",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Town:
                    switch (entryGateName)
                    {
                        case "door_sly":
                            PlayerData.instance.SetBool("slyRescued", true);
                            PlayerData.instance.SetBool("openedSlyShop", true);
                            break;
                        case "door_station":
                            PlayerData.instance.SetBool("openedTownBuilding", true);
                            PlayerData.instance.SetBool("openedTown", true);
                            break;
                        case "door_mapper":
                            PlayerData.instance.SetBool("openedMapperShop", true);
                            break;
                        case "door_bretta":
                            PlayerData.instance.SetBool("brettaRescued", true);
                            break;
                        case "door_jiji":
                            PlayerData.instance.SetBool("jijiDoorUnlocked", true);
                            break;
                    }
                    if (entryGateName != "left1")
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Town",
                            id = "Door Destroyer",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Waterways_01:
                    if (entryGateName.StartsWith("t"))
                    {
                        PlayerData.instance.SetBool("openedWaterwaysManhole", true);
                    }
                    if (entryGateName.StartsWith("r"))
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Waterways_01",
                            id = "Breakable Wall Waterways",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Waterways_02:
                    if (entryGateName.StartsWith("bot1"))
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Waterways_02",
                            id = "Quake Floor",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Waterways_04:
                    if (entryGateName.StartsWith("b"))
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Waterways_04",
                            id = "Quake Floor (1)",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Waterways_05:
                    if (entryGateName.StartsWith("r"))
                    {
                        PlayerData.instance.SetBool("dungDefenderWallBroken", true);
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Waterways_05",
                            id = "One Way Wall",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    if (entryGateName.StartsWith("bot2"))
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
                case SceneNames.Waterways_07:
                    if (entryGateName.StartsWith("right1"))
                    {
                        PlayerData.instance.SetBool("waterwaysAcidDrained", true);
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Waterways_05",
                            id = "Waterways_Crank_Lever",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Waterways_08:
                    if (entryGateName == "left2")
                    {
                        SavePersistentBoolItemState(new PersistentBoolData
                        {
                            sceneName = "Waterways_08",
                            id = "Breakable Wall Waterways",
                            activated = true,
                            semiPersistent = false
                        });
                    }
                    break;
                case SceneNames.Waterways_09:
                    if (entryGateName.StartsWith("left"))
                    {
                        PlayerData.instance.SetBool("waterwaysGate", true);
                    }
                    break;
                case SceneNames.White_Palace_13:
                    PlayerData.instance.SetBool(nameof(PlayerData.whitePalaceSecretRoomVisited), true);
                    break;
            }
        }
    }
}
