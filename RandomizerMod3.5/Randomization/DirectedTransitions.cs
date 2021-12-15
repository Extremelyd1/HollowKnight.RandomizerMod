using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomizerMod.Randomization
{
    class DirectedTransitions
    {
        public List<string> leftTransitions;
        public List<string> rightTransitions;
        public List<string> topTransitions;
        public List<string> botTransitions;
        private Random rand;

        public bool left => leftTransitions.Any();
        public bool right => rightTransitions.Any();
        public bool top => topTransitions.Any();
        public bool bot => botTransitions.Any();

        public List<string> AllTransitions => leftTransitions.Union(rightTransitions.Union(topTransitions.Union(botTransitions))).ToList();

        public DirectedTransitions(Random rnd)
        {
            leftTransitions = new List<string>();
            rightTransitions = new List<string>();
            topTransitions = new List<string>();
            botTransitions = new List<string>();
            rand = rnd;
        }

        public void Add(List<string> newTransitions)
        {
            leftTransitions.AddRange(newTransitions.Where(transition => LogicManager.GetTransitionDef(transition).doorName.StartsWith("left")));
            rightTransitions.AddRange(newTransitions.Where(transition => LogicManager.GetTransitionDef(transition).doorName.StartsWith("right") || LogicManager.GetTransitionDef(transition).doorName.StartsWith("door")));
            topTransitions.AddRange(newTransitions.Where(transition => LogicManager.GetTransitionDef(transition).doorName.StartsWith("top")));
            botTransitions.AddRange(newTransitions.Where(transition => LogicManager.GetTransitionDef(transition).doorName.StartsWith("bot")));
        }
        public void Add(params string[] newTransitions)
        {
            foreach(string t in newTransitions)
            {
                string doorName = LogicManager.GetTransitionDef(t).doorName;
                switch (doorName.Substring(0, 3))
                {
                    case "doo":
                    case "rig":
                        rightTransitions.Add(t);
                        break;
                    case "lef":
                        leftTransitions.Add(t);
                        break;
                    case "top":
                        topTransitions.Add(t);
                        break;
                    case "bot":
                        botTransitions.Add(t);
                        break;
                }
            }
        }

        public void Remove(params string[] transitions)
        {
            foreach (string transition in transitions)
            {
                leftTransitions.Remove(transition);
                rightTransitions.Remove(transition);
                topTransitions.Remove(transition);
                botTransitions.Remove(transition);
            }
        }
        public bool Test(string transitionTarget)
        {
            if (SinglyCompatible()) return true;
            string doorName = LogicManager.GetTransitionDef(transitionTarget).doorName;

            switch (doorName.Substring(0, 3))
            {
                case "doo":
                case "rig":
                    if (left) return true;
                    break;
                case "lef":
                    if (right) return true;
                    break;
                case "top":
                    if (bot) return true;
                    break;
                case "bot":
                    if (top) return true;
                    break;
            }
            return false;
        }
        public string GetNextTransition(string input, bool favorSameArea = false)
        {
            string doorName = LogicManager.GetTransitionDef(input).doorName;
            string output = null;

            if (favorSameArea)
            {
                string area = LogicManager.GetTransitionDef(input).areaName;
                List<string> compatibleTransitions = new List<string>();
                switch (doorName.Substring(0, 3))
                {
                    case "doo":
                    case "rig":
                        compatibleTransitions = leftTransitions.Where(t => LogicManager.GetTransitionDef(t).areaName == area).ToList();
                        break;
                    case "lef":
                        compatibleTransitions = rightTransitions.Where(t => LogicManager.GetTransitionDef(t).areaName == area).ToList();
                        break;
                    case "top":
                        compatibleTransitions = botTransitions.Where(t => LogicManager.GetTransitionDef(t).areaName == area).ToList();
                        break;
                    case "bot":
                        compatibleTransitions = topTransitions.Where(t => LogicManager.GetTransitionDef(t).areaName == area).ToList();
                        break;
                }
                if (compatibleTransitions.Any()) output = compatibleTransitions[rand.Next(compatibleTransitions.Count)];
            }

            if (string.IsNullOrEmpty(output))
            {
                switch (doorName.Substring(0, 3))
                {
                    case "doo":
                    case "rig":
                        if (leftTransitions.Any()) output = leftTransitions[rand.Next(leftTransitions.Count)];
                        break;
                    case "lef":
                        if (rightTransitions.Any()) output = rightTransitions[rand.Next(rightTransitions.Count)];
                        break;
                    case "top":
                        if (botTransitions.Any()) output = botTransitions[rand.Next(botTransitions.Count)];
                        break;
                    case "bot":
                        if (topTransitions.Any()) output = topTransitions[rand.Next(topTransitions.Count)];
                        break;
                }
            }
            
            return output;
        }

        public bool AnyCompatible()
        {
            return left || right || top || bot;
        }

        public bool SinglyCompatible()
        {
            return leftTransitions.Count > 0 && rightTransitions.Count > 0 && topTransitions.Count > 0 && botTransitions.Count > 0;
        }

        public bool DoublyCompatible()
        {
            return leftTransitions.Count > 1 && rightTransitions.Count > 1 && topTransitions.Count > 1 && botTransitions.Count > 1;
        }

        public void LogCounts()
        {
            int left1 = leftTransitions.Where(t => LogicManager.GetTransitionDef(t).oneWay != 0).Count();
            int right1 = rightTransitions.Where(t => LogicManager.GetTransitionDef(t).oneWay != 0).Count();
            int top1 = topTransitions.Where(t => LogicManager.GetTransitionDef(t).oneWay != 0).Count();
            int bot1 = botTransitions.Where(t => LogicManager.GetTransitionDef(t).oneWay != 0).Count();

            int left2 = leftTransitions.Where(t => LogicManager.GetTransitionDef(t).oneWay == 0).Count();
            int right2 = rightTransitions.Where(t => LogicManager.GetTransitionDef(t).oneWay == 0).Count();
            int top2 = topTransitions.Where(t => LogicManager.GetTransitionDef(t).oneWay == 0).Count();
            int bot2 = botTransitions.Where(t => LogicManager.GetTransitionDef(t).oneWay == 0).Count();

            if (0 != left1 || 0 != right1 || 0 != top1 || bot1 != 0)
            {
                LogHelper.Log("One-way counts:");
                LogHelper.Log("Left: " + left1);
                LogHelper.Log("Right: " + right1);
                LogHelper.Log("Top: " + top1);
                LogHelper.Log("Bottom: " + bot1);
            }
            LogHelper.Log("Two-way counts:");
            LogHelper.Log("Left: " + left2);
            LogHelper.Log("Right: " + right2);
            LogHelper.Log("Top: " + top2);
            LogHelper.Log("Bottom: " + bot2);
        }
    }
}
