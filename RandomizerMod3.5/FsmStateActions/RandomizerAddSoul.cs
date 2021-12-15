using HutongGames.PlayMaker;
using UnityEngine;

namespace RandomizerMod.FsmStateActions
{
    internal class RandomizerAddSoul : FsmStateAction
    {
        private readonly GameObject _gameObject;

        public RandomizerAddSoul(GameObject baseObj)
        {
            _gameObject = baseObj;
        }

        public override void OnEnter()
        {
            GameObject soulPrefab = ObjectCache.Soul;

            // Workaround because Spawn extension is slightly broken
            Object.Destroy(soulPrefab.Spawn());

            soulPrefab.SetActive(true);

            FlingUtils.Config flingConfig = new FlingUtils.Config
            {
                Prefab = soulPrefab,
                AmountMin = 100,
                AmountMax = 101,
                SpeedMin = 10f,
                SpeedMax = 20f,
                AngleMin = 0f,
                AngleMax = 360f
            };

            FlingUtils.SpawnAndFling(flingConfig, _gameObject.transform, new Vector3(0f, 0f, 0f));

            soulPrefab.SetActive(false);

            Finish();
        }
    }
}
