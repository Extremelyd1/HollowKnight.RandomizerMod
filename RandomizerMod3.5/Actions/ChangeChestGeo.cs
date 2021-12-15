using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using RandomizerMod.Extensions;
using RandomizerMod.FsmStateActions;
using UnityEngine;

namespace RandomizerMod.Actions
{
    public class ChangeChestGeo : RandomizerAction
    {
        private readonly string _fsmName;
        private readonly int _geoAmount;
        private readonly string _objectName;

        private readonly string _sceneName;
        private readonly string _item;
        private readonly string _location;

        public ChangeChestGeo(string sceneName, string objectName, string fsmName, int geoAmount, string item, string location)
        {
            _sceneName = sceneName;
            _objectName = objectName;
            _fsmName = fsmName;
            _geoAmount = geoAmount;
            _item = item;
            _location = location;
        }

        public override ActionType Type => ActionType.PlayMakerFSM;

        public override void Process(string scene, Object changeObj)
        {
            if (scene != _sceneName || !(changeObj is PlayMakerFSM fsm) || fsm.FsmName != _fsmName ||
                fsm.gameObject.name != _objectName)
            {
                return;
            }

            // Open the chest if the item has already been collected
            FsmState init = fsm.GetState("Init");
            RandomizerExecuteLambda disableChest = new RandomizerExecuteLambda(() => fsm.SendEvent(
                    RandomizerMod.Instance.Settings.CheckLocationFound(_location) ? "ACTIVATE" : null
                    ));
            init.Actions = new FsmStateAction[]
            {
                init.Actions[0],
                init.Actions[1],
                init.Actions[2],
                init.Actions[3],
                init.Actions[4],
                disableChest,
                init.Actions[5],
                init.Actions[6],
            };

            // Remove actions that activate shiny item
            FsmState spawnItems = fsm.GetState("Spawn Items");
            spawnItems.RemoveActionsOfType<ActivateAllChildren>();
            fsm.GetState("Activated").RemoveActionsOfType<ActivateAllChildren>();

            // Add geo to chest
            // Chest geo pool cannot be trusted, often spawns less than it should
            spawnItems.AddAction(new RandomizerAddGeo(fsm.gameObject, _geoAmount));
            spawnItems.AddAction(new RandomizerExecuteLambda(() => GiveItemActions.GiveItem(GiveItemActions.GiveAction.None, _item, _location)));

            // Remove pre-existing geo from chest
            foreach (FlingObjectsFromGlobalPool fling in spawnItems.GetActionsOfType<FlingObjectsFromGlobalPool>())
            {
                fling.spawnMin = 0;
                fling.spawnMax = 0;
            }

            // Need to check SpawnFromPool action too because of Mantis Lords chest
            foreach (SpawnFromPool spawn in spawnItems.GetActionsOfType<SpawnFromPool>())
            {
                spawn.spawnMin = 0;
                spawn.spawnMax = 0;
            }
        }
    }
}
