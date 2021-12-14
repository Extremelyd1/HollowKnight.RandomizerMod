using GlobalEnums;
using HutongGames.PlayMaker.Actions;
using Modding;
using RandomizerMod.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

namespace RandomizerMod.SceneChanges
{
    internal static partial class SceneEditor
    {
        private static readonly Random Rnd = new Random();
        private static int _rndNum;

        public static void Hook()
        {
            UnHook();

            ModHooks.ObjectPoolSpawnHook += FixExplosionPogo;
            On.EnemyHitEffectsArmoured.RecieveHitEffect += FalseKnightNoises;
            On.PlayMakerFSM.OnEnable += FsmSceneEdits;
            // ModHooks.Instance.OnEnableEnemyHook += BossRewardReplacement.ReplaceBossRewards;
            On.PlayMakerFSM.OnEnable += ModifyFSM;
            On.GameManager.SaveLevelState += SavePersistentBoolItems;
        }

        public static void UnHook()
        {
            ModHooks.ObjectPoolSpawnHook -= FixExplosionPogo;
            On.EnemyHitEffectsArmoured.RecieveHitEffect -= FalseKnightNoises;
            On.PlayMakerFSM.OnEnable -= FsmSceneEdits;
            // ModHooks.Instance.OnEnableEnemyHook -= BossRewardReplacement.ReplaceBossRewards;
            On.PlayMakerFSM.OnEnable -= ModifyFSM;
            On.GameManager.SaveLevelState -= SavePersistentBoolItems;
        }

        public static void SceneChanged(Scene newScene)
        {
            RecalculateRandom();

            // Critical changes for randomizer functionality
            {
                ApplyRandomizerChanges(newScene);
                BreakDiveFloors(newScene);
                ExtraPlatforms(newScene);
                // EditStagStations(newScene);
                EditCorniferAndIselda(newScene);
                DestroyLoreTablets(newScene);
                DeleteCollectorGrubs(newScene);
                DestroyMimicObjects(newScene);
            }

            // Transition fixes: critical changes for transition randomizer functionality, protected by bool checks so they can also be used for item randomizer if necessary
            // Control fixes: npc quests that could otherwise be broken with out-of-sequence rooms
            {
                ApplyTransitionFixes(newScene);
                ApplyControlFixes(newScene);
            }

            // QoL Fixes - hints of all types, lemm sell all, fast grubfather, fast dream nail cutscene, etc
            {
                MiscQoLChanges(newScene);
                ApplyHintChanges(newScene);
                Jiji.JijiSceneEdits(newScene);
                AddWaterSpawns(newScene);
            }

            // Mainly restores pogos, etc., that were removed by TC
            {
                FixMiscSkips(newScene);
            }

            {
                DreamPlantEdits.ReplaceDreamPlantOrbs(newScene);
            }

            // make sure log is regularly updated with game info
            // do not destroy helper log on game end or quitout
            if (newScene.name != SceneNames.Menu_Title)
            {
                RandoLogger.UpdateHelperLog();
            }
        }

        // For the time being, let's keep some fsm scene edits here rather than in the SceneChanged function.
        // This way, they work even when the previous scene is a boss scene.
        public static void FsmSceneEdits(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            EditStagStations(self);
            DisableInfectedCrossroads(self);
            // BossRewardReplacement.DestroyGruzmomGeo(self);
            Jiji.KillTuk(self);

            orig(self);
        }



        // Everything below here is ancient, and can't really be moved elsewhere

        // No idea
        private static void RecalculateRandom()
        {
            _rndNum = Rnd.Next(25);
        }

        // No idea
        private static void FalseKnightNoises(On.EnemyHitEffectsArmoured.orig_RecieveHitEffect orig,
            EnemyHitEffectsArmoured self, float dir)
        {
            orig(self, dir);

            if (_rndNum != 17 || self.gameObject.name != "False Knight New")
            {
                return;
            }

            AudioPlayerOneShot hitPlayer = FSMUtility.LocateFSM(self.gameObject, "FalseyControl").GetState("Hit")
                .GetActionsOfType<AudioPlayerOneShot>()[0];
            AudioClip clip = hitPlayer.audioClips[Rnd.Next(hitPlayer.audioClips.Length)];

            AudioClip temp = self.enemyDamage.Clip;
            self.enemyDamage.Clip = clip;
            self.enemyDamage.SpawnAndPlayOneShot(self.audioPlayerPrefab, self.transform.position);
            self.enemyDamage.Clip = temp;
        }

        // Has side effect of making some things pogoable which originally were not; i.e. enemy death explosions
        private static GameObject FixExplosionPogo(GameObject go)
        {
            if (!go.name.StartsWith("Gas Explosion Recycle M"))
            {
                return go;
            }

            go.layer = (int) PhysLayers.ENEMIES;
            NonBouncer noFun = go.GetComponent<NonBouncer>();
            if (noFun)
            {
                noFun.active = false;
            }

            return go;
        }

        // Seems to remove Zote death triggers and also affect dream nail storage? Not entirely sure.
        private static void ModifyFSM(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            if (self.Fsm.FsmComponent.FsmName == "Check Zote Death")
            {
                Object.Destroy(self);
                return;
            }

            orig(self);

            if (self.gameObject.name != "Knight" || self.FsmName != "Dream Nail")
            {
                return;
            }

            self.GetState("Cancelable").GetActionsOfType<ListenForDreamNail>()[0].activeBool = true;
            self.GetState("Cancelable Dash").GetActionsOfType<ListenForDreamNail>()[0].activeBool = true;
            self.GetState("Queuing").GetActionsOfType<ListenForDreamNail>()[0].activeBool = true;
            self.GetState("Queuing").RemoveActionsOfType<BoolTest>();
        }
    }
}
