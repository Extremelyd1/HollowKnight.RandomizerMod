using RandomizerMod.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RandomizerMod.Actions
{
    public class ReplaceObjectWithSoulTotem : RandomizerAction
    {
        private readonly string _newTotemName;
        private readonly string _objectName;
        private readonly string _sceneName;
        private readonly float _elevation;
        private readonly SoulTotemSubtype _subtype;
        private readonly string _item;
        private readonly string _location;
        private readonly SoulTotemSubtype _intendedSubtype;

        public ReplaceObjectWithSoulTotem(string sceneName, string objectName, float elevation, string newTotemName, string item, string location, SoulTotemSubtype subtype, SoulTotemSubtype intendedSubtype)
        {
            _newTotemName = newTotemName;
            _objectName = objectName;
            _sceneName = sceneName;
            _elevation = elevation;
            _subtype = subtype;
            _item = item;
            _location = location;
            _intendedSubtype = intendedSubtype;
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

            GameObject totem = ObjectCache.SoulTotem(_subtype);
            totem.name = _newTotemName;
            if (obj.transform.parent != null)
            {
                totem.transform.SetParent(obj.transform.parent);
            }
            totem.transform.position = obj.transform.position;
            totem.transform.localPosition = obj.transform.localPosition;
            totem.transform.position += Vector3.up * (CreateNewSoulTotem.Elevation[_subtype] - _elevation);
            if (CreateNewSoulTotem.ShrinkageFactor.TryGetValue(_subtype, out var k))
            {
                var t = totem.transform;
                t.localScale = new Vector3(t.localScale.x * k, t.localScale.y * k, t.localScale.z);
            }
            totem.SetActive(obj.activeSelf);
            CreateNewSoulTotem.SetSoul(totem, _item, _location, _intendedSubtype);
            Object.Destroy(obj);
        }
    }
}
