using BepInEx;
using BoplFixedMath;
using UnityEngine;
using HarmonyLib;
using BepInEx.Logging;

namespace TripleProjectiles
{
    [BepInPlugin(ModID, ModName, ModVersion)]
    public class TripleProjectiles : BaseUnityPlugin
    {
        const string ModID = "com.mangochicken.tripleprojectiles";
        public const string ModName = "TripleProjectiles";
        public const string ModVersion = "1.0.0";

        internal static ManualLogSource Log;

        internal static bool IsFullGame;
        internal static bool HasCheckedDemo;

        internal static bool IsLoaded;

        private void Awake()
        {
            Log = this.Logger;

            //Harmony stuff
            Harmony harmony = new Harmony(ModID);

            harmony.PatchAll();

            On.ShootScaleChange.Shoot += ShootScaleChange_Shoot1;

            IsLoaded = true;

            Logger.LogInfo($"Plugin {ModName} is loaded!");
        }

        private void ShootScaleChange_Shoot1(On.ShootScaleChange.orig_Shoot orig, ShootScaleChange self, Vec2 firepointFIX, Vec2 directionFIX, ref bool hasFired, int playerId, bool alreadyHitWater)
        {
            orig.Invoke(self, firepointFIX, directionFIX, ref hasFired, playerId, alreadyHitWater);
            if (ShootScaleChange_Shoot.HasTripledThisShot || alreadyHitWater) 
            {
                return;
            }

            hasFired = false;
            Vec2 newDir = Helpers.RotateBy(directionFIX, (Fix)20);
            orig.Invoke(self, firepointFIX + ((Fix)2 * newDir), newDir, ref hasFired, playerId, alreadyHitWater);
            
            hasFired = false;
            newDir = Helpers.RotateBy(directionFIX, (Fix)(-20));
            orig.Invoke(self, firepointFIX + ((Fix)2 * newDir), newDir, ref hasFired, playerId, alreadyHitWater);
        }
    }

    

    public class Helpers
    {
        public static Vec2 RotateBy(Vec2 v, Fix delta, bool useRadians = false)
        {
            v = Vec2.Normalized(v);
            if (!useRadians) delta *= (Fix)Mathf.Deg2Rad;
            return new Vec2(
                v.x * Fix.Cos(delta) - v.y * Fix.Sin(delta),
                v.x * Fix.Sin(delta) + v.y * Fix.Cos(delta)
            );
        }
    }
}
