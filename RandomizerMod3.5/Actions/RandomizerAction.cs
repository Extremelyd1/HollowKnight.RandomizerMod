using System;
using System.Collections.Generic;
using System.Linq;
using RandomizerMod.Randomization;
using UnityEngine;
using static RandomizerMod.LogHelper;
using static RandomizerMod.GiveItemActions;
using Object = UnityEngine.Object;

namespace RandomizerMod.Actions
{
    public abstract class RandomizerAction
    {
        public enum ActionType
        {
            GameObject,
            PlayMakerFSM,
            EnemyDeath
        }

        private static readonly List<RandomizerAction> Actions = new List<RandomizerAction>();
        public static Dictionary<string, string> AdditiveBoolNames = new Dictionary<string, string>(); // item name, additive bool name
        public static Dictionary<(string, string), string> ShopItemBoolNames = new Dictionary<(string, string), string>(); // (item name, shop name), shop item bool name

        public abstract ActionType Type { get; }

        public static void ClearActions()
        {
            Actions.Clear();
        }

        public static void CreateActions((string, string)[] items, SaveSettings settings)
        {
            ClearActions();
            
            ShopItemBoolNames = new Dictionary<(string, string), string>();
            AdditiveBoolNames = new Dictionary<string, string>();

            int newShinies = 0;
            int newGrubs = 0;
            int newRocks = 0;
            int newTotems = 0;
            string[] shopNames = LogicManager.ShopNames;

            // Loop non-shop items
            foreach ((string newItemName, string location) in items.Where(item => !shopNames.Contains(item.Item2)))
            {
                ReqDef oldItem = LogicManager.GetItemDef(location);
                ReqDef newItem = LogicManager.GetItemDef(newItemName);

                // Solution done for MW with different settings compatibility
                if (newItemName == location)
                {
                    if (!settings.RandomizeMaps && newItem.pool == "Map")
                        continue;
                
                    if (!settings.RandomizeStags && newItem.pool == "Stag")
                        continue;
                    
                    if (!settings.RandomizeRocks && newItem.pool == "Rock")
                        continue;
                    
                    if (!settings.RandomizeSoulTotems && newItem.pool == "Soul")
                        continue;
                    
                    if (!settings.RandomizePalaceTotems && newItem.pool == "PalaceSoul")
                        continue;
                    
                    if (!settings.RandomizePalaceTablets && newItem.pool == "PalaceLore")
                        continue;
                    
                    if (!settings.RandomizeLoreTablets && newItem.pool == "Lore")
                        continue;
                }

                if (settings.NPCItemDialogue)
                {
                    if (oldItem.objectName == "NM Sheo NPC" || oldItem.objectName == "NM Mato NPC" || oldItem.objectName == "NM Oro NPC")
                    {
                        Actions.Add(new ChangeNailmasterReward(oldItem.sceneName, oldItem.objectName, oldItem.fsmName, newItem.action, newItemName, location));
                        continue;
                    }
                    else if (oldItem.objectName == "Sly Basement NPC")
                    {
                        Actions.Add(new ChangeSlyReward(oldItem.sceneName, oldItem.objectName, oldItem.fsmName, newItem.action, newItemName, location));
                        continue;
                    }
                    else if (oldItem.objectName == "Crystal Shaman")
                    {
                        Actions.Add(new ChangeCrystalShamanReward(oldItem.sceneName, oldItem.objectName, oldItem.fsmName, newItem.action, newItemName, location));
                        continue;
                    }
                    else if (oldItem.objectName == "Ruins Shaman")
                    {
                        Actions.Add(new ChangeSanctumShamanReward(oldItem.sceneName, oldItem.objectName, oldItem.fsmName, newItem.action, newItemName, location));
                        continue;
                    }
                    else if (oldItem.objectName == "Cornifer" || oldItem.objectName == "Cornifer Deepnest")
                    {
                        Actions.Add(new ChangeCorniferReward(oldItem.sceneName, oldItem.objectName, oldItem.fsmName, newItem.action, newItemName, location));
                        continue;
                    }
                }

                if (!string.IsNullOrEmpty(oldItem.inspectName))
                {
                    // For some reason, in most cases the inspect region is a separate object to the lore tablet sprite, so
                    // we have to disable it separately
                    Actions.Add(new DisableLoreTablet(oldItem.sceneName, oldItem.inspectName, oldItem.inspectFsmName));
                }
                else if ((location == "Focus" || location == "World_Sense") && !settings.RandomizeLoreTablets)
                {
                    // Disable the Focus/World Sense tablets here
                    Actions.Add(new DisableLoreTablet(oldItem.sceneName, "Tut_tablet_top", "Inspection"));
                }

                // Some objects destroy themselves based on a pdbool check via the FSM. This executes before we have
                // a chance to replace with a shiny when coming from a boss scene. Disable that behaviour here;
                // we need to do it here to cover the grub, rock cases.
                if (!string.IsNullOrEmpty(oldItem.selfDestructFsmName) && oldItem.replace)
                {
                    // With NPC Item Dialogue we shouldn't do this for the VS pickup
                    if (!(settings.NPCItemDialogue && location == "Vengeful_Spirit"))
                    {
                        Actions.Add(new PreventSelfDestruct(oldItem.sceneName, oldItem.objectName, oldItem.selfDestructFsmName));
                    }
                }

                bool hasCost = (oldItem.cost != 0 || oldItem.costType != AddYNDialogueToShiny.CostType.Geo) 
                    && !(location == "Vessel_Fragment-Basin" && settings.NPCItemDialogue)
                    && oldItem.costType != AddYNDialogueToShiny.CostType.RancidEggs;
                bool canReplaceWithObj = oldItem.elevation != 0 && !(settings.NPCItemDialogue && location == "Vengeful_Spirit") && location != "Hunter's_Journal" && !hasCost;
                bool replacedWithGrub = newItem.pool.StartsWith("Grub") && canReplaceWithObj;
                bool replacedWithGeoRock = newItem.pool == "Rock" && canReplaceWithObj;
                bool replacedWithSoulTotem = newItem.type == ItemType.Soul && canReplaceWithObj;
                bool replacedWithMimic = newItem.pool.StartsWith("Mimic") && canReplaceWithObj;
                bool replaced = replacedWithGrub || replacedWithGeoRock || replacedWithSoulTotem || replacedWithMimic;

                void preventSelfDestruct()
                {
                    // Add a PreventSelfDestruct for shiny items not typically replaced.
                    // Add the action only for items which set a bool, because only those will use a
                    // PlayerData rather than a SceneData check for their original Self Destruction.
                    if (!oldItem.replace && oldItem.fsmName == "Shiny Control" && !string.IsNullOrEmpty(oldItem.boolName))
                    {
                        Actions.Add(new PreventSelfDestruct(oldItem.sceneName, oldItem.objectName, "Shiny Control"));
                    }
                }

                if (replacedWithGrub)
                {
                    string jarName = "Randomizer Grub Jar " + newGrubs++;
                    if (oldItem.newShiny)
                    {
                        Actions.Add(new CreateNewGrubJar(oldItem.sceneName, oldItem.x, oldItem.y + CreateNewGrubJar.GRUB_JAR_ELEVATION - oldItem.elevation, jarName, newItemName, location));
                    }
                    else
                    {
                        Actions.Add(new ReplaceObjectWithGrubJar(oldItem.sceneName, oldItem.objectName, oldItem.elevation, jarName, newItemName, location));
                        preventSelfDestruct();
                    }
                }
                else if (replacedWithMimic)
                {
                    string bottleName = "Randomizer Mimic Bottle " + newGrubs++;
                    if (oldItem.newShiny)
                    {
                        Actions.Add(new CreateNewMimicBottle(oldItem.sceneName, oldItem.x, oldItem.y + CreateNewMimicBottle.MIMIC_BOTTLE_ELEVATION - oldItem.elevation, bottleName, newItemName, location));
                    }
                    else
                    {
                        Actions.Add(new ReplaceObjectWithMimicBottle(oldItem.sceneName, oldItem.objectName, oldItem.elevation, bottleName, newItemName, location));
                        preventSelfDestruct();
                    }
                }
                else if (replacedWithGeoRock)
                {
                    string rockName = "Randomizer Geo Rock " + newRocks++;
                    GeoRockSubtype subtype = GetRockSubtype(newItem.objectName);
                    // The 420 geo rock gives 5-geo pieces, so the amount
                    // spawned must be reduced proportionally.
                    int geo = newItem.geo;
                    if (subtype == GeoRockSubtype.Outskirts420) {
                        geo /= 5;
                    }
                    if (oldItem.newShiny)
                    {
                        Actions.Add(new CreateNewGeoRock(oldItem.sceneName, oldItem.x, oldItem.y + CreateNewGeoRock.Elevation[subtype] - oldItem.elevation, rockName, newItemName, location, geo, subtype));
                    }
                    else
                    {
                        Actions.Add(new ReplaceObjectWithGeoRock(oldItem.sceneName, oldItem.objectName, oldItem.elevation, rockName, newItemName, location, geo, subtype));
                        preventSelfDestruct();
                    }
                }
                else if (replacedWithSoulTotem)
                {
                    bool infinite = newItem.objectName.Contains("nfinte");              // Not a typo
                    string totemName = "Randomizer Soul Totem " + newTotems++;
                    SoulTotemSubtype intendedSubtype = GetTotemSubtype(newItem.objectName);
                    SoulTotemSubtype subtype = ObjectCache.GetPreloadedTotemType(intendedSubtype);


                    if (oldItem.newShiny)
                    {
                        Actions.Add(new CreateNewSoulTotem(oldItem.sceneName, oldItem.x, oldItem.y + CreateNewSoulTotem.Elevation[subtype] - oldItem.elevation, totemName, newItemName, location, subtype, intendedSubtype));
                    }
                    else
                    {
                        Actions.Add(new ReplaceObjectWithSoulTotem(oldItem.sceneName, oldItem.objectName, oldItem.elevation, totemName, newItemName, location, subtype, intendedSubtype));
                        preventSelfDestruct();
                    }
                }
                else if (location.StartsWith("450_Geo-Egg_Shop"))
                {
                    string newShinyName = "Randomizer Shiny " + location;     // lazy way to let the Jiji FSM edit determine what to do about the shiny

                    int cost = settings.GetVariableCost(location);
                    Actions.Add(new CreateInactiveShiny(oldItem.sceneName, oldItem.objectName, newShinyName, oldItem.x, oldItem.y,
                        () => Ref.PD.jinnEggsSold >= cost));

                    oldItem.objectName = newShinyName;
                    oldItem.fsmName = "Shiny Control";
                    oldItem.type = ItemType.Charm;
                }
                else if (location == "Boss_Geo-Gruz_Mother")
                {
                    string newShinyName = "Randomizer Shiny " + newShinies++;
                    string parentName = newShinyName + " Parent";

                    Actions.Add(new CreateInactiveShiny(oldItem.sceneName, parentName, newShinyName, oldItem.x, oldItem.y,
                        oldItem.boolDataScene, oldItem.boolDataId));
                    Actions.Add(new ChangeGruzMomReward(parentName));

                    oldItem.objectName = newShinyName;
                    oldItem.fsmName = "Shiny Control";
                    oldItem.type = ItemType.Charm;
                }
                else if (!string.IsNullOrEmpty(oldItem.pdBool))
                {
                    string newShinyName = "Randomizer Shiny " + newShinies++;
                    string parentName = newShinyName + " Parent";

                    Actions.Add(new CreateInactiveShiny(oldItem.sceneName, parentName, newShinyName, oldItem.x, oldItem.y, oldItem.pdBool));
                    Actions.Add(new ActivateEnemyShiny(oldItem.sceneName, oldItem.enemyName, parentName));
                    
                    oldItem.objectName = newShinyName;
                    oldItem.fsmName = "Shiny Control";
                    oldItem.type = ItemType.Charm;
                }
                else if (!string.IsNullOrEmpty(oldItem.boolDataId))
                {
                    string newShinyName = "Randomizer Shiny " + newShinies++;
                    string parentName = newShinyName + " Parent";

                    Actions.Add(new CreateInactiveShiny(oldItem.sceneName, parentName, newShinyName, oldItem.x, oldItem.y, 
                        oldItem.boolDataScene, oldItem.boolDataId));
                    Actions.Add(new ActivateEnemyShiny(oldItem.sceneName, oldItem.enemyName, parentName));

                    oldItem.objectName = newShinyName;
                    oldItem.fsmName = "Shiny Control";
                    oldItem.type = ItemType.Charm;
                }
                else if (oldItem.replace)
                {
                    string replaceShinyName = "Randomizer Shiny " + newShinies++;
                    if (location == "Dream_Nail" || location == "Mask_Shard-Brooding_Mawlek" || location == "Nailmaster's_Glory" || location == "Godtuner")
                    {
                        replaceShinyName = "Randomizer Shiny"; // legacy name for scene edits
                    }
                    oldItem.fsmName = "Shiny Control";
                    oldItem.type = ItemType.Charm;

                    if (settings.NPCItemDialogue && location == "Vengeful_Spirit")
                    {
                        Actions.Add(new ReplaceObjectWithShiny(oldItem.sceneName, "Vengeful Spirit", replaceShinyName));
                        Actions.Add(new ReplaceVengefulSpiritWithShiny(oldItem.sceneName, replaceShinyName, location));
                    }
                    else if (location == "Vessel_Fragment-Basin")
                    {
                        if (settings.NPCItemDialogue)
                        {
                            Actions.Add(new ReplaceObjectWithShiny(oldItem.sceneName, oldItem.objectName, replaceShinyName));
                            Actions.Add(new ReplaceBasinVesselWithShiny(replaceShinyName));
                        }
                        else
                        {
                            Actions.Add(new CreateNewShiny(oldItem.sceneName, oldItem.x, oldItem.y, replaceShinyName));
                            oldItem.fsmName = "Shiny Control";
                        }
                    }
                    else if (settings.NPCItemDialogue && oldItem.objectName == "Egg Sac")
                    {
                        Actions.Add(new CreateInactiveShiny(oldItem.sceneName, replaceShinyName + " Parent", replaceShinyName, oldItem.x, oldItem.y, 
                            oldItem.sceneName, oldItem.objectName));
                        Actions.Add(new ReplaceBluggsacReward(oldItem.sceneName, replaceShinyName));
                    }
                    else
                    {
                        if (location == "Hunter's_Journal")
                        {
                            Actions.Add(new ReplaceJournalWithShiny(replaceShinyName));
                        }
                        Actions.Add(new ReplaceObjectWithShiny(oldItem.sceneName, oldItem.objectName, replaceShinyName));
                    }
                    oldItem.objectName = replaceShinyName;
                }

                else if (oldItem.newShiny)
                {
                    string newShinyName = "New Shiny " + newShinies++;
                    if (location == "Simple_Key-Lurker")
                    {
                        newShinyName = "New Shiny"; // legacy name for scene edits
                    }
                    else if (location.StartsWith("Boss_Geo-Gruz_Mother"))
                    {
                        newShinyName = "New Shiny Boss Geo";
                    }
                    Actions.Add(new CreateNewShiny(oldItem.sceneName, oldItem.x, oldItem.y, newShinyName));
                    oldItem.objectName = newShinyName;
                    oldItem.fsmName = "Shiny Control";
                    oldItem.type = ItemType.Charm;
                }

                else if ((oldItem.type == ItemType.Geo || oldItem.pool == "JunkPitChest") && newItem.type != ItemType.Geo)
                {
                    string newShinyName = "Randomizer Chest Shiny " + newShinies++;
                    Actions.Add(new AddShinyToChest(oldItem.sceneName, oldItem.objectName, oldItem.fsmName,
                        newShinyName));
                    oldItem.objectName = newShinyName;
                    oldItem.fsmName = "Shiny Control";
                    oldItem.type = ItemType.Charm;
                }
                else if (oldItem.type == ItemType.Flame)
                {
                    // Even if the new item is also a flame, this action should still run in order to
                    // guarantee that the player can't be locked out of getting it by upgrading their
                    // Grimmchild.
                    Actions.Add(new ChangeGrimmkinReward(oldItem.sceneName, oldItem.objectName, oldItem.fsmName, newItem.action, newItemName, location));
                    continue;
                }
                else if (oldItem.pool == "Essence_Boss")
                {
                    Actions.Add(new ChangeBossEssenceReward(oldItem.sceneName, oldItem.objectName, oldItem.fsmName, newItem.action, newItemName, location));
                    continue;
                }

                // Dream nail needs a special case
                if (location == "Dream_Nail")
                {
                    Actions.Add(new ChangeBoolTest("RestingGrounds_04", "Binding Shield Activate", "FSM", "Check",
                        newItemName, playerdata: false, 
                        altTest:() => RandomizerMod.Instance.Settings.CheckLocationFound(location)));
                    Actions.Add(new ChangeBoolTest("RestingGrounds_04", "Dreamer Plaque Inspect",
                        "Conversation Control", "End", newItemName, playerdata: false,
                        altTest:() => RandomizerMod.Instance.Settings.CheckLocationFound(location)));
                    Actions.Add(new ChangeBoolTest("RestingGrounds_04", "Dreamer Scene 2", "Control", "Init",
                        newItemName, playerdata: false,
                        altTest: () => RandomizerMod.Instance.Settings.CheckLocationFound(location)));
                    Actions.Add(new ChangeBoolTest("RestingGrounds_04", "PreDreamnail", "FSM", "Check",
                        newItemName, playerdata: false,
                        altTest: () => RandomizerMod.Instance.Settings.CheckLocationFound(location)));
                    Actions.Add(new ChangeBoolTest("RestingGrounds_04", "PostDreamnail", "FSM", "Check",
                        newItemName, playerdata: false,
                        altTest: () => RandomizerMod.Instance.Settings.CheckLocationFound(location)));
                }

                if (replaced)
                {
                    continue;
                }

                switch (newItem.type)
                {
                    default:
                        Actions.Add(new ChangeShinyIntoItem(oldItem.sceneName, oldItem.objectName, oldItem.fsmName,
                            newItem.action, newItemName, location, newItem.nameKey, newItem.shopSpriteKey));
                        if (!string.IsNullOrEmpty(oldItem.altObjectName))
                        {
                            Actions.Add(new ChangeShinyIntoItem(oldItem.sceneName, oldItem.altObjectName, oldItem.fsmName,
                                newItem.action, newItemName, location, newItem.nameKey, newItem.shopSpriteKey));
                        }
                        break;

                    case ItemType.Big:
                    case ItemType.Spell:
                        BigItemDef[] newItemsArray = GetBigItemDefArray(newItemName);

                        Actions.Add(new ChangeShinyIntoBigItem(oldItem.sceneName, oldItem.objectName,
                            oldItem.fsmName, newItemsArray, newItem.action, newItemName, location));
                        if (!string.IsNullOrEmpty(oldItem.altObjectName))
                        {
                            Actions.Add(new ChangeShinyIntoBigItem(oldItem.sceneName, oldItem.altObjectName,
                                oldItem.fsmName, newItemsArray, newItem.action, newItemName, location));
                        }

                        break;

                    case ItemType.Geo:
                        if (oldItem.inChest)
                        {
                            Actions.Add(new ChangeChestGeo(oldItem.sceneName, oldItem.chestName,
                                oldItem.chestFsmName, newItem.geo, newItemName, location));
                        }
                        else if (oldItem.type == ItemType.Geo || oldItem.pool == "JunkPitChest")
                        {
                            Actions.Add(new ChangeChestGeo(oldItem.sceneName, oldItem.objectName, oldItem.fsmName,
                                    newItem.geo, newItemName, location));
                        }
                        else
                        {
                            Actions.Add(new ChangeShinyIntoGeo(oldItem.sceneName, oldItem.objectName,
                                oldItem.fsmName, newItem.geo, newItemName, location));

                            if (!string.IsNullOrEmpty(oldItem.altObjectName))
                            {
                                Actions.Add(new ChangeShinyIntoGeo(oldItem.sceneName, oldItem.altObjectName,
                                    oldItem.fsmName, newItem.geo, newItemName, location));
                            }
                        }
                        break;
                    case ItemType.Lifeblood:
                        Actions.Add(new ChangeShinyIntoLifeblood(oldItem.sceneName, oldItem.objectName, oldItem.fsmName, newItem.lifeblood, newItemName, location));
                        
                        if (!string.IsNullOrEmpty(oldItem.altObjectName))
                        {
                            Actions.Add(new ChangeShinyIntoLifeblood(oldItem.sceneName, oldItem.altObjectName,
                                oldItem.fsmName, newItem.lifeblood, newItemName, location));
                        }
                        break;

                    case ItemType.Soul:
                        Actions.Add(new ChangeShinyIntoSoul(oldItem.sceneName, oldItem.objectName,
                            oldItem.fsmName, newItemName, location));

                        if (!string.IsNullOrEmpty(oldItem.altObjectName))
                        {
                            Actions.Add(new ChangeShinyIntoSoul(oldItem.sceneName, oldItem.altObjectName,
                                oldItem.fsmName, newItemName, location));
                        }
                        break;

                    case ItemType.Lore:
                        newItem.loreSheet = string.IsNullOrEmpty(newItem.loreSheet) ? "Lore Tablets" : newItem.loreSheet;

                        Actions.Add(new ChangeShinyIntoText(oldItem.sceneName, oldItem.objectName, oldItem.fsmName,
                            newItem.loreKey, newItem.loreSheet, newItem.textType, newItemName, location));

                        if (!string.IsNullOrEmpty(oldItem.altObjectName))
                        {
                            Actions.Add(new ChangeShinyIntoText(oldItem.sceneName, oldItem.altObjectName, oldItem.fsmName,
                                newItem.loreKey, newItem.loreSheet, newItem.textType, newItemName, location));
                        }
                        break;
                }

                if (hasCost)
                {
                    int cost = oldItem.cost;
                    if (oldItem.costType == AddYNDialogueToShiny.CostType.Essence || oldItem.costType == AddYNDialogueToShiny.CostType.Grub)
                    {
                        cost = settings.GetVariableCost(location);
                    }

                    Actions.Add(new AddYNDialogueToShiny(
                        oldItem.sceneName,
                        oldItem.objectName,
                        oldItem.fsmName,
                        newItem.nameKey,
                        cost,
                        oldItem.costType));
                }
            }

            List<ChangeShopContents> shopActions = new List<ChangeShopContents>();

            // TODO: Change to use additiveItems rather than hard coded
            // No point rewriting this before making the shop component
            foreach ((string shopItem, string shopName) in items.Where(item => shopNames.Contains(item.Item2)))
            {
                ReqDef newItem = LogicManager.GetItemDef(shopItem);

                GiveAction giveAction = newItem.action;
                if (giveAction == GiveAction.SpawnGeo)
                {
                    giveAction = GiveAction.AddGeo;
                }

                string boolName = "RandomizerMod." + giveAction.ToString() + "." + shopItem + "." + shopName;

                ShopItemBoolNames[(shopItem, shopName)] = boolName;
                
                ShopItemDef newItemDef = new ShopItemDef
                {
                    PlayerDataBoolName = boolName,
                    NameConvo = newItem.nameKey,
                    DescConvo = newItem.shopDescKey,
                    RequiredPlayerDataBool = LogicManager.GetShopDef(shopName).requiredPlayerDataBool,
                    RemovalPlayerDataBool = string.Empty,
                    DungDiscount = LogicManager.GetShopDef(shopName).dungDiscount,
                    NotchCostBool = newItem.notchCost,
                    Cost = settings.ShopCosts.First(pair => pair.Item1 == shopItem).Item2,
                    SpriteName = newItem.shopSpriteKey
                };

                if (newItemDef.Cost == 0)
                {
                    newItemDef.Cost = 1;
                    LogWarn($"Found item {shopItem} in {shopName} with no saved cost.");
                }

                if (newItemDef.Cost < 5)
                {
                    newItemDef.DungDiscount = false;
                }

                ChangeShopContents existingShopAction = shopActions.FirstOrDefault(action =>
                    action.SceneName == LogicManager.GetShopDef(shopName).sceneName &&
                    action.ObjectName == LogicManager.GetShopDef(shopName).objectName);

                if (existingShopAction == null)
                {
                    shopActions.Add(new ChangeShopContents(LogicManager.GetShopDef(shopName).sceneName,
                        LogicManager.GetShopDef(shopName).objectName, new[] { newItemDef }));
                }
                else
                {
                    existingShopAction.AddItemDefs(new[] { newItemDef });
                }
            }

            shopActions.ForEach(action => Actions.Add(action));

            // Add an action for each shop to allow showing Lore
            if (settings.RandomizeLoreTablets || settings.RandomizePalaceTablets)
            {
                Actions.Add(new ShowLoreTextInShop(SceneNames.Room_shop, "UI List", "Confirm Control"));
                Actions.Add(new ShowLoreTextInShop(SceneNames.Room_mapper, "UI List", "Confirm Control"));
                Actions.Add(new ShowLoreTextInShop(SceneNames.Room_Charm_Shop, "UI List", "Confirm Control"));
                Actions.Add(new ShowLoreTextInShop(SceneNames.Fungus2_26, "UI List", "Confirm Control"));
            }

            // Mimics/Grubs when grubs aren't rando
            if (settings.RandomizeMimics && !settings.RandomizeGrubs)
            {
                foreach (var kvp in settings._mimicPlacements)
                {
                    ReqDef def = LogicManager.GetItemDef(kvp.Key);
                    if (kvp.Value)
                    {
                        if (def.replace)
                        {
                            Actions.Add(new ReplaceObjectWithMimicBottle(def.sceneName, def.objectName, def.elevation,
                                "Randomizer Mimic " + kvp.Key, "Mimic_Grub", kvp.Key, unrandomized: true));
                        }
                        else
                        {
                            Actions.Add(new CreateNewMimicBottle(def.sceneName, def.x, def.y + CreateNewMimicBottle.MIMIC_BOTTLE_ELEVATION - def.elevation,
                                "Randomizer Mimic " + kvp.Key, "Mimic_Grub", kvp.Key, unrandomized:true));
                        }
                    }
                    else
                    {
                        if (def.replace)
                        {
                            Actions.Add(new ReplaceObjectWithGrubJar(def.sceneName, def.objectName, def.elevation,
                                "Randomizer Grub " + kvp.Key, "Grub", kvp.Key, unrandomized:true));
                        }
                        else
                        {
                            Actions.Add(new CreateNewGrubJar(def.sceneName, def.x, def.y + CreateNewGrubJar.GRUB_JAR_ELEVATION - def.elevation,
                                "Randomizer Grub " + kvp.Key, "Grub", kvp.Key, unrandomized:true));
                        }
                    }
                }
            }
        }

        private static GeoRockSubtype GetRockSubtype(string objName) {
            GeoRockSubtype subtype = GeoRockSubtype.Default;
            if (objName.Contains("Abyss")) {
                subtype = GeoRockSubtype.Abyss;
            }
            else if (objName.Contains("City")) {
                subtype = GeoRockSubtype.City;
            }
            else if (objName.Contains("Deepnest")) {
                subtype = GeoRockSubtype.Deepnest;
            }
            else if (objName.Contains("Fung 01")) {
                subtype = GeoRockSubtype.Fung01;
            }
            else if (objName.Contains("Fung 02")) {
                subtype = GeoRockSubtype.Fung02;
            }
            else if (objName.Contains("Grave 01")) {
                subtype = GeoRockSubtype.Grave01;
            }
            else if (objName.Contains("Grave 02")) {
                subtype = GeoRockSubtype.Grave02;
            }
            else if (objName.Contains("Green Path 01")) {
                subtype = GeoRockSubtype.GreenPath01;
            }
            else if (objName.Contains("Green Path 02")) {
                subtype = GeoRockSubtype.GreenPath02;
            }
            else if (objName.Contains("Hive")) {
                subtype = GeoRockSubtype.Hive;
            }
            else if (objName.Contains("Mine")) {
                subtype = GeoRockSubtype.Mine;
            }
            else if (objName.Contains("Outskirts")) {
                subtype = GeoRockSubtype.Outskirts;
            }
            else if (objName == "Giant Geo Egg") {
                subtype = GeoRockSubtype.Outskirts420;
            }

            return ObjectCache.GetPreloadedRockType(subtype);
        }

        private static SoulTotemSubtype GetTotemSubtype(string objName)
        {
            var subtype = SoulTotemSubtype.A;
            if (objName == "Soul Totem 5") {
                subtype = SoulTotemSubtype.A;
            }
            else if (objName == "Soul Totem mini_two_horned") {
                subtype = SoulTotemSubtype.B;
            }
            else if (objName == "Soul Totem mini_horned") {
                subtype = SoulTotemSubtype.C;
            }
            else if (objName == "Soul Totem 1") {
                subtype = SoulTotemSubtype.D;
            }
            else if (objName == "Soul Totem 4") {
                subtype = SoulTotemSubtype.E;
            }
            else if (objName == "Soul Totem 2") {
                subtype = SoulTotemSubtype.F;
            }
            else if (objName == "Soul Totem 3") {
                subtype = SoulTotemSubtype.G;
            }
            else if (objName == "Soul Totem white") {
                subtype = SoulTotemSubtype.Palace;
            }
            else if (objName.StartsWith("Soul Totem white_Infinte")) {
                subtype = SoulTotemSubtype.PathOfPain;
            }

            return subtype;
        }

        public static string GetAdditivePrefix(string itemName)
        {
            return LogicManager.AdditiveItemNames.FirstOrDefault(itemSet =>
                LogicManager.GetAdditiveItems(itemSet).Contains(itemName));
        }

        private static BigItemDef[] GetBigItemDefArray(string itemName)
        {
            itemName = LogicManager.RemoveDuplicateSuffix(itemName);
            string prefix = GetAdditivePrefix(itemName);
            if (prefix != null)
            {
                return LogicManager.GetAdditiveItems(prefix)
                    .Select(LogicManager.GetItemDef)
                    .Select(item => new BigItemDef
                    {
                        Name = itemName,
                        BoolName = item.boolName,
                        SpriteKey = item.bigSpriteKey,
                        TakeKey = item.takeKey,
                        NameKey = item.nameKey,
                        ButtonKey = item.buttonKey,
                        DescOneKey = item.descOneKey,
                        DescTwoKey = item.descTwoKey
                    }).ToArray();
            }

            ReqDef item2 = LogicManager.GetItemDef(itemName);
            return new[]
            {
                new BigItemDef
                {
                    Name = itemName,
                    BoolName = item2.boolName,
                    SpriteKey = item2.bigSpriteKey,
                    TakeKey = item2.takeKey,
                    NameKey = item2.nameKey,
                    ButtonKey = item2.buttonKey,
                    DescOneKey = item2.descOneKey,
                    DescTwoKey = item2.descTwoKey
                }
            };
        }

        private static string GetAdditiveBoolName(string boolName, ref Dictionary<string, int> additiveCounts)
        {
            if (additiveCounts == null)
            {
                additiveCounts = LogicManager.AdditiveItemNames.ToDictionary(str => str, str => 0);
            }

            string prefix = GetAdditivePrefix(boolName);
            if (string.IsNullOrEmpty(prefix))
            {
                return null;
            }

            additiveCounts[prefix] = additiveCounts[prefix] + 1;
            AdditiveBoolNames[boolName] = prefix + additiveCounts[prefix];
            return prefix + additiveCounts[prefix];
        }

        public static void Hook()
        {
            UnHook();
            
            On.PlayMakerFSM.OnEnable += ProcessFSM;
            On.HealthManager.Die += OnEnemyDeath;
        }

        public static void UnHook()
        {
            On.PlayMakerFSM.OnEnable -= ProcessFSM;
            On.HealthManager.Die -= OnEnemyDeath;
        }

        public static void ProcessFSM(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM fsm)
        {
            orig(fsm);

            string scene = fsm.gameObject.scene.name;

            foreach (RandomizerAction action in Actions)
            {
                if (action.Type != ActionType.PlayMakerFSM)
                {
                    continue;
                }

                try
                {
                    action.Process(scene, fsm);
                }
                catch (Exception e)
                {
                    LogError(
                        $"Error processing PlayMakerFSM action of type {action.GetType()} in scene {scene}:\n{JsonUtility.ToJson(action)}\n{e}");
                }
            }
        }

        public static void EditShinies()
        {
            string scene = Ref.GM.GetSceneNameString();

            foreach (RandomizerAction action in Actions)
            {
                if (action.Type != ActionType.GameObject)
                {
                    continue;
                }

                try
                {
                    action.Process(scene, null);
                }
                catch (Exception e)
                {
                    LogError(
                        $"Error processing GameObject action of type {action.GetType()} in scene {scene}:\n{JsonUtility.ToJson(action)}\n{e}");
                }
            }
        }

        private static void OnEnemyDeath(On.HealthManager.orig_Die orig, HealthManager hm, 
            float? attackDirection, AttackTypes attackType, bool ignoreEvasion)
        {
            string scene = Ref.GM.GetSceneNameString();

            foreach (RandomizerAction action in Actions)
            {
                if (action.Type != ActionType.EnemyDeath)
                {
                    continue;
                }

                try
                {
                    action.Process(scene, hm);
                }
                catch (Exception e)
                {
                    LogError(
                        $"Error processing EnemyDeath action of type {action.GetType()} in scene {scene}:\n{JsonUtility.ToJson(action)}\n{e}");
                }
            }

            orig(hm, attackDirection, attackType, ignoreEvasion);   // Call the original after we set the geo
        }



        public abstract void Process(string scene, Object changeObj);
    }
}
