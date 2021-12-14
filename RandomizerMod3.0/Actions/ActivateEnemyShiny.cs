using UnityEngine;

namespace RandomizerMod.Actions
{
    public class ActivateEnemyShiny : RandomizerAction
    {
        private readonly string _sceneName;
        private readonly string _enemyName;
        private readonly string _shinyParentName;

        public ActivateEnemyShiny(string sceneName, string enemyName, string shinyParentName)
        {
            _sceneName = sceneName;
            _enemyName = enemyName;
            _shinyParentName = shinyParentName;
        }

        public override ActionType Type => ActionType.EnemyDeath;

        public override void Process(string scene, Object changeObj)
        {
            if (scene != _sceneName || !(changeObj is HealthManager hm) || hm.gameObject.name != _enemyName
                || !(GameObject.Find(_shinyParentName) is GameObject parent))
            {
                return;
            }

            hm.SetGeoLarge(0);
            hm.SetGeoMedium(0);
            hm.SetGeoSmall(0);

            foreach (Transform t in parent.transform)
            {
                t.SetPosition2D(hm.gameObject.transform.position);
                t.gameObject.SetActive(true);
            }

            if (parent.name.Contains("Randomizer Shiny"))
            {
                parent.transform.DetachChildren();
                Object.Destroy(parent);
            }
        }
    }
}
