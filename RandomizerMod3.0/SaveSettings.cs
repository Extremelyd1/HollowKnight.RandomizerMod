using System;
using System.Collections.Generic;
using System.Linq;
using GlobalEnums;
using RandomizerMod.Actions;
using RandomizerMod.Randomization;

namespace RandomizerMod
{
    public class SaveSettings
    {
        /*
         * UNLISTED BOOLS
         * rescuedSly is used in room randomizer to control when Sly appears in the shop, separately from when the door is unlocked
         */
        public Dictionary<string, bool> _bools = new Dictionary<string, bool>();

        public bool GetBool(bool defaultValue, string name)
        {
            if (_bools.TryGetValue(name, out var value))
            {
                return value;
            }

            return defaultValue;
        }
        
        public bool GetBool(string name)
        {
            return GetBool(false, name);
        }

        public void SetBool(bool value, string name)
        {
            _bools[name] = value;
        }

        public Dictionary<string, string> _itemPlacements = new Dictionary<string, string>();
        public Dictionary<string, int> _orderedLocations = new Dictionary<string, int>();
        public Dictionary<string, string> _transitionPlacements = new Dictionary<string, string>();
        public Dictionary<string, int> _variableCosts = new Dictionary<string, int>();
        public Dictionary<string, int> _shopCosts = new Dictionary<string, int>();
        public Dictionary<string, int> _additiveCounts = new Dictionary<string, int>();
        
        public Dictionary<string, bool> _obtainedItems = new Dictionary<string, bool>();
        public Dictionary<string, bool> _obtainedLocations = new Dictionary<string, bool>();
        public Dictionary<string, bool> _obtainedTransitions = new Dictionary<string, bool>();
        
        public Dictionary<string, bool> _mimicPlacements = new Dictionary<string, bool>();

        /// <remarks>item, location</remarks>
        public (string, string)[] ItemPlacements => _itemPlacements.Select(pair => (pair.Key, pair.Value)).ToArray();

        public int NumItemsFound => _obtainedItems.Keys.Intersect(_itemPlacements.Keys).Count();

        public int MaxOrder => _orderedLocations.Count;

        public (string, int)[] VariableCosts => _variableCosts.Select(pair => (pair.Key, pair.Value)).ToArray();
        public int GetVariableCost(string item) => _variableCosts[item]; 
        public (string, int)[] ShopCosts => _shopCosts.Select(pair => (pair.Key, pair.Value)).ToArray();

        public bool RandomizeTransitions => RandomizeAreas || RandomizeRooms;

        public bool FreeLantern => !(DarkRooms || RandomizeKeys);

        public int JijiHintCounter { get; set; } = 0;

        public int QuirrerHintCounter { get; set; } = 0;

        public bool AllBosses  { get; set; } = false;

        public bool AllSkills  { get; set; } = false;

        public bool AllCharms  { get; set; } = false;

        public bool CharmNotch  { get; set; } = false;

        public bool Grubfather  { get; set; } = false;
        public bool Jiji  { get; set; } = false;
        public bool JinnSellAll  { get; set; } = false;
        public bool Quirrel  { get; set; } = false;
        public bool ItemDepthHints  { get; set; } = false;

        public bool EarlyGeo  { get; set; } = false;

        public bool NPCItemDialogue  { get; set; } = false;

        public bool ExtraPlatforms  { get; set; } = false;

        public bool Randomizer  { get; set; } = false;
        public bool RandomizeAreas  { get; set; } = false;
        public bool RandomizeRooms  { get; set; } = false;
        public bool ConnectAreas  { get; set; } = false;
        public bool SlyCharm  { get; set; } = false;
        public bool RandomizeDreamers  { get; set; } = false;
        public bool RandomizeSkills  { get; set; } = false;
        public bool RandomizeCharms  { get; set; } = false;
        public bool RandomizeKeys  { get; set; } = false;
        public bool RandomizeGeoChests  { get; set; } = false;
        public bool RandomizeJunkPitChests  { get; set; } = false;
        public bool RandomizeMaskShards  { get; set; } = false;
        public bool RandomizeVesselFragments  { get; set; } = false;
        public bool RandomizeCharmNotches  { get; set; } = false;
        public bool RandomizePaleOre  { get; set; } = false;
        public bool RandomizeRancidEggs  { get; set; } = false;
        public bool EggShop  { get; set; } = false;
        public int MaxEggCost => !EggShop ? 0 : VariableCosts
            .Where(pair => LogicManager.GetItemDef(pair.Item1).costType == AddYNDialogueToShiny.CostType.RancidEggs)
            .Select(pair => pair.Item2)
            .Max();


        public bool RandomizeRelics  { get; set; } = false;

        public bool RandomizeMaps  { get; set; } = false;

        public bool RandomizeStags  { get; set; } = false;

        public bool RandomizeGrubs  { get; set; } = false;
        public bool RandomizeMimics  { get; set; } = false;

        public bool RandomizeWhisperingRoots  { get; set; } = false;
        
        public bool RandomizeRocks  { get; set; } = false;

        public bool RandomizeBossGeo  { get; set; } = false;
        
        public bool RandomizeSoulTotems  { get; set; } = false;

        public bool RandomizeLoreTablets  { get; set; } = false;
        public bool RandomizePalaceTotems  { get; set; } = false;
        public bool RandomizePalaceTablets  { get; set; } = false;
        public bool RandomizePalaceEntries  { get; set; } = false;

        public bool RandomizeLifebloodCocoons  { get; set; } = false;

        public bool RandomizeGrimmkinFlames  { get; set; } = false;
        public int TotalFlamesCollected { get; set; } = 0;

        public bool RandomizeBossEssence  { get; set; } = false;

        public bool RandomizeJournalEntries  { get; set; } = false;

        public bool DuplicateMajorItems  { get; set; } = false;

        public bool RandomizeCloakPieces  { get; set; } = false;
        public bool RandomizeClawPieces  { get; set; } = false;

        public bool RandomizeNotchCosts  { get; set; } = true;

        public bool RandomizeFocus  { get; set; } = false;

        public bool RandomizeSwim  { get; set; } = true;
        public bool ElevatorPass  { get; set; } = true;

        public bool CursedNail  { get; set; } = false;

        public bool CursedNotches  { get; set; } = false;

        public bool CursedMasks  { get; set; } = false;

        internal bool GetRandomizeByPool(string pool)
        {
            switch (pool)
            {
                case "Dreamer":
                    return RandomizeDreamers;
                case "Skill":
                    return RandomizeSkills;
                case "SplitClaw":
                    return RandomizeClawPieces;
                case "SplitCloak":
                case "SplitCloakLocation":
                    return RandomizeCloakPieces;
                case "Charm":
                    return RandomizeCharms;
                case "Key":
                    return RandomizeKeys;
                case "Mask":
                    return RandomizeMaskShards;
                case "Vessel":
                    return RandomizeVesselFragments;
                case "Ore":
                    return RandomizePaleOre;
                case "Notch":
                    return RandomizeCharmNotches;
                case "Geo":
                    return RandomizeGeoChests;
                case "Egg":
                    return RandomizeRancidEggs;
                case "EggShopItem":
                case "EggShopLocation":
                    return EggShop;
                case "Relic":
                    return RandomizeRelics;
                case "Map":
                    return RandomizeMaps;
                case "Stag":
                    return RandomizeStags;
                case "Grub":
                    return RandomizeGrubs;
                case "Root":
                    return RandomizeWhisperingRoots;
                case "Rock":
                    return RandomizeRocks;
                case "Soul":
                    return RandomizeSoulTotems;
                case "PalaceSoul":
                    return RandomizePalaceTotems;
                case "PalaceLore":
                    return RandomizePalaceTablets;
                case "Lore":
                    return RandomizeLoreTablets;
                case "Journal":
                    return RandomizeJournalEntries;
                case "PalaceJournal":
                    return RandomizePalaceEntries;
                case "Lifeblood":
                    return RandomizeLifebloodCocoons;
                case "Flame":
                    return RandomizeGrimmkinFlames;
                case "Essence_Boss":
                    return RandomizeBossEssence;
                case "Boss_Geo":
                    return RandomizeBossGeo;
                case "CursedNail":
                    return CursedNail;
                case "CursedNotch":
                    return CursedNotches;
                case "CursedMask":
                    return CursedMasks;
                case "Focus":
                    return RandomizeFocus;
                case "Swim":
                    return RandomizeSwim;
                case "Fake":
                default:
                    return false;
            }
        }


        public bool CreateSpoilerLog  { get; set; } = false;

        public bool Cursed  { get; set; } = false;

        public bool RandomizeStartItems  { get; set; } = false;

        public bool RandomizeStartLocation  { get; set; } = false;

        // The following settings names are referenced in Benchwarp. Please do not change!
        public string StartName  { get; set; } = "King's Pass";

        public string StartSceneName  { get; set; } = "Tutorial_01";

        public string StartRespawnMarkerName  { get; set; } = "Randomizer Respawn Marker";

        public int StartRespawnType { get; set; } = 0;

        public int StartMapZone { get; set; } = (int)MapZone.KINGS_PASS;
        // End Benchwarp block.

        public bool ShadeSkips  { get; set; } = false;

        public bool AcidSkips  { get; set; } = false;

        public bool SpikeTunnels  { get; set; } = false;

        public bool MildSkips  { get; set; } = false;

        public bool SpicySkips  { get; set; } = false;

        public bool FireballSkips  { get; set; } = false;

        public bool DarkRooms  { get; set; } = false;

        public int Seed { get; set; } = -1;

        public void ResetPlacements()
        {
            _itemPlacements = new Dictionary<string, string>();
            _orderedLocations = new Dictionary<string, int>();
            _transitionPlacements = new Dictionary<string, string>();
            _variableCosts = new Dictionary<string, int>();
            _shopCosts = new Dictionary<string, int>();
            _additiveCounts = new Dictionary<string, int>();

            _obtainedItems = new Dictionary<string, bool>();
            _obtainedLocations = new Dictionary<string, bool>();
            _obtainedTransitions = new Dictionary<string, bool>();
        }

        public void AddItemPlacement(string item, string location)
        {
            _itemPlacements[item] = location;
        }

        public void RemoveItem(string oldItem)
        {
            _itemPlacements.Remove(oldItem);
        }

        public void AddOrderedLocation(string location, int order)
        {
            _orderedLocations[location] = order;
        }

        public int GetLocationOrder(string location)
        {
            return _orderedLocations[location];
        }

        public string GetNthLocation(int n)
        {
            return _orderedLocations.FirstOrDefault(kvp => kvp.Value == n).Key;
        }

        public string[] GetNthLocationItems(int n)
        {
            string location = GetNthLocation(n);
            return ItemPlacements.Where(pair => pair.Item2 == location).Select(pair => pair.Item1).ToArray();
        }

        public string GetItemPlacedAt(string location)
        {
            foreach (var ilp in _itemPlacements)
            {
                if (ilp.Value == location)
                {
                    return ilp.Key;
                }
            }
            return "";
        }
        
        public void AddTransitionPlacement(string entrance, string exit)
        {
            _transitionPlacements[entrance] = exit;
        }

        public void AddNewCost(string item, int cost)
        {
            _variableCosts[item] = cost;
        }

        public void AddShopCost(string item, int cost)
        {
            _shopCosts[item] = cost;
        }

        public int GetShopCost(string item)
        {
            return _shopCosts[item];
        }

        public bool HasShopCost(string item)
        {
            return _shopCosts.ContainsKey(item);
        }

        public void RemoveShopCost(string item)
        {
            if (!_shopCosts.ContainsKey(item)) return;
            _shopCosts.Remove(item);
        }

        public void MarkItemFound(string item)
        {
            _obtainedItems[item] = true;
        }

        public bool CheckItemFound(string item)
        {
            if (!_obtainedItems.TryGetValue(item, out bool found)) return false;
            return found;
        }

        public string[] GetItemsFound()
        {
            return _obtainedItems.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToArray();
        }

        public int GetNumLocations()
        {
            return _orderedLocations.Count + _shopCosts.Count - 5;
        }

        public HashSet<string> GetPlacedItems()
        {
            return new HashSet<string>(ItemPlacements.Select(pair => pair.Item1));
        }

        public void MarkLocationFound(string location)
        {
            if (string.IsNullOrEmpty(location)) return;
            _obtainedLocations[location] = true;
        }

        public bool CheckLocationFound(string location)
        {
            if (!_obtainedLocations.TryGetValue(location, out bool found)) return false;
            return found;
        }

        public string[] GetLocationsFound()
        {
            return _obtainedLocations.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToArray();
        }

        public void MarkTransitionFound(string transition)
        {
            _obtainedTransitions[transition] = true;
        }

        public bool CheckTransitionFound(string transition)
        {
            if (!_obtainedTransitions.TryGetValue(transition, out bool found)) return false;
            return found;
        }

        public string[] GetTransitionsFound()
        {
            return _obtainedTransitions.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToArray();
        }

        // Returns the actual item that will be obtained by picking up the given item; these may differ
        // if the pickup is part of an additive group.
        public string GetEffectiveItem(string item)
        {
            string[] additiveSet = LogicManager.AdditiveItemSets.FirstOrDefault(set => set.Contains(item));
            if (additiveSet != null)
            {
                int count = Math.Min(GetAdditiveCount(item), additiveSet.Length - 1);
                item = additiveSet[count];
            }
            // Add special case for dealing with L/R shade cloak; if they already have at least one dash in each direction
            // we just show Shade Cloak, to prevent possible confusion. In RecentItems, it's probably more helpful to show
            // the direction of the shade cloak, so as not to destroy relevant information.
            // - Deactivated because I felt that destroying the information about which shade cloak it is is more
            // annoying than showing an incorrect dash direction.
            /*
            if (LogicManager.GetItemDef(item).pool == "SplitCloak" && compressSplit)
            {
                if (GetAdditiveCount("Left_Mothwing_Cloak") > 0 && GetAdditiveCount("Right_Mothwing_Cloak") > 0)
                {
                    item = "Shade_Cloak";
                }
            }
            */
            return item;
        }

        public int GetAdditiveCount(string item)
        {
            string[] additiveSet = LogicManager.AdditiveItemSets.FirstOrDefault(set => set.Contains(item));
            if (additiveSet is null) return 0;
            if (!_additiveCounts.TryGetValue(additiveSet[0], out int count))
            {
                _additiveCounts.Add(additiveSet[0], 0);
                count = 0;
            }
            return count;
        }

        public void IncrementAdditiveCount(string item)
        {
            string[] additiveSet = LogicManager.AdditiveItemSets.FirstOrDefault(set => set.Contains(item));
            if (additiveSet is null) return;
            if (!_additiveCounts.ContainsKey(additiveSet[0]))
            {
                _additiveCounts.Add(additiveSet[0], 0);
            }
            _additiveCounts[additiveSet[0]]++;

            // Special code for Left/Right Dash so dupes work
            if (LogicManager.GetItemDef(item).pool == "SplitCloak")
            {
                //When we give left/right shade cloak for the first time, increment the other pool
                if (additiveSet[0] == "Left_Mothwing_Cloak" && _additiveCounts[additiveSet[0]] == 2)
                {
                    if (!_additiveCounts.ContainsKey("Right_Mothwing_Cloak")) _additiveCounts.Add("Right_Mothwing_Cloak", 0);
                    _additiveCounts["Right_Mothwing_Cloak"]++;
                }
                else if (additiveSet[0] == "Right_Mothwing_Cloak" && _additiveCounts[additiveSet[0]] == 2)
                {
                    if (!_additiveCounts.ContainsKey("Left_Mothwing_Cloak")) _additiveCounts.Add("Left_Mothwing_Cloak", 0);
                    _additiveCounts["Left_Mothwing_Cloak"]++;
                }
            }
        }
    }


    public class GlobalSettings
    {
        public bool NPCItemDialogue  { get; set; } = true;

        public bool RecentItems  { get; set; } = true;

        public bool ReduceRockPreloads  { get; set; } = true;

        public bool ReduceTotemPreloads  { get; set; } = true;
    }
}
