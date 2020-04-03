﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static RandomizerMod.LogHelper;
using static RandomizerMod.Randomization.Randomizer;

namespace RandomizerMod.Randomization
{
    internal static class PostRandomizer
    {
        public static void PostRandomizationTasks()
        {
            RemovePlaceholders();
            SaveAllPlacements();
            //No vanilla'd loctions in the spoiler log, please!
            (int, string, string)[] orderedILPairs = RandomizerMod.Instance.Settings.ItemPlacements.Except(VanillaManager.Instance.ItemPlacements)
                .Select(pair => (pair.Item2.StartsWith("Equip") ? 0 : ItemManager.locationOrder[pair.Item2], pair.Item1, pair.Item2)).ToArray();
            if (RandomizerMod.Instance.Settings.CreateSpoilerLog) RandoLogger.LogAllToSpoiler(orderedILPairs, RandomizerMod.Instance.Settings._transitionPlacements.Select(kvp => (kvp.Key, kvp.Value)).ToArray());
        }

        private static void RemovePlaceholders()
        {
            if (RandomizerMod.Instance.Settings.DuplicateMajorItems)
            {
                // Duplicate items should not be placed very early in logic
                int minimumDepth = Math.Min(ItemManager.locationOrder.Count / 5, ItemManager.locationOrder.Count - 2 * ItemManager.duplicatedItems.Count);
                int maximumDepth = ItemManager.locationOrder.Count;
                bool ValidIndex(int i)
                {
                    string location = ItemManager.locationOrder.FirstOrDefault(kvp => kvp.Value == i).Key;
                    return !string.IsNullOrEmpty(location) && !LogicManager.ShopNames.Contains(location) && !LogicManager.GetItemDef(ItemManager.nonShopItems[location]).progression;
                }
                List<int> allowedDepths = Enumerable.Range(minimumDepth, maximumDepth).Where(i => ValidIndex(i)).ToList();
                Random rand = new Random(RandomizerMod.Instance.Settings.Seed + 29);

                foreach (string majorItem in ItemManager.duplicatedItems)
                {
                    while (allowedDepths.Any())
                    {
                        int depth = allowedDepths[rand.Next(allowedDepths.Count)];
                        string location = ItemManager.locationOrder.First(kvp => kvp.Value == depth).Key;
                        string swapItem = ItemManager.nonShopItems[location];
                        string toShop = LogicManager.ShopNames.OrderBy(shop => ItemManager.shopItems[shop].Count).First();

                        ItemManager.nonShopItems[location] = majorItem + "_(1)";
                        ItemManager.shopItems[toShop].Add(swapItem);
                        allowedDepths.Remove(depth);
                        break;
                    }
                }
            }
        }

        private static void SaveAllPlacements()
        {
            if (RandomizerMod.Instance.Settings.RandomizeTransitions)
            {
                foreach (KeyValuePair<string, string> kvp in TransitionManager.transitionPlacements)
                {
                    RandomizerMod.Instance.Settings.AddTransitionPlacement(kvp.Key, kvp.Value);
                    // For map tracking
                    //     RandoLogger.LogTransitionToTracker(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, List<string>> kvp in ItemManager.shopItems)
            {
                foreach (string item in kvp.Value)
                {
                    RandomizeShopCost(item);
                }
            }

            foreach (var (item, shop) in VanillaManager.Instance.ItemPlacements.Where(p => LogicManager.ShopNames.Contains(p.Item2)))
            {
                RandomizerMod.Instance.Settings.AddShopCost(item, LogicManager.GetItemDef(item).cost);
            }

            foreach ((string, string) pair in GetPlacedItemPairs())
            {
                RandomizerMod.Instance.Settings.AddItemPlacement(pair.Item1, pair.Item2);
            }

            for (int i = 0; i < startItems.Count; i++)
            {
                RandomizerMod.Instance.Settings.AddItemPlacement(startItems[i], "Equipped_(" + i + ")");
            }

            foreach (var kvp in ItemManager.locationOrder)
            {
                RandomizerMod.Instance.Settings.AddOrderedLocation(kvp.Key, kvp.Value);
            }

            RandomizerMod.Instance.Settings.StartName = StartName;
            StartDef startDef = LogicManager.GetStartLocation(StartName);
            RandomizerMod.Instance.Settings.StartSceneName = startDef.sceneName;
            RandomizerMod.Instance.Settings.StartRespawnMarkerName = OpenMode.RESPAWN_MARKER_NAME;
            RandomizerMod.Instance.Settings.StartRespawnType = 0;
            RandomizerMod.Instance.Settings.StartMapZone = (int)startDef.zone;
        }

        public static int RandomizeShopCost(string item)
        {
            rand = new Random(RandomizerMod.Instance.Settings.Seed + item.GetHashCode()); // make shop item cost independent from prior randomization

            // Give a shopCost to every shop item
            ReqDef def = LogicManager.GetItemDef(item);
            int priceFactor = 1;
            if (def.geo > 0) priceFactor = 0;
            if (item.StartsWith("Rancid") || item.StartsWith("Mask")) priceFactor = 2;
            if (item.StartsWith("Pale_Ore") || item.StartsWith("Charm_Notch")) priceFactor = 3;
            if (item.StartsWith("Godtuner") || item.StartsWith("Collector") || item.StartsWith("World_Sense")) priceFactor = 0;

            int cost;
            if (RandomizerMod.Instance.Settings.GetRandomizeByPool(def.pool))
            {
                cost = (100 + rand.Next(41) * 10) * priceFactor;
            }
            else
            {
                cost = def.shopCost; // this part is never actually reached, because the vm has not merged with the im placements yet
            }
            cost = Math.Max(cost, 1);
            
            RandomizerMod.Instance.Settings.AddShopCost(item, cost);

            return cost;
        }

        public static List<(string, string)> GetPlacedItemPairs()
        {
            List<(string, string)> pairs = new List<(string, string)>();
            foreach (KeyValuePair<string, List<string>> kvp in ItemManager.shopItems)
            {
                foreach (string item in kvp.Value)
                {
                    pairs.Add((item, kvp.Key));
                }
            }
            foreach (KeyValuePair<string, string> kvp in ItemManager.nonShopItems)
            {
                pairs.Add((kvp.Value, kvp.Key));
            }

            //Vanilla Item Placements (for RandomizerActions, Hints, Logs, etc)
            foreach ((string, string) pair in vm.ItemPlacements)
            {
                pairs.Add((pair.Item1, pair.Item2));
            }

            return pairs;
        }

        public static void LogItemPlacements(ProgressionManager pm)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("All Item Placements:");
            foreach ((string, string) pair in GetPlacedItemPairs())
            {
                ReqDef def = LogicManager.GetItemDef(pair.Item1);
                if (def.progression) sb.AppendLine($"--{pm.CanGet(pair.Item2)} - {pair.Item1} -at- {pair.Item2}");
            }

            Log(sb.ToString());
        }
    }
}
