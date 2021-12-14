using System.Linq;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using RandomizerMod.Extensions;
using RandomizerMod.FsmStateActions;
using RandomizerMod.Randomization;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RandomizerMod.SceneChanges
{
    internal static partial class SceneEditor
    {
        /*
         * Better organization someday...
         */
        public static void MiscQoLChanges(Scene newScene)
        {
            string sceneName = newScene.name;

            // Make baldurs always able to spit rollers and reduce hp
            if (sceneName == SceneNames.Crossroads_11_alt || sceneName == SceneNames.Crossroads_ShamanTemple ||
                sceneName == SceneNames.Fungus1_28)
            {
                foreach (GameObject obj in Object.FindObjectsOfType<GameObject>())
                {
                    if (obj.name.Contains("Blocker"))
                    {
                        HealthManager hm = obj.GetComponent<HealthManager>();
                        if (hm != null)
                        {
                            hm.hp = 5;
                        }
                        PlayMakerFSM fsm = FSMUtility.LocateFSM(obj, "Blocker Control");
                        if (fsm != null)
                        {
                            fsm.GetState("Can Roller?").RemoveActionsOfType<IntCompare>();
                        }
                    }
                }
            }

            switch (sceneName)
            {
                // Lemm sell all
                /*
                case SceneNames.Ruins1_05b when RandomizerMod.Instance.Settings.Lemm:
                    PlayMakerFSM lemm = FSMUtility.LocateFSM(GameObject.Find("Relic Dealer"), "npc_control");
                    lemm.GetState("Convo End").AddAction(new RandomizerSellRelics());
                    break;
                */

                // Grubfather rewards are given out all at once
                case SceneNames.Crossroads_38 when RandomizerMod.Instance.Settings.Grubfather:
                    PlayMakerFSM grubDaddy = FSMUtility.LocateFSM(GameObject.Find("Grub King"), "King Control");
                    grubDaddy.GetState("Final Reward?").RemoveTransitionsTo("Recover");
                    grubDaddy.GetState("Final Reward?").AddTransition("FINISHED", "Recheck");
                    grubDaddy.GetState("Recheck").RemoveTransitionsTo("Gift Anim");
                    grubDaddy.GetState("Recheck").AddTransition("FINISHED", "Activate Reward");

                    int geoTotal = 0;
                    grubDaddy.GetState("All Given").AddAction(new RandomizerAddGeo(grubDaddy.gameObject, 0, true));
                    grubDaddy.GetState("Recheck").AddFirstAction(new RandomizerExecuteLambda(() =>
                        grubDaddy.GetState("All Given").GetActionsOfType<RandomizerAddGeo>()[0].SetGeo(geoTotal)));

                    foreach (PlayMakerFSM grubFsm in grubDaddy.gameObject.GetComponentsInChildren<PlayMakerFSM>(true))
                    {
                        if (grubFsm.FsmName == "grub_reward_geo")
                        {
                            FsmState grubGeoState = grubFsm.GetState("Remaining?");
                            int geo = grubGeoState.GetActionsOfType<IntCompare>()[0].integer1.Value;

                            grubGeoState.RemoveActionsOfType<FsmStateAction>();
                            grubGeoState.AddAction(new RandomizerExecuteLambda(() => geoTotal += geo));
                            grubGeoState.AddTransition("FINISHED", "End");
                        }
                    }
                    break;

                // Great Hopper Easter Egg, I guess
                case SceneNames.Deepnest_East_16:
                    GameObject hopper1 = newScene.FindGameObject("Giant Hopper");
                    GameObject hopper2 = newScene.FindGameObject("Giant Hopper (1)");

                    for (int i = 0; i < 10; i++)
                    {
                        GameObject newHopper1 = Object.Instantiate(hopper1, hopper1.transform.parent);
                        GameObject newHopper2 = Object.Instantiate(hopper2, hopper2.transform.parent);

                        HealthManager hopper1HM = newHopper1.GetComponent<HealthManager>();
                        hopper1HM.SetGeoSmall(0);
                        hopper1HM.SetGeoMedium(0);
                        hopper1HM.SetGeoLarge(0);

                        HealthManager hopper2HM = newHopper2.GetComponent<HealthManager>();
                        hopper2HM.SetGeoSmall(0);
                        hopper2HM.SetGeoMedium(0);
                        hopper2HM.SetGeoLarge(0);

                        Vector3 hopper1Pos = newHopper1.transform.localPosition;
                        hopper1Pos = new Vector3(
                            hopper1Pos.x + i,
                            hopper1Pos.y,
                            hopper1Pos.z);
                        newHopper1.transform.localPosition = hopper1Pos;

                        Vector3 hopper2Pos = newHopper2.transform.localPosition;
                        hopper2Pos = new Vector3(
                            hopper2Pos.x + i - 4,
                            hopper2Pos.y,
                            hopper2Pos.z);
                        newHopper2.transform.localPosition = hopper2Pos;
                    }
                    break;

                // Skip dreamer text before Dream Nail
                case SceneNames.RestingGrounds_04:
                    FsmState dreamerPlaqueInspect = FSMUtility
                        .LocateFSM(GameObject.Find("Dreamer Plaque Inspect"), "Conversation Control")
                        .GetState("Hero Anim");
                    dreamerPlaqueInspect.RemoveActionsOfType<ActivateGameObject>();
                    dreamerPlaqueInspect.RemoveTransitionsTo("Fade Up");
                    dreamerPlaqueInspect.AddTransition("FINISHED", "Map Msg?");

                    PlayMakerFSM dreamerScene2 = FSMUtility.LocateFSM(GameObject.Find("Dreamer Scene 2"), "Control");
                    dreamerScene2.GetState("Take Control").RemoveTransitionsTo("Blast");
                    dreamerScene2.GetState("Take Control").AddTransition("FINISHED", "Fade Out");
                    dreamerScene2.GetState("Fade Out").RemoveTransitionsTo("Dial Wait");
                    dreamerScene2.GetState("Fade Out").AddTransition("FINISHED", "Set Compass Point");
                    break;
            }
        }

        public static void ApplyHintChanges(Scene newScene)
        {
            switch (newScene.name)
            {
                // King Fragment hint
                case SceneNames.Abyss_05:
                    {
                        string item = RandomizerMod.Instance.Settings.ItemPlacements.FirstOrDefault(pair => pair.Item2 == "King_Fragment").Item1;
                        string itemName = Language.Language.Get(LogicManager.GetItemDef(item).nameKey, "UI");
                        LanguageStringManager.SetString(
                            "Lore Tablets",
                            "DUSK_KNIGHT_CORPSE",
                            "A corpse in white armour. You can clearly see the "
                                + itemName + " it's holding, " +
                                "but for some reason you get the feeling you're going to have to go" +
                                " through an unnecessarily long gauntlet of spikes and sawblades just to pick it up."
                                );
                    }
                    break;

                // Colosseum hints
                case SceneNames.Room_Colosseum_01:
                    {
                        string item = RandomizerMod.Instance.Settings.ItemPlacements.FirstOrDefault(pair => pair.Item2 == "Charm_Notch-Colosseum").Item1;
                        string itemName = Language.Language.Get(LogicManager.GetItemDef(item).nameKey, "UI");
                        LanguageStringManager.SetString("Prompts", "TRIAL_BOARD_BRONZE", "Trial of the Warrior. Fight for " + itemName + ".\n" + "Place a mark and begin the Trial?");
                    }
                    {
                        string item = RandomizerMod.Instance.Settings.ItemPlacements.FirstOrDefault(pair => pair.Item2 == "Pale_Ore-Colosseum").Item1;
                        string itemName = Language.Language.Get(LogicManager.GetItemDef(item).nameKey, "UI");
                        LanguageStringManager.SetString("Prompts", "TRIAL_BOARD_SILVER", "Trial of the Conqueror. Fight for " + itemName + ".\n" + "Place a mark and begin the Trial?");
                    }

                    break;

                // Grey Mourner hint
                case SceneNames.Room_Mansion:
                    {
                        string item = RandomizerMod.Instance.Settings.ItemPlacements.FirstOrDefault(pair => pair.Item2 == "Mask_Shard-Grey_Mourner").Item1;
                        string itemName = Language.Language.Get(LogicManager.GetItemDef(item).nameKey, "UI");
                        LanguageStringManager.SetString(
                            "Prompts",
                            "XUN_OFFER",
                            "Accept the Gift, even knowing you'll only get a lousy " + itemName + "?"
                            );
                    }

                    break;

                // Tuk only sells eggs when you have no eggs in your inventory, to balance around hints and/or eggs
                case SceneNames.Waterways_03:
                    GameObject.Find("Tuk NPC").LocateMyFSM("Conversation Control").GetState("Convo Choice").GetActionOfType<IntCompare>().integer2 = 1;
                    break;
            }
        }

        public static void AddWaterSpawns(Scene newScene)
        {
            if (!RandomizerMod.Instance.Settings.RandomizeSwim) return;

            switch (newScene.name)
            {
                case SceneNames.Crossroads_50:
                    CreateWaterSpawn(19f, 45.5f, 1f, 5f, true);
                    CreateWaterSpawn(21f, 26.5f, 6f, 8f, true);
                    CreateWaterSpawn(231f, 26.5f, 6f, 8f, false);
                    break;

                case "GG_Atrium":
                    CreateWaterSpawn(148.5f, 63f, 10f, 5f, false);
                    CreateWaterSpawn(140f, 18f, 1f, 13f, true);
                    break;

                case "GG_Lurker":
                    CreateWaterSpawn(84f, 112.5f, 1f, 15f, true);
                    CreateWaterSpawn(102f, 110f, 1f, 10f, false);
                    CreateWaterSpawn(79f, 80f, 1f, 10f, true);
                    CreateWaterSpawn(109f, 59.5f, 1f, 17f, true);
                    CreateWaterSpawn(177.5f, 57.5f, 1f, 15f, true);
                    CreateWaterSpawn(181f, 82.5f, 1f, 10f, true);
                    CreateWaterSpawn(189f, 82.5f, 1f, 10f, false);
                    break;

                case SceneNames.GG_Waterways:
                    CreateWaterSpawn(72.5f, 16f, 2f, 9f, true);
                    CreateWaterSpawn(61.5f, 63.5f, 4f, 7f, true);
                    CreateWaterSpawn(81f, 63f, 1f, 6f, false);
                    CreateWaterSpawn(91f, 63f, 1f, 6f, true);
                    CreateWaterSpawn(95f, 49.75f, 5.5f, 6f, false);
                    CreateWaterSpawn(128f, 62f, 1f, 22f, false);
                    break;

                case SceneNames.RestingGrounds_08:
                    CreateWaterSpawn(37.5f, 11f, 1f, 15f, true);
                    CreateWaterSpawn(62f, 11f, 1f, 15f, false);
                    CreateWaterSpawn(44f, 38f, 1f, 14f, true);
                    CreateWaterSpawn(56f, 38f, 1f, 14f, false);
                    CreateWaterSpawn(44f, 63f, 1f, 14f, true);
                    CreateWaterSpawn(56f, 62.5f, 1f, 17f, false);
                    CreateWaterSpawn(107.7f, 9.5f, 1f, 11f, true);
                    CreateWaterSpawn(107.7f, 20.95f, 1f, 10.9f, true);
                    CreateWaterSpawn(107.7f, 33.45f, 1f, 14.6f, true);
                    CreateWaterSpawn(116.5f, 13f, 2f, 6f, true);
                    CreateWaterSpawn(127.5f, 27f, 1f, 5f, false);
                    CreateWaterSpawn(116.5f, 61f, 1f, 10f, true);
                    CreateWaterSpawn(124f, 61f, 1f, 10f, false);
                    break;

                case "Room_GG_Shortcut":
                    CreateWaterSpawn(36.5f, 43f, 4f, 7f, false);
                    CreateWaterSpawn(32f, 74f, 4f, 9f, true);
                    CreateWaterSpawn(9.5f, 89f, 9f, 5f, true);
                    break;

                case "GG_Pipeway":
                    CreateWaterSpawn(8f, 15f, 1f, 7f, false);
                    CreateWaterSpawn(6f, 24f, 1f, 7f, false);
                    break;

                case SceneNames.Ruins1_03:
                    CreateWaterSpawn(125f, 48f, 1f, 16f, false);
                    CreateWaterSpawn(127f, 14.5f, 1f, 15f, false);
                    CreateWaterSpawn(81f, 29.75f, 1f, 8.5f, true);
                    CreateWaterSpawn(54.5f, 29.75f, 1f, 8.5f, false);
                    CreateWaterSpawn(40f, 29.25f, 1f, 9.5f, true);
                    CreateWaterSpawn(38.45f, 15.9f, 3.9f, 12.2f, true);
                    CreateWaterSpawn(48.6f, 13.9f, 4f, 8.2f, true);
                    CreateWaterSpawn(57.1f, 11.75f, 1.2f, 12.5f, false);
                    CreateWaterSpawn(80f, 11.75f, 1.2f, 12.5f, true);
                    CreateWaterSpawn(96.05f, 14.25f, 10.7f, 15.5f, false);
                    CreateWaterSpawn(110.95f, 13.75f, 2.9f, 12.5f, false);
                    CreateWaterSpawn(24.5f, 11.75f, 1f, 9.5f, true);
                    break;

                case SceneNames.Ruins1_04:
                    CreateWaterSpawn(93f, 14.5f, 1f, 8f, false);
                    CreateWaterSpawn(59.52f, 14.5f, 1f, 8f, false);
                    CreateWaterSpawn(49f, 15.5f, 1f, 6f, false);
                    CreateWaterSpawn(39.5f, 13.75f, 2f, 9.5f, false);
                    CreateWaterSpawn(32f, 7f, 1f, 5f, true);
                    CreateWaterSpawn(32f, 14.05f, 1f, 8.9f, true);
                    CreateWaterSpawn(49.5f, 39.75f, 1f, 10.5f, true);
                    break;

                case SceneNames.Ruins1_27:
                    CreateWaterSpawn(6f, 9f, 1f, 12f, true);
                    CreateWaterSpawn(51.5f, 8f, 1f, 10f, false);
                    break;

                case SceneNames.Ruins2_04:
                    CreateWaterSpawn(91.5f, 11.9f, 1f, 12.2f, true);
                    CreateWaterSpawn(101.8f, 9.15f, 1f, 10.3f, false);
                    CreateWaterSpawn(96.65f, 10.2f, 9.3f, 1.6f, true);
                    break;

                case SceneNames.Ruins2_06:
                    CreateWaterSpawn(25.75f, 45.25f, 12.5f, 5.5f, true);
                    CreateWaterSpawn(25.75f, 38.5f, 12.5f, 5f, true);
                    CreateWaterSpawn(25.75f, 29.25f, 12.5f, 9.5f, true);
                    CreateWaterSpawn(25.75f, 17.25f, 12.5f, 9.5f, true);
                    break;

                case SceneNames.Ruins2_07:
                    CreateWaterSpawn(30.4f, 8.25f, 1f, 7.5f, true);
                    CreateWaterSpawn(65f, 14f, 1f, 6f, false);
                    CreateWaterSpawn(81.5f, 15f, 1f, 8f, true);
                    break;

                case SceneNames.Waterways_01:
                    CreateWaterSpawn(127f, 32f, 1f, 6f, true);
                    CreateWaterSpawn(127f, 12.5f, 1f, 7f, true);
                    CreateWaterSpawn(65.5f, 12.5f, 1f, 7f, true);
                    CreateWaterSpawn(96f, 12.5f, 1f, 7f, false);
                    break;

                case SceneNames.Waterways_02:
                    CreateWaterSpawn(69f, 12f, 1f, 4f, true);
                    CreateWaterSpawn(90f, 7.5f, 1f, 5f, false);
                    CreateWaterSpawn(85.5f, 28.75f, 1f, 4.5f, true);
                    CreateWaterSpawn(75.5f, 28.75f, 1f, 4.5f, false);
                    break;

                case SceneNames.Waterways_04:
                    CreateWaterSpawn(70.5f, 32.55f, 1f, 6.1f, true);
                    CreateWaterSpawn(127f, 20.55f, 1f, 4.9f, false);
                    break;

                case SceneNames.Waterways_04b:
                    CreateWaterSpawn(108f, 24.95f, 1f, 4.5f, true);
                    CreateWaterSpawn(32.5f, 21.15f, 1f, 5.3f, true);
                    CreateWaterSpawn(96f, 21.05f, 1f, 5.1f, false);
                    CreateWaterSpawn(26.5f, 10.5f, 1f, 6f, true);
                    break;

                case SceneNames.Waterways_12:
                    CreateWaterSpawn(11.4f, 6f, 5.8f, 4f, true);
                    CreateWaterSpawn(27f, 6f, 7f, 4f, false);
                    CreateWaterSpawn(25.45f, 10.65f, 1.9f, 3.3f, false);
                    CreateWaterSpawn(29.575f, 20.35f, 2.55f, 5.3f, false);
                    CreateWaterSpawn(25.36f, 24.325f, 1.68f, 5.35f, false);
                    CreateWaterSpawn(11.95f, 24.325f, 1.7f, 5.35f, true);
                    CreateWaterSpawn(9.9f, 15.25f, 2.8f, 4f, true);
                    CreateWaterSpawn(12.4f, 10.65f, 1.6f, 3.3f, true);
                    break;
            }
        }

        private static void CreateWaterSpawn(float x, float y, float xSize, float ySize, bool respawnFacingRight = true)
        {
            GameObject go = new GameObject("Randomizer Hazard Respawn");
            go.transform.SetPosition2D(new Vector2(x, y));

            BoxCollider2D box = go.AddComponent<BoxCollider2D>();
            box.isTrigger = true;
            box.size = new Vector2(xSize, ySize);

            HazardRespawnMarker hrm = go.AddComponent<HazardRespawnMarker>();
            HazardRespawnTrigger hrt = go.AddComponent<HazardRespawnTrigger>();
            hrt.respawnMarker = hrm;
            hrm.respawnFacingRight = respawnFacingRight;

            go.SetActive(true);
        }
    }
}
