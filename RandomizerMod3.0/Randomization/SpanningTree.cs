using System.Collections.Generic;
using System.Linq;
using static RandomizerMod.LogHelper;
using static RandomizerMod.Randomization.Randomizer;

namespace RandomizerMod.Randomization
{
    internal static class SpanningTree
    {
        public static void BuildAreaSpanningTree()
        {
            List<string> areas = new List<string>();
            Dictionary<string, List<string>> areaTransitions = new Dictionary<string, List<string>>();

            foreach (string transition in LogicManager.TransitionNames())
            {
                if (transition == startTransition) continue;
                TransitionDef def = LogicManager.GetTransitionDef(transition);
                string areaName = def.areaName;
                if (new List<string> { "City_of_Tears", "Forgotten_Crossroads", "Resting_Grounds" }.Contains(areaName)) areaName = "Kings_Station";
                if (new List<string> { "Ancient_Basin", "Kingdoms_Edge" }.Contains(areaName)) areaName = "Deepnest";

                if (!areas.Contains(areaName) && !def.deadEnd && !def.isolated)
                {
                    areas.Add(areaName);
                    areaTransitions.Add(areaName, new List<string>());
                }
            }

            foreach (string transition in LogicManager.TransitionNames())
            {
                if (transition == startTransition) continue;
                TransitionDef def = LogicManager.GetTransitionDef(transition);
                string areaName = def.areaName;
                if (def.oneWay == 0 && areas.Contains(areaName)) areaTransitions[areaName].Add(transition);
            }

            BuildSpanningTree(areaTransitions);
        }

        public static void BuildRoomSpanningTree()
        {
            List<string> rooms = new List<string>();
            Dictionary<string, List<string>> roomTransitions = new Dictionary<string, List<string>>();

            foreach (string transition in LogicManager.TransitionNames())
            {
                if (transition == startTransition) continue;
                TransitionDef def = LogicManager.GetTransitionDef(transition);
                string roomName = def.sceneName;
                if (new List<string> { "Crossroads_46", "Crossroads_46b" }.Contains(roomName)) roomName = "Crossroads_46";
                if (new List<string> { "Abyss_03", "Abyss_03_b", "Abyss_03_c" }.Contains(roomName)) roomName = "Abyss_03";
                if (new List<string> { "Ruins2_10", "Ruins2_10b" }.Contains(roomName)) roomName = "Ruins2_10";

                if (!rooms.Contains(roomName) && !def.deadEnd && !def.isolated)
                {
                    rooms.Add(roomName);
                    roomTransitions.Add(roomName, new List<string>());
                }
            }

            foreach (string transition in LogicManager.TransitionNames())
            {
                if (transition == startTransition) continue;
                TransitionDef def = LogicManager.GetTransitionDef(transition);
                string roomName = def.sceneName;
                if (def.oneWay == 0 && rooms.Contains(roomName)) roomTransitions[roomName].Add(transition);
            }

            BuildSpanningTree(roomTransitions);
        }

        public static void BuildCARSpanningTree()
        {
            List<string> areas = new List<string>();
            Dictionary<string, List<string>> rooms = new Dictionary<string, List<string>>();
            foreach (string t in tm.unplacedTransitions)
            {
                if (t == startTransition) continue;
                if (!LogicManager.GetTransitionDef(t).isolated || !LogicManager.GetTransitionDef(t).deadEnd)
                {
                    if (!areas.Contains(LogicManager.GetTransitionDef(t).areaName))
                    {
                        areas.Add(LogicManager.GetTransitionDef(t).areaName);
                        rooms.Add(LogicManager.GetTransitionDef(t).areaName, new List<string>());
                    }


                    if (!rooms[LogicManager.GetTransitionDef(t).areaName].Contains(LogicManager.GetTransitionDef(t).sceneName))
                        rooms[LogicManager.GetTransitionDef(t).areaName].Add(LogicManager.GetTransitionDef(t).sceneName);
                }
            }

            var areaTransitions = new Dictionary<string, Dictionary<string, List<string>>>(); // [area][scene][transition]
            foreach (string area in areas) areaTransitions.Add(area, new Dictionary<string, List<string>>());
            foreach (var kvp in rooms) foreach (string room in kvp.Value) areaTransitions[kvp.Key].Add(room, new List<string>());
            foreach (string t in tm.unplacedTransitions)
            {
                if (t == startTransition) continue;
                TransitionDef def = LogicManager.GetTransitionDef(t);
                if (!areas.Contains(def.areaName) || !areaTransitions[def.areaName].ContainsKey(def.sceneName)) continue;
                areaTransitions[def.areaName][def.sceneName].Add(t);
            }
            foreach (string area in areas) BuildSpanningTree(areaTransitions[area]);
            var worldTransitions = new Dictionary<string, List<string>>();
            foreach (string area in areas)
            {
                worldTransitions.Add(area, new List<string>());
            }
            foreach (string t in tm.unplacedTransitions)
            {
                if (t == startTransition) continue;
                if (areas.Contains(LogicManager.GetTransitionDef(t).areaName) && rooms[LogicManager.GetTransitionDef(t).areaName].Contains(LogicManager.GetTransitionDef(t).sceneName))
                {
                    worldTransitions[LogicManager.GetTransitionDef(t).areaName].Add(t);
                }
            }
            BuildSpanningTree(worldTransitions);
        }

        public static void BuildSpanningTree(Dictionary<string, List<string>> sortedTransitions, string first = null)
        {
            List<string> remaining = sortedTransitions.Keys.ToList();
            while (first == null)
            {
                first = remaining[rand.Next(remaining.Count)];
                if (!sortedTransitions[first].Any(t => !LogicManager.GetTransitionDef(t).isolated)) first = null;
            }
            remaining.Remove(first);
            List<DirectedTransitions> directed = new List<DirectedTransitions>();
            directed.Add(new DirectedTransitions(rand));
            directed[0].Add(sortedTransitions[first].Where(t => !LogicManager.GetTransitionDef(t).isolated).ToList());
            int failsafe = 0;

            while (remaining.Any())
            {
                bool placed = false;
                failsafe++;
                if (failsafe > 500 || !directed[0].AnyCompatible())
                {
                    Log("Triggered failsafe on round " + failsafe + " in BuildSpanningTree, where first transition set was: " + first + " with count: " + sortedTransitions[first].Count);
                    randomizationError = true;
                    return;
                }

                string nextRoom = remaining[rand.Next(remaining.Count)];

                foreach (DirectedTransitions dt in directed)
                {
                    List<string> nextAreaTransitions = sortedTransitions[nextRoom].Where(transition => !LogicManager.GetTransitionDef(transition).deadEnd && dt.Test(transition)).ToList();
                    List<string> newTransitions = sortedTransitions[nextRoom].Where(transition => !LogicManager.GetTransitionDef(transition).isolated).ToList();

                    if (!nextAreaTransitions.Any())
                    {
                        continue;
                    }

                    string transitionTarget = nextAreaTransitions[rand.Next(nextAreaTransitions.Count)];
                    string transitionSource = dt.GetNextTransition(transitionTarget);

                    tm.PlaceTransitionPair(transitionSource, transitionTarget);
                    remaining.Remove(nextRoom);

                    dt.Add(newTransitions);
                    dt.Remove(transitionTarget, transitionSource);
                    placed = true;
                    break;
                }
                if (placed) continue;
                else
                {
                    DirectedTransitions dt = new DirectedTransitions(rand);
                    dt.Add(sortedTransitions[nextRoom].Where(transition => !LogicManager.GetTransitionDef(transition).isolated).ToList());
                    directed.Add(dt);
                    remaining.Remove(nextRoom);
                }
            }
            //Log("Completed first pass of BuildSpanningTree with " + directed.Count + " connected component(s).");
            for (int i = 0; i < directed.Count; i++)
            {
                DirectedTransitions dt = directed[i];
                DirectedTransitions dt1 = null;
                string transition1 = null;
                string transition2 = null;

                foreach (var dt2 in directed)
                {
                    if (dt == dt2) continue;

                    if (dt.left && dt2.right)
                    {
                        transition1 = dt.leftTransitions[rand.Next(dt.leftTransitions.Count)];
                        transition2 = dt2.rightTransitions[rand.Next(dt2.rightTransitions.Count)];
                        dt1 = dt2;
                        break;
                    }
                    else if (dt.right && dt2.left)
                    {
                        transition1 = dt.rightTransitions[rand.Next(dt.rightTransitions.Count)];
                        transition2 = dt2.leftTransitions[rand.Next(dt2.leftTransitions.Count)];
                        dt1 = dt2;
                        break;
                    }
                    else if (dt.top && dt2.bot)
                    {
                        transition1 = dt.topTransitions[rand.Next(dt.topTransitions.Count)];
                        transition2 = dt2.botTransitions[rand.Next(dt2.botTransitions.Count)];
                        dt1 = dt2;
                        break;
                    }
                    else if (dt.bot && dt2.top)
                    {
                        transition1 = dt.botTransitions[rand.Next(dt.botTransitions.Count)];
                        transition2 = dt2.topTransitions[rand.Next(dt2.topTransitions.Count)];
                        dt1 = dt2;
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(transition1))
                {
                    tm.PlaceTransitionPair(transition1, transition2);
                    dt1.Add(dt.AllTransitions);
                    dt1.Remove(transition1, transition2);
                    directed.Remove(dt);
                    i = -1;
                }
            }
            //Log("Exited BuildSpanningTree with " + directed.Count + " connected component(s).");
        }

    }
}
