using System.Collections.Generic;
using HutongGames.PlayMaker.Actions;
using RandomizerMod.Extensions;
using RandomizerMod.FsmStateActions;
using UnityEngine;
using static RandomizerMod.GiveItemActions;

namespace RandomizerMod.Actions
{
    public class CreateNewSoulTotem : RandomizerAction
    {
        private readonly float _x;
        private readonly float _y;
        private readonly string _sceneName;
        private readonly string _totemName;
        private readonly string _item;
        private readonly string _location;
        private readonly SoulTotemSubtype _subtype;
        private readonly SoulTotemSubtype _intendedSubtype;

        public static Dictionary<SoulTotemSubtype, float> Elevation = new Dictionary<SoulTotemSubtype, float>() {
            [SoulTotemSubtype.A] = 0.5f,
            [SoulTotemSubtype.B] = -0.1f,
            [SoulTotemSubtype.C] = -0.1f,
            // Some elevation values adjusted from the originals to account for the shrinkage.
            [SoulTotemSubtype.D] = 1.3f - 0.5f,
            [SoulTotemSubtype.E] = 1.2f - 0.5f,
            [SoulTotemSubtype.F] = 0.8f,
            [SoulTotemSubtype.G] = 0.2f,
            [SoulTotemSubtype.Palace] = 1.3f - 0.3f,
            [SoulTotemSubtype.PathOfPain] = 1.5f - 0.9f,
        };

        public static Dictionary<SoulTotemSubtype, float> ShrinkageFactor = new Dictionary<SoulTotemSubtype, float>() {
            [SoulTotemSubtype.D] = 0.7f,
            [SoulTotemSubtype.E] = 0.7f,
            [SoulTotemSubtype.Palace] = 0.8f,
            [SoulTotemSubtype.PathOfPain] = 0.7f,
        };

        public static Dictionary<SoulTotemSubtype, int> HitCount = new Dictionary<SoulTotemSubtype, int>()
        {
            [SoulTotemSubtype.A] = 5,
            [SoulTotemSubtype.B] = 3,
            [SoulTotemSubtype.C] = 3,
            [SoulTotemSubtype.D] = 5,
            [SoulTotemSubtype.E] = 5,
            [SoulTotemSubtype.F] = 5,
            [SoulTotemSubtype.G] = 5,
            [SoulTotemSubtype.Palace] = 5,
            [SoulTotemSubtype.PathOfPain] = -1,
        };

        public CreateNewSoulTotem(string sceneName, float x, float y, string totemName, string item, string location, SoulTotemSubtype subtype, SoulTotemSubtype intendedSubtype)
        {
            _sceneName = sceneName;
            _x = x;
            _y = y;
            _totemName = totemName;
            _item = item;
            _location = location;
            _subtype = subtype;
            _intendedSubtype = intendedSubtype;
        }

        public override ActionType Type => ActionType.GameObject;

        public override void Process(string scene, Object changeObj)
        {
            if (scene != _sceneName)
            {
                return;
            }

            GameObject totem = ObjectCache.SoulTotem(_subtype);
            totem.name = _totemName;
            totem.transform.position = new Vector3(_x, _y, totem.transform.position.z);
            if (ShrinkageFactor.TryGetValue(_subtype, out var k))
            {
                var t = totem.transform;
                t.localScale = new Vector3(t.localScale.x * k, t.localScale.y * k, t.localScale.z);
            }
            totem.SetActive(true);
            SetSoul(totem, _item, _location, _intendedSubtype);
        }

        public static void SetSoul(GameObject totem, string item, string location, SoulTotemSubtype intendedSubtype)
        {
            int hitCount = HitCount[intendedSubtype];

            var fsm = FSMUtility.LocateFSM(totem, "soul_totem");
            var init = fsm.GetState("Init");
            var hit = fsm.GetState("Hit");

            if (hitCount == -1)
            {
                // PoP totems, or fake PoP totems, should be infinite
                init.RemoveTransitionsTo("Mesh Renderer Off");
                hit.RemoveTransitionsTo("Depleted");
                hit.RemoveActionsOfType<IntCompare>();
            }
            else
            {
                // Normal totems can use the PersistentIntItem component to store hits remaining etc
                PersistentIntData pid = totem.GetComponent<PersistentIntItem>().persistentIntData;
                pid.id = totem.name;
                pid.sceneName = totem.scene.name;

                fsm.FsmVariables.GetFsmInt("Value").Value = hitCount;
                fsm.GetState("Reset?").GetActionOfType<SetIntValue>().intValue.Value = hitCount;
                fsm.GetState("Reset").GetActionOfType<SetIntValue>().intValue.Value = hitCount;
            }

            hit.AddAction(new RandomizerExecuteLambda(() =>
            {
                if (!RandomizerMod.Instance.Settings.CheckLocationFound(location)) GiveItem(GiveAction.None, item, location);
            }));
        }
    }
}
