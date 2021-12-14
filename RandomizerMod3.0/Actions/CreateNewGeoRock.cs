using System.Collections.Generic;
using HutongGames.PlayMaker.Actions;
using RandomizerMod.Extensions;
using RandomizerMod.FsmStateActions;
using UnityEngine;
using static RandomizerMod.GiveItemActions;

namespace RandomizerMod.Actions
{
    public class CreateNewGeoRock : RandomizerAction
    {
        private readonly float _x;
        private readonly float _y;
        private readonly string _sceneName;
        private readonly string _rockName;
        private readonly string _item;
        private readonly string _location;
        private readonly int _geo;
        private readonly GeoRockSubtype _subtype;

        public static Dictionary<GeoRockSubtype, float> Elevation = new Dictionary<GeoRockSubtype, float>() {
            [GeoRockSubtype.Default] = -0.8f,
            [GeoRockSubtype.Abyss] = -0.5f,
            [GeoRockSubtype.City] = 0,
            [GeoRockSubtype.Deepnest] = -0.6f,
            [GeoRockSubtype.Fung01] = -0.5f,
            [GeoRockSubtype.Fung02] = -0.5f,
            [GeoRockSubtype.Grave01] = 0.2f,
            [GeoRockSubtype.Grave02] = 0.2f,
            [GeoRockSubtype.GreenPath01] = -0.6f,
            [GeoRockSubtype.GreenPath02] = -0.7f,
            [GeoRockSubtype.Hive] = -0.2f,
            [GeoRockSubtype.Mine] = 0.1f,
            [GeoRockSubtype.Outskirts] = -0.8f,
            // Not the same as the elevation of the original rock because
            // we're shrinking it to half the size.
            [GeoRockSubtype.Outskirts420] = 0.3f
        };

        public CreateNewGeoRock(string sceneName, float x, float y, string rockName, string item, string location, int geo, GeoRockSubtype subtype)
        {
            _sceneName = sceneName;
            _x = x;
            _y = y;
            _rockName = rockName;
            _item = item;
            _location = location;
            _geo = geo;
            _subtype = subtype;
        }

        public override ActionType Type => ActionType.GameObject;

        public override void Process(string scene, Object changeObj)
        {
            if (scene != _sceneName)
            {
                return;
            }

            GameObject rock = ObjectCache.GeoRock(_subtype);
            rock.name = _rockName;
            rock.transform.position = new Vector3(_x, _y, rock.transform.position.z);
            if (_subtype == GeoRockSubtype.Outskirts420) {
                var t = rock.transform;
                t.localScale = new Vector3(t.localScale.x * 0.5f, t.localScale.y * 0.5f, t.localScale.z);
            }
            rock.SetActive(true);
            SetGeo(rock, _item, _location, _geo);
        }

        public static void SetGeo(GameObject rock, string item, string location, int geo) {
            var fsm = FSMUtility.LocateFSM(rock, "Geo Rock");
            var init = fsm.GetState("Initiate");
            init.RemoveActionsOfType<IntCompare>();
            init.AddAction(new RandomizerExecuteLambda(() => {
                fsm.SendEvent(RandomizerMod.Instance.Settings.CheckLocationFound(location) ? "BROKEN" : null);
            }));
            var hit = fsm.GetState("Hit");
            hit.ClearTransitions();
            hit.AddTransition("HIT", "Pause Frame");
            hit.AddTransition("FINISHED", "Pause Frame");
            hit.RemoveActionsOfType<FlingObjectsFromGlobalPool>();
            var payout = fsm.GetState("Destroy");
            var payoutAction = payout.GetActionOfType<FlingObjectsFromGlobalPool>();

            // If we're flinging 420 geo from a rock that isn't the 420 rock, the game doesn't like loading 420 items at once.
            if (geo == 420)
            {
                geo /= 5;
                payout.AddFirstAction(new RandomizerExecuteLambda(() => {
                    GameObject mediumPrefab = ObjectCache.MediumGeo;
                    Object.Destroy(mediumPrefab.Spawn());
                    mediumPrefab.SetActive(true);
                    payoutAction.gameObject.Value = mediumPrefab;
                }));
                payout.AddAction(new RandomizerExecuteLambda(() => payoutAction.gameObject.Value.SetActive(false)));
            }

            payoutAction.spawnMin.Value = geo;
            payoutAction.spawnMax.Value = geo;
            // Keep geo from flying into unreachable spots
            switch (location) {
                case "Thorns_of_Agony":
                case "Spore_Shroom":
                case "Hallownest_Seal-Fog_Canyon_East":
                    payoutAction.angleMin.Value = 90;
                    payoutAction.angleMax.Value = 90;
                    break;
            }
            payout.AddAction(new RandomizerExecuteLambda(() => GiveItem(GiveAction.None, item, location)));
        }
    }
}
