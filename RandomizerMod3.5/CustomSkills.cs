using Modding;
using UnityEngine;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using GlobalEnums;
using RandomizerMod.Extensions;

namespace RandomizerMod
{
    static class CustomSkills
    {
        public static void Hook()
        {
            UnHook();
            ModHooks.GetPlayerBoolHook += SkillBoolGetOverride;
            ModHooks.SetPlayerBoolHook += SkillBoolSetOverride;
            On.PlayMakerFSM.OnEnable += ShowSkillsInInventory;
            On.PlayMakerFSM.OnEnable += DisableSwim;
            On.HeroController.CanFocus += DisableFocus;
            On.HeroController.CanDash += DisableDash;
            On.HeroController.CanAttack += DisableAttack;
        }

        public static void UnHook()
        {
            ModHooks.GetPlayerBoolHook -= SkillBoolGetOverride;
            ModHooks.SetPlayerBoolHook -= SkillBoolSetOverride;
            On.PlayMakerFSM.OnEnable -= ShowSkillsInInventory;
            On.PlayMakerFSM.OnEnable -= DisableSwim;
            On.HeroController.CanFocus -= DisableFocus;
            On.HeroController.CanDash -= DisableDash;
            On.HeroController.CanAttack -= DisableAttack;
        }


        private static bool SkillBoolGetOverride(string boolName, bool orig)
        {
            // bools for left and right cloak
            // canDash: Override here so they always have dash with exactly one direction, and disable it separately in the 
            // DisableDash function. If they have neither or both of the directions, we shouldn't do anything here to provide
            // minimal disruption for other mods
            if (boolName == "canDash")
            {
                return RandomizerMod.Instance.Settings.GetBool(name: "canDashLeft") && !RandomizerMod.Instance.Settings.GetBool(name: "canDashRight")
                    || RandomizerMod.Instance.Settings.GetBool(name: "canDashRight") && !RandomizerMod.Instance.Settings.GetBool(name: "canDashLeft")
                    || PlayerData.instance.GetBoolInternal("canDash");
            }
            // hasDashAny: dummy bool to check if we should be showing dash in the inventory
            if (boolName == "hasDashAny")
            {
                return RandomizerMod.Instance.Settings.GetBool(name: "canDashLeft") && !RandomizerMod.Instance.Settings.GetBool(name: "canDashRight")
                   || RandomizerMod.Instance.Settings.GetBool(name: "canDashRight") && !RandomizerMod.Instance.Settings.GetBool(name: "canDashLeft")
                   || PlayerData.instance.GetBoolInternal("hasDash");
            }

            // bools for left and right claw
            if (boolName == "hasWalljumpLeft" || boolName == "hasWalljumpRight")
            {
                return RandomizerMod.Instance.Settings.GetBool(name: boolName);
            }
            // We don't need to check if split claw is active, because this code should only execute if the player has exactly one
            // claw piece
            if (boolName == "hasWalljump")
            {
                // If the player has both claw pieces, they are considered to have claw so we don't need to do anything here. 
                // This way, if they have both claw pieces then we won't override the behaviour in case e.g. they disable claw with debug mod.
                if (RandomizerMod.Instance.Settings.GetBool(name: "hasWalljumpLeft")
                    && !RandomizerMod.Instance.Settings.GetBool(name: "hasWalljumpRight")
                    && HeroController.instance.touchingWallL)
                {
                    return true;
                }
                else if (RandomizerMod.Instance.Settings.GetBool(name: "hasWalljumpRight")
                    && !RandomizerMod.Instance.Settings.GetBool(name: "hasWalljumpLeft")
                    && HeroController.instance.touchingWallR)
                {
                    return true;
                }
            }
            // dummy bool to check if we should be showing the mantis claw in inventory
            if (boolName == "hasWalljumpAny")
            {
                return RandomizerMod.Instance.Settings.GetBool(name: "hasWalljumpLeft")
                    || RandomizerMod.Instance.Settings.GetBool(name: "hasWalljumpRight")
                    || PlayerData.instance.GetBoolInternal("hasWalljump");
            }

            return Ref.PD.GetBoolInternal(boolName);
        }

        private static bool SkillBoolSetOverride(string boolName, bool value)
        {
            // bools for left and right cloak
            if (boolName == "canDashLeft" || boolName == "canDashRight")
            {
                // Give the player shadowdash if they already have that dash direction
                if (RandomizerMod.Instance.Settings.GetBool(name: boolName) && value)
                {
                    Ref.PD.SetBool("hasShadowDash", true);
                }
                // Otherwise, let the player dash in that direction
                else
                {
                    RandomizerMod.Instance.Settings.SetBool(value, boolName);
                }
                if (RandomizerMod.Instance.Settings.GetBool(name: "canDashLeft") && RandomizerMod.Instance.Settings.GetBool(name: "canDashRight"))
                {
                    Ref.PD.SetBool("hasDash", true);
                }
            }

            // bools for left and right claw
            // If the player has one piece and gets the other, then we give them the full mantis claw. This allows the split claw to work with other mods more easily, 
            // unless of course they have only one piece.
            else if (boolName == "hasWalljumpLeft")
            {
                RandomizerMod.Instance.Settings.SetBool(value, boolName);
                if (value && RandomizerMod.Instance.Settings.GetBool(name: "hasWalljumpRight"))
                {
                    Ref.PD.SetBool("hasWalljump", true);
                }
            }
            else if (boolName == "hasWalljumpRight")
            {
                RandomizerMod.Instance.Settings.SetBool(value, boolName);
                if (value && RandomizerMod.Instance.Settings.GetBool(name: "hasWalljumpLeft"))
                {
                    Ref.PD.SetBool("hasWalljump", true);
                }
            }
            // Send the set through to the actual set
            return value;
        }

        private static void ShowSkillsInInventory(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            orig(self);

            if (self.FsmName == "Build Equipment List" && self.gameObject.name == "Equipment")
            {
                self.GetState("Walljump").GetActionOfType<PlayerDataBoolTest>().boolName.Value = "hasWalljumpAny";

                PlayerDataBoolTest[] dashChecks = self.GetState("Dash").GetActionsOfType<PlayerDataBoolTest>();
                dashChecks[0].boolName.Value = "hasDashAny";
            }
        }

        private static bool DisableFocus(On.HeroController.orig_CanFocus orig, HeroController self)
        {
            if (RandomizerMod.Instance.Settings.RandomizeFocus && !RandomizerMod.Instance.Settings.GetBool(name: "canFocus")) return false;
            else return orig(self);
        }

        private static bool DisableDash(On.HeroController.orig_CanDash orig, HeroController self)
        {
            // Only disable dash in a direction if they have it in the other direction. If they have both or neither dash
            // direction, then it will be handled by the original function.
            // We don't need to check if Split Cloak is active, because we only change the output if the player has exactly one cloak piece
            switch (GetDashDirection(self))
            {
                default:
                    return orig(self);
                case Direction.leftward:
                    return orig(self) && (!RandomizerMod.Instance.Settings.GetBool(name: "canDashRight") || RandomizerMod.Instance.Settings.GetBool(name: "canDashLeft"));
                case Direction.rightward:
                    return orig(self) && (!RandomizerMod.Instance.Settings.GetBool(name: "canDashLeft") || RandomizerMod.Instance.Settings.GetBool(name: "canDashRight"));
                case Direction.downward:
                    return orig(self);
            }
        }

        private static void DisableSwim(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            orig(self);

            if (RandomizerMod.Instance.Settings.RandomizeSwim && self.FsmName == "Surface Water Region")
            {
                if (self.gameObject.LocateFSM("Acid Armour Check") != null) return; // acid

                FsmState splash = self.GetState("Big Splash?");
                FsmStateAction acidDeath = new FsmStateActions.RandomizerExecuteLambda(() =>
                {

                    if (!RandomizerMod.Instance.Settings.GetBool(name: "canSwim"))
                    {
                        // this is actually the spike death despite the enum, because the acid death splashes green stuff
                        HeroController.instance.TakeDamage(self.gameObject, CollisionSide.other, 1, (int)HazardType.ACID);
                        PlayMakerFSM.BroadcastEvent("SWIM DEATH");
                    }
                    
                });

                splash.AddFirstAction(acidDeath);
                splash.AddTransition("SWIM DEATH", "Idle");
            }
        }

        private static Direction GetDashDirection(HeroController hc)
        {
            InputHandler input = Ref.Input;
            if (!hc.cState.onGround && input.inputActions.down.IsPressed && hc.playerData.GetBool("equippedCharm_31")
                    && !(input.inputActions.left.IsPressed || input.inputActions.right.IsPressed))
            {
                return Direction.downward;
            }
            if (hc.wallSlidingL) return Direction.rightward;
            else if (hc.wallSlidingR) return Direction.leftward;
            else if (input.inputActions.right.IsPressed) return Direction.rightward;
            else if (input.inputActions.left.IsPressed) return Direction.leftward;
            else if (hc.cState.facingRight) return Direction.rightward;
            else return Direction.leftward;
        }


        private static bool DisableAttack(On.HeroController.orig_CanAttack orig, HeroController self)
        {
            switch (GetAttackDirection(self))
            {
                default:
                    return orig(self);

                case Direction.upward:
                    return orig(self) && (RandomizerMod.Instance.Settings.GetBool(name: "canUpslash") || !RandomizerMod.Instance.Settings.CursedNail);
                case Direction.leftward:
                    return orig(self) && (RandomizerMod.Instance.Settings.GetBool(name: "canSideslashLeft") || !RandomizerMod.Instance.Settings.CursedNail);
                case Direction.rightward:
                    return orig(self) && (RandomizerMod.Instance.Settings.GetBool(name: "canSideslashRight") || !RandomizerMod.Instance.Settings.CursedNail);
                case Direction.downward:
                    return orig(self);
            }
        }
        // This function copies the code in HeroController.DoAttack to determine the attack direction, with an
        // additional check if the player is wallsliding (because we want to treat a wallslash as a normal slash)
        private static Direction GetAttackDirection(HeroController hc)
        {
            if (hc.wallSlidingL)
            {
                return Direction.rightward;
            }
            else if (hc.wallSlidingR)
            {
                return Direction.leftward;
            }

            if (hc.vertical_input > Mathf.Epsilon)
            {
                return Direction.upward;
            }
            else if (hc.vertical_input < -Mathf.Epsilon)
            {
                if (hc.hero_state != GlobalEnums.ActorStates.idle && hc.hero_state != GlobalEnums.ActorStates.running)
                {
                    return Direction.downward;
                }
                else
                {
                    return hc.cState.facingRight ? Direction.rightward : Direction.leftward;
                }
            }
            else
            {
                return hc.cState.facingRight ? Direction.rightward : Direction.leftward;
            }
        }

        private enum Direction
        {
            upward,
            leftward,
            rightward,
            downward
        }
    }
}
