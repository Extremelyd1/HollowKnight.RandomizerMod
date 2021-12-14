using System.Collections.Generic;

namespace RandomizerMod.SceneChanges
{
    internal static partial class SceneEditor
    {
        private static List<PersistentBoolData> QueuedPersistentBoolData = new List<PersistentBoolData>();

        // Save our PersistentBoolData after the game does, so we overwrite the game's data rather than the other way round
        public static void SavePersistentBoolItems(On.GameManager.orig_SaveLevelState orig, GameManager self)
        {
            orig(self);
            foreach (PersistentBoolData pbd in QueuedPersistentBoolData)
            {
                SceneData.instance.SaveMyState(pbd);
            }
            QueuedPersistentBoolData.Clear();
        }

        public static void SavePersistentBoolItemState(PersistentBoolData pbd)
        {
            GameManager.instance.sceneData.SaveMyState(pbd);
            QueuedPersistentBoolData.Add(pbd);
        }
    }
}
