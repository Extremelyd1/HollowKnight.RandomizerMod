using System;
using System.Collections.Generic;
using System.Linq;
using static RandomizerMod.LogHelper;

namespace RandomizerMod.Randomization
{
    class TransitionManager
    {
        public DirectedTransitions dt;
        public ProgressionManager pm;

        public static Dictionary<string, string> transitionPlacements;
        public static HashSet<string> recentProgression; // accessed by the progression manager whenever Add is called

        public List<string> unplacedTransitions;
        public Dictionary<string, string> standbyTransitions;
        public HashSet<string> reachableTransitions;
        private HashSet<string> vanillaProgression; // vanilla progression items, excluding stags. Randomized items are given in the randomizer.
        private HashSet<string> checkProgression; // vanilla progression items in areas/rooms we should be able to reach. This is what it actually iterates through to check logic on each round

        public List<string> availableTransitions => reachableTransitions.Intersect(unplacedTransitions).ToList();
        public List<string> placeableTransitions => availableTransitions.Where(t => dt.Test(t)).ToList();
        public int placeableCount => placeableTransitions.Count;

        private Random rand;

        public TransitionManager(Random rnd)
        {
            rand = rnd;
            dt = new DirectedTransitions(rnd);
            pm = new ProgressionManager(
                RandomizerState.InProgress
                );
            // start items added to pm in Connect Start to Graph in Randomizer

            transitionPlacements = new Dictionary<string, string>();

            List<string> iterate = LogicManager.TransitionNames().ToList();
            unplacedTransitions = new List<string>();
            while (iterate.Any())
            {
                string t = iterate[rand.Next(iterate.Count)];
                unplacedTransitions.Add(t);
                iterate.Remove(t);
            }

            standbyTransitions = new Dictionary<string, string>();
            reachableTransitions = new HashSet<string>();
            recentProgression = new HashSet<string>();
            vanillaProgression = VanillaManager.GetVanillaProgression();
            checkProgression = new HashSet<string>();

            dt.Add(unplacedTransitions);
        }

        public void ResetReachableTransitions()
        {
            reachableTransitions = new HashSet<string>();
        }
        public void UpdateReachableTransitions(string newThing = null, bool item = false, ProgressionManager _pm = null)
        {
            if (_pm != null) pm = _pm;
            if (newThing == null)
            {
                newThing = Randomizer.startTransition;
            }

            Queue<string> updates = new Queue<string>();

            if (!item) reachableTransitions.Add(newThing);
            pm.Add(newThing);
            updates.Enqueue(newThing);

            while (updates.Any())
            {
                string next = updates.Dequeue();
                if (transitionPlacements.TryGetValue(next, out string next2) && !reachableTransitions.Contains(next2))
                {
                    reachableTransitions.Add(next2);
                    pm.Add(next2);
                    updates.Enqueue(next2);
                }

                HashSet<string> potentialTransitions = LogicManager.GetTransitionsByProgression(recentProgression);

                // update possible vanilla locations
                HashSet<string> potentialLocations = LogicManager.GetLocationsByProgression(recentProgression);
                potentialLocations.IntersectWith(vanillaProgression);
                if (potentialLocations.Any())
                {
                    checkProgression.UnionWith(potentialLocations);
                    vanillaProgression.ExceptWith(checkProgression);
                }

                recentProgression = new HashSet<string>();



                foreach (string transition in potentialTransitions)
                {
                    if (!reachableTransitions.Contains(transition) && pm.CanGet(transition))
                    {
                        reachableTransitions.Add(transition);
                        pm.Add(transition);
                        updates.Enqueue(transition);
                        if (transitionPlacements.TryGetValue(transition, out string transition2))
                        {
                            reachableTransitions.Add(transition2);
                            pm.Add(transition2);
                            updates.Enqueue(transition2);
                        }
                    }
                }

                if (!updates.Any()) // vanilla locations are very unlikely to enter into logic, so we only check them right before the loop ends
                {
                    List<string> iterate = new List<string>();
                    foreach (string loc in checkProgression)
                    {
                        if (pm.CanGet(loc))
                        {
                            pm.Add(loc);
                            iterate.Add(loc); // **Don't**modify**collection**while**iterating**
                            updates.Enqueue(loc);
                        }
                    }
                    checkProgression.ExceptWith(iterate);
                }
            }
        }

        private HashSet<string> FakeUpdateReachableTransitions(string newThing = null, ProgressionManager _pm = null)
        {
            if (_pm != null) pm = _pm;
            if (newThing == null)
            {
                newThing = Randomizer.startTransition;
            }

            Queue<string> updates = new Queue<string>();
            HashSet<string> reachable = new HashSet<string>(reachableTransitions);

            reachable.Add(newThing);
            pm.AddTemp(newThing);
            updates.Enqueue(newThing);

            while (updates.Any())
            {
                string next = updates.Dequeue();
                foreach (string transition in LogicManager.GetTransitionsByProgression(recentProgression))
                {
                    if (!reachable.Contains(transition) && pm.CanGet(transition))
                    {
                        reachable.Add(transition);
                        pm.AddTemp(transition);
                        updates.Enqueue(transition);
                        if (transitionPlacements.TryGetValue(transition, out string transition2))
                        {
                            reachable.Add(transition2);
                            pm.AddTemp(transition2);
                            updates.Enqueue(transition2);
                        }
                    }
                }
                if (!updates.Any()) // check vanilla items last, because these almost never change
                {
                    foreach (string loc in LogicManager.GetLocationsByProgression(recentProgression))
                    {
                        if (!vanillaProgression.Contains(loc) && !checkProgression.Contains(loc)) continue;
                        if (!pm.Has(loc) && pm.CanGet(loc))
                        {
                            pm.AddTemp(loc);
                            updates.Enqueue(loc);
                        }
                    }
                }
            }
            reachable.ExceptWith(reachableTransitions);
            pm.RemoveTempItems();
            return reachable;
        }

        public void UpdateTransitionStandby(string transition1, string transition2)
        {
            bool ableToRelink1;
            bool ableToRelink2;

            bool T1InStandby = standbyTransitions.TryGetValue(transition1, out string oldTransition2);
            bool T2InStandby = standbyTransitions.TryGetValue(transition2, out string oldTransition1);
            standbyTransitions.Remove(transition1);
            standbyTransitions.Remove(transition2);

            if (T1InStandby && oldTransition1 == transition1) return; // this means t1 and t2 were already linked in standby

            if (T1InStandby || T2InStandby)
            {
                DirectedTransitions dt = new DirectedTransitions(rand);
                dt.Add(unplacedTransitions);
                if (T1InStandby && dt.GetNextTransition(oldTransition2) is string newTransition1)
                {
                    standbyTransitions[oldTransition2] = newTransition1;
                    standbyTransitions[newTransition1] = oldTransition2;
                    unplacedTransitions.Remove(newTransition1);
                    dt.Remove(newTransition1);
                    ableToRelink1 = true;
                }
                else ableToRelink1 = !T1InStandby;

                if (T2InStandby && dt.GetNextTransition(oldTransition1) is string newTransition2)
                {
                    standbyTransitions[oldTransition1] = newTransition2;
                    standbyTransitions[newTransition2] = oldTransition1;
                    unplacedTransitions.Remove(newTransition2);
                    ableToRelink2 = true;
                }
                else ableToRelink2 = !T2InStandby;

                if (T1InStandby && T2InStandby && !ableToRelink1 && !ableToRelink2)
                {
                    standbyTransitions[oldTransition1] = oldTransition2;
                    standbyTransitions[oldTransition2] = oldTransition1;
                    return;
                }

                if (!ableToRelink1 || !ableToRelink2)
                {
                    LogError("Error encountered in updating standby transitions. Unable to relink after removing standby transition.");
                    Randomizer.randomizationError = true;
                }
            }
        }

        public string NextTransition(DirectedTransitions _dt = null)
        {
            if (_dt == null) _dt = dt;
            return reachableTransitions.FirstOrDefault(t => _dt.Test(t) && unplacedTransitions.Contains(t));
        }

        public string ForceTransition(DirectedTransitions _dt = null)
        {
            if (_dt == null) _dt = dt;

            List<string> candidateTransitions = new List<string>();
            candidateTransitions.AddRange(unplacedTransitions);
            candidateTransitions.AddRange(standbyTransitions.Keys);
            candidateTransitions = candidateTransitions.Except(reachableTransitions).Where(transition => _dt.Test(transition)).ToList();
            bool Test(string transition)
            {
                HashSet<string> tempProgression = FakeUpdateReachableTransitions(transition);
                tempProgression.Remove(transition);
                tempProgression.IntersectWith(candidateTransitions);
                return tempProgression.Any();
            }
            return candidateTransitions.FirstOrDefault(t => Test(t));
        }

        public void PlaceTransitionPair(string transition1, string transition2)
        {
            transitionPlacements.Add(transition1, transition2);
            transitionPlacements.Add(transition2, transition1);
            unplacedTransitions.Remove(transition1);
            unplacedTransitions.Remove(transition2);
            dt.Remove(transition1, transition2);
            pm.Add(transition1);
            pm.Add(transition2);
            UpdateReachableTransitions(transition1);
            UpdateReachableTransitions(transition2);
            UpdateTransitionStandby(transition1, transition2);
        }

        public void PlaceOneWayPair(string entrance, string exit)
        {
            transitionPlacements.Add(entrance, exit);
            unplacedTransitions.Remove(entrance);
            unplacedTransitions.Remove(exit);
            dt.Remove(entrance, exit);
        }

        public void PlaceStandbyPair(string transition1, string transition2)
        {
            standbyTransitions.Add(transition1, transition2);
            standbyTransitions.Add(transition2, transition1);
            unplacedTransitions.Remove(transition1);
            unplacedTransitions.Remove(transition2);
            dt.Remove(transition1, transition2);
        }

        public void UnloadReachableStandby()
        {
            Queue<(string, string)> placePairs = new Queue<(string, string)>();
            foreach (string transition1 in reachableTransitions)
            {
                if (!standbyTransitions.TryGetValue(transition1, out string transition2)) continue;
                standbyTransitions.Remove(transition1);
                standbyTransitions.Remove(transition2);
                placePairs.Enqueue((transition1, transition2));
            }
            while (placePairs.Any())
            {
                (string, string) pair = placePairs.Dequeue();
                PlaceTransitionPair(pair.Item1, pair.Item2);
            }
        }

        public void UnloadStandby()
        {
            foreach (KeyValuePair<string, string> kvp in standbyTransitions)
            {
                //Log($"Reserve place: {kvp.Key}, {kvp.Value}");
                transitionPlacements.Add(kvp.Key, kvp.Value);
            }
        }
    }
}
