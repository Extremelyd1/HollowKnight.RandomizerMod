using RandomizerMod.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RandomizerMod.Actions
{
    public class ReplaceObjectWithGrubJar : RandomizerAction
    {
        private readonly string _objectName;
        private readonly string _sceneName;
        private readonly string _jarName;
        private readonly string _item;
        private readonly string _location;
        private readonly float _elevation;
        private readonly bool _unrandomized;

        public ReplaceObjectWithGrubJar(string sceneName, string objectName, float elevation, string jarName, string item, string location, bool unrandomized = false)
        {
            _sceneName = sceneName;
            _objectName = objectName;
            _jarName = jarName;
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

            // Put a jar in the same location as the original
            GameObject jar = ObjectCache.GrubJar;
            jar.name = _jarName;
            if (obj.transform.parent != null)
            {
                jar.transform.SetParent(obj.transform.parent);
            }

            jar.transform.position = obj.transform.position;
            jar.transform.localPosition = obj.transform.localPosition;
            var pos = jar.transform.position;
            // Move the jar forward so it appears in front of any background objects
            jar.transform.position = new Vector3(pos.x, pos.y + CreateNewGrubJar.GRUB_JAR_ELEVATION - _elevation, pos.z - 0.1f);
            var grub = jar.transform.Find("Grub");
            grub.position = new Vector3(grub.position.x, grub.position.y, pos.z);
            jar.SetActive(obj.activeSelf);

            CreateNewGrubJar.FixBottleFSM(jar, _item, _location, _unrandomized);


            // Destroy the original
            Object.Destroy(obj);
        }
    }
}
