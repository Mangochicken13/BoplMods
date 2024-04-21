using BoplFixedMath;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripleProjectiles.AbilityComponents;
using UnityEngine;

namespace TripleProjectiles.Patches
{
    [HarmonyPatch]
    public class GrenadePatch
    {
        static readonly int NumOfGrenades = 3;
        static readonly Fix AngleBetween = (Fix)18;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ThrowItem2), nameof(ThrowItem2.Awake))]
        public static void AddExtraGrenadeReferences(ThrowItem2 __instance)
        {
            if (!TripleProjectiles.IsFullGame)
            {
                return;
            }

            var cp = __instance.gameObject.AddComponent<ExtraGrenades>();
            cp.parent = __instance;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ThrowItem2), nameof(ThrowItem2.SpawnGrenade))]
        public static void SpawnExtraGrenades(ThrowItem2 __instance)
        {
            if (!TripleProjectiles.IsFullGame)
            {
                return;
            }

            var cp = __instance.gameObject.GetComponent<ExtraGrenades>();
            for (int i = 0; i < NumOfGrenades; i++)
            {
                if (i == (NumOfGrenades - 1) / 2)
                {
                    continue;
                }
                var grenadeBody = FixTransform.InstantiateFixed<BoplBody>(__instance.ItemPrefab, __instance.dummyPos(), __instance.body.rotation);
                grenadeBody.Scale = __instance.player.Scale;
                Item component = grenadeBody.GetComponent<Item>();
                component.OwnerId = __instance.playerInfo.playerId;
                var grenade = component.GetComponent<Grenade>();
                grenade.Initialize(__instance);
                grenade.DetonatesOnOwner = false;
                //this.dummy.SetActive(false);

                cp.InitializeGrenades(grenade, grenadeBody, i == 0);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ThrowItem2), nameof(ThrowItem2.Fire))]
        public static void ShootExtraGrenades(ThrowItem2 __instance)
        {
            if (!TripleProjectiles.IsFullGame)
            {
                return;
            }

            var cp = __instance.gameObject.GetComponent<ExtraGrenades>();
            cp.FireGrenade(AngleBetween, true);
            cp.FireGrenade(AngleBetween, false);
        }

        //[HarmonyPatch(typeof(Grenade), nameof(Grenade.UpdateSim))]
        //[HarmonyPrefix]
        public static void GrenadeLogger(Grenade __instance)
        {
            TripleProjectiles.Log.LogInfo(__instance.selfDestructDelay);
        }

    }
}
