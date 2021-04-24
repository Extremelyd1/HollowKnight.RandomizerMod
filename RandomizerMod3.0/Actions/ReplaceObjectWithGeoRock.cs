using System.Collections.Generic;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using SereCore;
using UnityEngine;
using UnityEngine.SceneManagement;
using RandomizerMod.FsmStateActions;
using static RandomizerMod.GiveItemActions;

namespace RandomizerMod.Actions
{
    public class ReplaceObjectWithGeoRock : RandomizerAction
    {
        private readonly string _objectName;
        private readonly string _sceneName;
        private readonly string _rockName;
        private readonly string _item;
        private readonly string _location;
        private readonly float _elevation;
        private readonly int _geo;
        private readonly GeoRockSubtype _subtype;

        public ReplaceObjectWithGeoRock(string sceneName, string objectName, float elevation, string rockName, string item, string location, int geo, GeoRockSubtype subtype)
        {
            _sceneName = sceneName;
            _objectName = objectName;
            _rockName = rockName;
            _item = item;
            _location = location;
            _elevation = elevation;
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

            Scene currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

            string[] objectHierarchy = _objectName.Split('\\');
            int i = 1;
            GameObject obj = currentScene.FindGameObject(objectHierarchy[0]);
            while (i < objectHierarchy.Length)
            {
                obj = obj.FindGameObjectInChildren(objectHierarchy[i++]);
            }

            if (obj == null) return;

            // Somehow, rocks placed at these locations would be placed far away
            // from their intended location if we spawn them the same way we
            // spawn them everywhere else. Oddly enough grubs do not have the same
            // problem.
            var isSpecialLocation = _location == "Shade_Cloak" || _location == "Grub-Watcher's_Spire";

            // Put a geo rock in the same location as the original
            GameObject rock = ObjectCache.GeoRock(_subtype);
            rock.name = _rockName;
            if (obj.transform.parent != null && !isSpecialLocation)
            {
                rock.transform.SetParent(obj.transform.parent);
            }

            rock.transform.position = obj.transform.position;
            if (!isSpecialLocation)
            {
                rock.transform.localPosition = obj.transform.localPosition;
            }
            rock.transform.position += Vector3.up * (CreateNewGeoRock.Elevation[_subtype] - _elevation);
            if (_subtype == GeoRockSubtype.Outskirts420) {
                var t = rock.transform;
                t.localScale = new Vector3(t.localScale.x * 0.5f, t.localScale.y * 0.5f, t.localScale.z);
            }

            rock.SetActive(obj.activeSelf);
            CreateNewGeoRock.SetGeo(rock, _item, _location, _geo);
            

            // Destroy the original
            Object.Destroy(obj);
        }
    }
}