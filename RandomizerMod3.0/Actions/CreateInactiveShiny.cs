using System;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using RandomizerMod.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RandomizerMod.Actions
{
    internal class CreateInactiveShiny : RandomizerAction
    {
        private readonly string _newShinyName;
        private readonly string _sceneName;
        private readonly string _parent;
        private readonly float _x;
        private readonly float _y;
        private readonly Func<bool> _activeCheck;    // If this returns true, set the shiny to be active at (x, y).

        public CreateInactiveShiny(string sceneName, string parent, string newShinyName, float x, float y, Func<bool> activeCheck)
        {
            _sceneName = sceneName;
            _newShinyName = newShinyName;
            _parent = parent;
            _x = x;
            _y = y;
            _activeCheck = activeCheck;
        }

        public CreateInactiveShiny(string sceneName, string parent, string newShinyName, float x, float y, string pdBool)
        {
            _sceneName = sceneName;
            _newShinyName = newShinyName;
            _parent = parent;
            _x = x;
            _y = y;
            _activeCheck = () => Ref.PD.GetBool(pdBool);
        }

        public CreateInactiveShiny(string sceneName, string parent, string newShinyName, float x, float y, string boolDataScene, string boolDataId)
        {
            _sceneName = sceneName;
            _newShinyName = newShinyName;
            _parent = parent;
            _x = x;
            _y = y;
            _activeCheck = () =>
            {
                PersistentBoolData boolData = new PersistentBoolData() { id = boolDataId, sceneName = boolDataScene };
                boolData = SceneData.instance.FindMyState(boolData) ?? boolData;
                return boolData.activated && !boolData.semiPersistent;
            };
        }


        public override ActionType Type => ActionType.GameObject;

        public override void Process(string scene, Object changeObj)
        {
            if (scene != _sceneName)
            {
                return;
            }

            GameObject shiny = ObjectCache.ShinyItem;
            shiny.name = _newShinyName;

            if (_activeCheck.Invoke())
            {
                // Simply create the shiny to be active, as in CreateNewShiny

                shiny.transform.position = new Vector3(_x, _y, shiny.transform.position.z);

                shiny.SetActive(true);

                // Force the new shiny to fall straight downwards
                PlayMakerFSM fsm = FSMUtility.LocateFSM(shiny, "Shiny Control");
                FsmState fling = fsm.GetState("Fling?");
                fling.ClearTransitions();
                fling.AddTransition("FINISHED", "Fling R");
                FlingObject flingObj = fsm.GetState("Fling R").GetActionsOfType<FlingObject>()[0];
                flingObj.angleMin = flingObj.angleMax = 270;

                // For some reason not setting speed manually messes with the object position
                flingObj.speedMin = flingObj.speedMax = 0.1f;
            }
            else
            {
                // Inactive shiny needs a parent so we can find it later, so create a dummy object if necessary

                string parentName = string.IsNullOrEmpty(_parent) ? _newShinyName + " Parent" : _parent;

                if (GameObject.Find(parentName) is GameObject go)
                {
                    shiny.transform.SetParent(go.transform);
                    shiny.transform.localPosition = new Vector3(0, 0, shiny.transform.position.z);
                }
                else
                {
                    GameObject go2 = new GameObject() { name = parentName };
                    go2.SetActive(true);
                    shiny.transform.SetParent(go2.transform);

                    shiny.SetActive(false);
                }
            }

        }
    }
}
