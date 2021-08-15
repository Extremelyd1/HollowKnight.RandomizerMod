﻿using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using SereCore;
using UnityEngine;
using RandomizerMod.FsmStateActions;
using static RandomizerMod.GiveItemActions;

namespace RandomizerMod.Actions
{
    internal class CreateNewGrubJar : RandomizerAction
    {
        public const float GRUB_JAR_ELEVATION = 0.1f;

        private readonly string _newGrubJarName;
        private readonly string _sceneName;
        private readonly float _x;
        private readonly float _y;
        private readonly string _item;
        private readonly string _location;

        public CreateNewGrubJar(string sceneName, float x, float y, string newGrubJarName, string item, string location)
        {
            _sceneName = sceneName;
            _x = x;
            _y = y;
            _newGrubJarName = newGrubJarName;
            _item = item;
            _location = location;
        }

        public override ActionType Type => ActionType.GameObject;

        public override void Process(string scene, Object changeObj)
        {
            if (scene != _sceneName)
            {
                return;
            }

            GameObject GrubJar = ObjectCache.GrubJar;
            GrubJar.name = _newGrubJarName;

            var z = GrubJar.transform.position.z;
            // Move the jar forward so it appears in front of any background objects
            GrubJar.transform.position = new Vector3(_x, _y, z - 0.1f);
            var grub = GrubJar.transform.Find("Grub");
            grub.position = new Vector3(grub.position.x, grub.position.y, z);

            FixBottleFSM(GrubJar, _item, _location);
            
            GrubJar.SetActive(true);
        }

        public static void FixBottleFSM(GameObject jar, string item, string location)
        {
            PersistentBoolData pbd = jar.GetComponent<PersistentBoolItem>().persistentBoolData;
            pbd.id = location;
            pbd.sceneName = jar.scene.name;

            PlayMakerFSM fsm = FSMUtility.LocateFSM(jar, "Bottle Control");
            FsmState init = fsm.GetState("Init");

            // It's easiest to simply use the sceneData check to decide whether to destroy the bottle
            // init.RemoveActionsOfType<BoolTest>();
            // init.AddFirstAction(new RandomizerExecuteLambda(() => fsm.SendEvent(RandomizerMod.Instance.Settings.CheckLocationFound(location) ? "ACTIVATE" : null)));

            // The bottle FSM already takes care of granting the grub and playing happy grub noises
            // We have to add the GiveItem action before incrementing the grub count so the RecentItems
            // correctly notes the grub index
            fsm.GetState("Shatter").AddFirstAction(new RandomizerExecuteLambda(() => GiveItem(GiveAction.None, item, location)));

            // It seems pointless to mark this scene as having been cleared of grub jars
            fsm.GetState("Set Map?").ClearTransitions();
        }
    }
}
