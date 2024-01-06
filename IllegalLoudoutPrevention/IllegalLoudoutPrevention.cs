using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace IllegalLoudoutPrevention
{
    [BepInPlugin(ModID, ModName, ModVersion)]
    [BepInProcess("BoplBattle.exe")]
    public class IllegalLoudoutPrevention : BaseUnityPlugin
    {
        private const string ModID = "com.mangochicken.IllegalLoudoutPrevention";
        private const string ModName = "Illegal Loudout Prevention";
        public const string ModVersion = "1.0.0";

        public static ManualLogSource Log { get; private set; }

        public IllegalLoudoutPrevention Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                DestroyImmediate(gameObject);
            }

            Log = Instance.Logger;

            Harmony harmony = new(ModID);

            harmony.PatchAll();

            Log.LogInfo($"Plugin {ModName} is loaded!");
        }
    }
}
