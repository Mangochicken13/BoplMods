using BepInEx;
using BoplFixedMath;
using UnityEngine;
using HarmonyLib;
using BepInEx.Logging;
using System.Reflection;

namespace TripleProjectiles
{
    [BepInPlugin(ModID, ModName, ModVersion)]
    [BepInProcess("BoplBattle.exe")]
    public class TripleProjectiles : BaseUnityPlugin
    {
        const string ModID = "com.mangochicken.tripleprojectiles";
        public const string ModName = "Triple Projectiles";
        public const string ModVersion = "1.0.0";

        internal static ManualLogSource Log;

        internal static Harmony Harmony;

        internal static bool IsFullGame;
        internal static bool HasCheckedDemo;

        internal static bool IsLoaded;

        private void Awake()
        {
            Log = this.Logger;

            //Harmony stuff
            Harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), ModID);

            IsLoaded = true;

            Logger.LogInfo($"Plugin {ModName} is loaded!");
        }
    }
}
