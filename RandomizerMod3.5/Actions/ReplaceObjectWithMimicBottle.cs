using RandomizerMod.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RandomizerMod.Actions
{
    public class ReplaceObjectWithMimicBottle : RandomizerAction
    {
        private readonly string _objectName;
        private readonly string _sceneName;
        private readonly string _bottleName;
        private readonly string _item;
        private readonly string _location;
        private readonly float _elevation;
        private readonly bool _unrandomized;

        public ReplaceObjectWithMimicBottle(string sceneName, string objectName, float elevation, string bottleName, string item, string location, bool unrandomized = false)
        {
            _sceneName = sceneName;
            _objectName = objectName;
            _bottleName = bottleName;
            _item = item;
            _location = location;
            _elevation = elevation;
            _unrandomized = unrandomized;
        }

        public override ActionType Type => ActionType.GameObject;

        public override void Process(string scene, Object changeObj)
        {
            if (scene != _sceneName)
            {
                return;
            }

            Scene currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

            string[] objectHierarchy = _objectName.Split('\\');
            int i = 1;
            GameObject obj = currentScene.FindGameObject(objectHierarchy[0]);
            while (i < objectHierarchy.Length)
            {
                obj = obj.FindGameObjectInChildren(objectHierarchy[i++]);
            }

            if (obj == null) return;

            // The easiest thing to do is to simply place the bottle and top where the object is
            float x = obj.transform.position.x;
            float y = obj.transform.position.y + CreateNewMimicBottle.MIMIC_BOTTLE_ELEVATION - _elevation;


            // Put a mimic in the same location as the original
            GameObject mimicBottle = ObjectCache.MimicBottle;
            mimicBottle.name = _bottleName + " Bottle";

            // Move the bottle forward so it appears in front of any background objects
            mimicBottle.transform.position = new Vector3(x, y, mimicBottle.transform.position.z - 0.1f);

            GameObject mimicTop = ObjectCache.MimicTop;
            mimicTop.name = _bottleName + " Top";
            mimicTop.transform.position = new Vector3(x, y - 0.2f, mimicTop.transform.position.z - 0.1f);

            GameObject mimic = mimicTop.FindGameObjectInChildren("Grub Mimic 1");
            mimic.name = _bottleName + " Mimic";
            Vector3 pos = mimic.transform.position;
            mimic.transform.position = new Vector3(pos.x, pos.y, mimicBottle.transform.position.z + 0.1f);

            GameObject mimicDialogue = ObjectCache.MimicDreamDialogue;
            mimicDialogue.name = _bottleName + " Dialogue";
            mimicDialogue.transform.SetPosition2D(mimicTop.transform.position);

            CreateNewMimicBottle.FixMimicFSMs(mimicBottle, mimicTop, mimic, mimicDialogue, _item, _location, _sceneName, _unrandomized);

            mimicBottle.SetActive(obj.activeSelf);
            mimicTop.SetActive(obj.activeSelf);

            // Destroy the original
            Object.Destroy(obj);
        }
    }
}
