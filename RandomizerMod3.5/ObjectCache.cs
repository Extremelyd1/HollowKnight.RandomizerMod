using System.Collections.Generic;
using HutongGames.PlayMaker.Actions;
using Modding;
using RandomizerMod.Extensions;
using UnityEngine;
using static RandomizerMod.LogHelper;

namespace RandomizerMod
{
    internal static class ObjectCache
    {
        private static GameObject _shinyItem;

        private static GameObject _smallGeo;
        private static GameObject _mediumGeo;
        private static GameObject _largeGeo;

        private static GameObject _soul;

        private static GameObject _tinkEffect;

        private static GameObject _respawnMarker;

        private static GameObject _smallPlatform;

        private static GameObject _relicGetMsg;

        private static GameObject _grubJar;
        private static GameObject _mimicBottle;
        private static GameObject _mimicTop;
        private static GameObject _mimicDialogue;

        private static GameObject _loreTablet;

        private static GameObject _lumaflyEscape;

        private static Dictionary<GeoRockSubtype, GameObject> _geoRocks;

        private static Dictionary<SoulTotemSubtype, GameObject> _soulTotems;

        public static GameObject ShinyItem => Object.Instantiate(_shinyItem);

        public static GameObject SmallGeo => Object.Instantiate(_smallGeo);

        public static GameObject MediumGeo => Object.Instantiate(_mediumGeo);

        public static GameObject LargeGeo => Object.Instantiate(_largeGeo);

        public static GameObject Soul => Object.Instantiate(_soul);

        public static GameObject TinkEffect => Object.Instantiate(_tinkEffect);

        public static GameObject RespawnMarker => Object.Instantiate(_respawnMarker);

        public static GameObject SmallPlatform => Object.Instantiate(_smallPlatform);

        public static GameObject RelicGetMsg => Object.Instantiate(_relicGetMsg);

        public static GameObject GrubJar => Object.Instantiate(_grubJar);
        public static GameObject MimicBottle => Object.Instantiate(_mimicBottle);
        public static GameObject MimicTop => Object.Instantiate(_mimicTop);
        public static GameObject MimicDreamDialogue => Object.Instantiate(_mimicDialogue);

        public static GeoRockSubtype GetPreloadedRockType(GeoRockSubtype t) {
            return _geoRocks.ContainsKey(t) ? t : GeoRockSubtype.Default;
        }

        public static GameObject GeoRock(GeoRockSubtype t) {
            return Object.Instantiate(_geoRocks[t]);
        }

        public static SoulTotemSubtype GetPreloadedTotemType(SoulTotemSubtype t) {
            return _soulTotems.ContainsKey(t) ? t : SoulTotemSubtype.B;
        }

        public static GameObject SoulTotem(SoulTotemSubtype t) {
            return Object.Instantiate(_soulTotems[t]);
        }

        public static GameObject Grub;
        public static AudioClip[] GrubCry;
        public static AudioClip MimicScream;

        public static AudioClip LoreSound;

        public static AudioClip BigItemJingle;

        public static GameObject LumaflyEscape => Object.Instantiate(_lumaflyEscape);

        public static void GetPrefabs(Dictionary<string, Dictionary<string, GameObject>> objectsByScene)
        {
            _shinyItem = objectsByScene[SceneNames.Tutorial_01]["_Props/Chest/Item/Shiny Item (1)"];
            _shinyItem.name = "Randomizer Shiny";

            PlayMakerFSM shinyFSM = _shinyItem.LocateMyFSM("Shiny Control");
            _relicGetMsg = Object.Instantiate(shinyFSM.GetState("Trink Flash").GetActionsOfType<SpawnObjectFromGlobalPool>()[1].gameObject.Value);
            _relicGetMsg.SetActive(false);
            Object.DontDestroyOnLoad(_relicGetMsg);

            BigItemJingle = (AudioClip)(shinyFSM.GetState("Walljump")
                .GetActionOfType<CreateUIMsgGetItem>().gameObject.Value
                .LocateMyFSM("Msg Control")
                .GetState("Top Up")
                .GetActionOfType<AudioPlayerOneShotSingle>().audioClip.Value);
            Object.DontDestroyOnLoad(BigItemJingle);

            HealthManager health = objectsByScene[SceneNames.Tutorial_01]["_Enemies/Crawler 1"].GetComponent<HealthManager>();
            _smallGeo = Object.Instantiate(
                ReflectionHelper.GetField<HealthManager, GameObject>(health, "smallGeoPrefab"));
            _mediumGeo =
                Object.Instantiate(ReflectionHelper.GetField<HealthManager, GameObject>(health, "mediumGeoPrefab"));
            _largeGeo = Object.Instantiate(
                ReflectionHelper.GetField<HealthManager, GameObject>(health, "largeGeoPrefab"));

            _smallGeo.SetActive(false);
            _mediumGeo.SetActive(false);
            _largeGeo.SetActive(false);
            Object.DontDestroyOnLoad(_smallGeo);
            Object.DontDestroyOnLoad(_mediumGeo);
            Object.DontDestroyOnLoad(_largeGeo);

            PlayMakerFSM fsm  = objectsByScene[SceneNames.Deepnest_East_17]["Soul Totem mini_two_horned"].LocateMyFSM("soul_totem");
            _soul = Object.Instantiate(fsm.GetState("Hit").GetActionOfType<FlingObjectsFromGlobalPool>().gameObject.Value);
            _soul.SetActive(false);
            Object.DontDestroyOnLoad(_soul);

            _tinkEffect = Object.Instantiate(objectsByScene[SceneNames.Tutorial_01]["_Props/Cave Spikes (1)"].GetComponent<TinkEffect>().blockEffect);
            _tinkEffect.SetActive(false);
            Object.DontDestroyOnLoad(_tinkEffect);

            Object.Destroy(objectsByScene[SceneNames.Tutorial_01]["_Props/Cave Spikes (1)"]);
            Object.Destroy(objectsByScene[SceneNames.Tutorial_01]["_Enemies/Crawler 1"]);

            _respawnMarker = objectsByScene[SceneNames.Tutorial_01]["_Markers/Death Respawn Marker"];
            Object.DontDestroyOnLoad(_respawnMarker);

            _smallPlatform = objectsByScene[SceneNames.Tutorial_01]["_Scenery/plat_float_17"];
            Object.DontDestroyOnLoad(_smallPlatform);

            _grubJar = objectsByScene[SceneNames.Deepnest_36]["Grub Bottle"];
            Object.DontDestroyOnLoad(_grubJar);
            _mimicBottle = objectsByScene[SceneNames.Deepnest_36]["Grub Mimic Bottle"];
            Object.DontDestroyOnLoad(_mimicBottle);
            _mimicTop = objectsByScene[SceneNames.Deepnest_36]["Grub Mimic Top"];
            Object.DontDestroyOnLoad(_mimicTop);
            _mimicDialogue = objectsByScene[SceneNames.Deepnest_36]["Grub Bottle/Bottle Physical/Dream Dialogue"];
            Object.DontDestroyOnLoad(_mimicDialogue);

            if (RandomizerMod.Instance._globalSettings.ReduceRockPreloads)
            {
                _geoRocks = new Dictionary<GeoRockSubtype, GameObject>() {
                    [GeoRockSubtype.Default] = objectsByScene[SceneNames.Tutorial_01]["_Props/Geo Rock 1"],
                };
            }
            else
            {
                _geoRocks = new Dictionary<GeoRockSubtype, GameObject>() {
                    [GeoRockSubtype.Default] = objectsByScene[SceneNames.Tutorial_01]["_Props/Geo Rock 1"],
                    [GeoRockSubtype.Abyss] = objectsByScene[SceneNames.Abyss_19]["Geo Rock Abyss"],
                    [GeoRockSubtype.City] = objectsByScene[SceneNames.Ruins2_05]["Geo Rock City 1"],
                    [GeoRockSubtype.Deepnest] = objectsByScene[SceneNames.Deepnest_02]["Geo Rock Deepnest"],
                    [GeoRockSubtype.Fung01] = objectsByScene[SceneNames.Fungus2_11]["Geo Rock Fung 01"],
                    [GeoRockSubtype.Fung02] = objectsByScene[SceneNames.Fungus2_11]["Geo Rock Fung 02"],
                    [GeoRockSubtype.Grave01] = objectsByScene[SceneNames.RestingGrounds_10]["Geo Rock Grave 01"],
                    [GeoRockSubtype.Grave02] = objectsByScene[SceneNames.RestingGrounds_10]["Geo Rock Grave 02"],
                    [GeoRockSubtype.GreenPath01] = objectsByScene[SceneNames.Fungus1_12]["Geo Rock Green Path 01"],
                    [GeoRockSubtype.GreenPath02] = objectsByScene[SceneNames.Fungus1_12]["Geo Rock Green Path 02"],
                    [GeoRockSubtype.Hive] = objectsByScene[SceneNames.Hive_01]["Geo Rock Hive"],
                    [GeoRockSubtype.Mine] = objectsByScene[SceneNames.Mines_20]["Geo Rock Mine (4)"],
                    [GeoRockSubtype.Outskirts] = objectsByScene[SceneNames.Deepnest_East_17]["Geo Rock Outskirts"],
                    [GeoRockSubtype.Outskirts420] = objectsByScene[SceneNames.Deepnest_East_17]["Giant Geo Egg"]
                };
                
            }

            if (RandomizerMod.Instance._globalSettings.ReduceTotemPreloads)
            {
                _soulTotems = new Dictionary<SoulTotemSubtype, GameObject>() {
                    [SoulTotemSubtype.B] = objectsByScene[SceneNames.Deepnest_East_17]["Soul Totem mini_two_horned"]
                };
            }
            else
            {
                _soulTotems = new Dictionary<SoulTotemSubtype, GameObject>() {
                    [SoulTotemSubtype.A] = objectsByScene[SceneNames.Cliffs_02]["Soul Totem 5"],
                    [SoulTotemSubtype.B] = objectsByScene[SceneNames.Deepnest_East_17]["Soul Totem mini_two_horned"],
                    [SoulTotemSubtype.C] = objectsByScene[SceneNames.Abyss_04]["Soul Totem mini_horned"],
                    [SoulTotemSubtype.D] = objectsByScene[SceneNames.Deepnest_10]["Soul Totem 1"],
                    [SoulTotemSubtype.E] = objectsByScene[SceneNames.RestingGrounds_05]["Soul Totem 4"],
                    [SoulTotemSubtype.F] = objectsByScene[SceneNames.Crossroads_ShamanTemple]["Soul Totem 2"],
                    [SoulTotemSubtype.G] = objectsByScene[SceneNames.Ruins1_32]["Soul Totem 3"],
                    [SoulTotemSubtype.Palace] = objectsByScene[SceneNames.White_Palace_02]["Soul Totem white"],
                    [SoulTotemSubtype.PathOfPain] = objectsByScene[SceneNames.White_Palace_18]["Soul Totem white_Infinte"]
                };
            }
            
            foreach (var entry in _geoRocks) {
                Object.DontDestroyOnLoad(entry.Value);
            }

            Grub = objectsByScene[SceneNames.Deepnest_36]["Grub Bottle/Grub"];
            GrubCry = Grub.LocateMyFSM("Grub Control").GetState("Leave").GetActionOfType<AudioPlayRandom>().audioClips;
            Object.DontDestroyOnLoad(Grub);
            foreach (AudioClip clip in GrubCry)
            {
                Object.DontDestroyOnLoad(clip);
            }
            MimicScream = (AudioClip)_mimicTop.transform.Find("Grub Mimic 1").gameObject.LocateMyFSM("Grub Mimic").GetState("Scream")
                .GetActionOfType<AudioPlaySimple>().oneShotClip.Value;
            Object.DontDestroyOnLoad(MimicScream);

            _loreTablet = objectsByScene[SceneNames.Tutorial_01]["_Props/Tut_tablet_top"];
            LoreSound = (AudioClip)_loreTablet.LocateMyFSM("Inspection").GetState("Prompt Up").GetActionOfType<AudioPlayerOneShotSingle>().audioClip.Value;
            Object.DontDestroyOnLoad(LoreSound);

            // Preloaded the lumafly object from Deepnest_36 for efficiency, but we have to change the object's properties a bit to make it
            // resemble the Junk Pit object
            _lumaflyEscape = objectsByScene[SceneNames.Deepnest_36]["d_break_0047_deep_lamp2/lamp_bug_escape (7)"];
            {
                ParticleSystem.MainModule psm = _lumaflyEscape.GetComponent<ParticleSystem>().main;
                ParticleSystem.EmissionModule pse = _lumaflyEscape.GetComponent<ParticleSystem>().emission;
                ParticleSystem.ShapeModule pss = _lumaflyEscape.GetComponent<ParticleSystem>().shape;
                ParticleSystem.TextureSheetAnimationModule pst = _lumaflyEscape.GetComponent<ParticleSystem>().textureSheetAnimation;
                ParticleSystem.ForceOverLifetimeModule psf = _lumaflyEscape.GetComponent<ParticleSystem>().forceOverLifetime;

                psm.duration = 1f;
                psm.startLifetimeMultiplier = 4f;
                psm.startSizeMultiplier = 2f;
                psm.startSizeXMultiplier = 2f;
                psm.gravityModifier = -0.2f;
                psm.maxParticles = 99;              // In practice it only spawns 9 lumaflies
                pse.rateOverTimeMultiplier = 10f;
                pss.radius = 0.5868902f;
                pst.cycleCount = 15;
                psf.xMultiplier = 3;
                psf.yMultiplier = 8;

                // I have no idea what this is supposed to be lmao
                AnimationCurve yMax = new AnimationCurve(new Keyframe(0, 0.0810811371f), new Keyframe(0.230769232f, 0.108108163f),
                    new Keyframe(0.416873455f, -0.135135055f), new Keyframe(0.610421836f, -0.054053992f), new Keyframe(0.799007416f, -0.29729721f));
                AnimationCurve yMin = new AnimationCurve(new Keyframe(0, 0.486486584f), new Keyframe(0.220843673f, 0.567567647f),
                    new Keyframe(0.411910683f, 0.270270377f), new Keyframe(0.605459034f, 0.405405462f), new Keyframe(0.801488876f, 0.108108193f));
                psf.y = new ParticleSystem.MinMaxCurve(8, yMin, yMax);

                psf.x.curveMax.keys[0].value = -0.324324369f;
                psf.x.curveMax.keys[1].value = -0.432432413f;

                psf.x.curveMin.keys[0].value = 0.162162244f;
                psf.x.curveMin.keys[1].time = 0.159520522f;
                psf.x.curveMin.keys[1].value = 0.35135144f;

                Transform t = _lumaflyEscape.GetComponent<Transform>();
                Vector3 loc = t.localScale;
                loc.x = 1f;
                t.localScale = loc;
            }
            Object.DontDestroyOnLoad(_lumaflyEscape);


            if (_shinyItem == null || _smallGeo == null || _mediumGeo == null || _largeGeo == null ||
                _tinkEffect == null || _respawnMarker == null || _smallPlatform == null)
            {
                LogWarn("One or more ObjectCache items are null");
            }
        }
    }
}
