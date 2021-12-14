using System;

namespace RandomizerMod
{
    public static class Events
    {
        // Logging events - these trigger in RandoLogger to allow other mods to make their own logs.
        public static event Action<string> OnTrackerLog;
        public static void LogTracker(string message) => OnTrackerLog?.Invoke(message);

        public static event Action<string> OnHelperLog;
        public static void LogHelper(string message) => OnHelperLog?.Invoke(message);

        public static event Action<string> OnSpoilerLog;
        public static void LogSpoiler(string message) => OnSpoilerLog?.Invoke(message);

        public static event Action<string> OnCondensedSpoilerLog;
        public static void LogCondensedSpoiler(string message) => OnCondensedSpoilerLog?.Invoke(message);

        // Logging initialization events - these trigger when the Randomizer________Log.txt is made. Allows other mods to properly sync their logs to Randomizer.
        public static event Action OnTrackerLogInit;
        public static void InitTracker() => OnTrackerLogInit?.Invoke();

        public static event Action OnHelperLogInit; // This event is called much more often, as helper log is fully remade at each transition & item pickup.
        public static void InitHelper() => OnHelperLogInit?.Invoke();

        public static event Action OnSpoilerLogInit;
        public static void InitSpoiler() => OnSpoilerLogInit?.Invoke();

        public static event Action OnCondensedSpoilerLogInit;
        public static void InitCondensedSpoiler() => OnCondensedSpoilerLogInit?.Invoke();

        /* To subscribe an event externally, we need to use System.Reflection to find the event in the assembly, and also UnityEngine to cast our delegate to an Action.
            Examples can be found in HKTranslator, and one is given below;

            For a given method OnTrackerLog(string message), we safely subscribe by doing:
        
            try 
            {
                Type.GetType("RandomizerMod.Events, RandomizerMod3.0")
                    .GetEvent("OnTrackerLog", BindingFlags.Public | BindingFlags.Static)
                    .AddEventHandler(null, (Action<string>)OnTrackerLog);
            } 
            catch 
            {
                Log("oof ouch my bones");
                return;
            }
         */
    }
}
