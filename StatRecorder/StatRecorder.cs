using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace StatRecorder
{
    [BepInPlugin(GUID, ModName, ModVersion)]
    public class StatRecorder : BaseUnityPlugin
    {
        const string GUID = "com.mangochicken.boplbattle.StatRecorder";
        const string ModName = "Stat Recorder";
        public const string ModVersion = "1.0.0";



        public static ManualLogSource Log;


        #region Stats
        static int RoundsPlayedThisSession;

        public PlayerData[] CurrentStats = new PlayerData[6];
        public PlayerData[] PreviousStats;

        #endregion

        private int i = 0;


        private void Awake()
        {
            Log = this.Logger;

            Harmony harmony = new(GUID);

            harmony.PatchAll();

            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }

    public class PlayerData
    {
        public string Name { get; set; }

        public int Wins { get; set; }

        public int Deaths { get; set; }

        public int Kills { get; set; }

        public Dictionary<string, int[]> AbilityUses;

    }
}
