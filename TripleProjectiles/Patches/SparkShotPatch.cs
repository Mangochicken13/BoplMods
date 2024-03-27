using HarmonyLib;
using System;
using UnityEngine;
using BoplFixedMath;

namespace TripleProjectiles.Patches
{
    [HarmonyPatch]
    public class SparkShotPatch
    {
        [HarmonyPatch(typeof(SparkShot), nameof(SparkShot.Fire))]
        [HarmonyPostfix]
        public static void CallMissileFromSky(SparkShot __instance)
        {
            Vec2 vec = __instance.body.position;
            vec += Vec2.up * (Fix)100;
            TripleProjectiles.Log.LogInfo(__instance.body.rotation);
            
            BoplBody sparkBody = FixTransform.InstantiateFixed<BoplBody>(__instance.SparkPrefab, vec, Fix.Pi);
            sparkBody.Scale = __instance.body.fixtrans.Scale;
            //this.player.CustomAimAbilityPosition = vec;
            sparkBody.StartVelocity = __instance.projectileStartSpeed * Vec2.down;
            //this.animator.beginAnimThenDoAction(this.animData.GetAnimation("enterToIdle"), new Action(this.Idle));
            sparkBody.GetComponent<SpriteRenderer>().material = __instance.playerInfo.playerMaterial;
            sparkBody.GetComponent<Item>().OwnerId = __instance.playerInfo.playerId;
            sparkBody.GetComponent<DestroyIfOutsideSceneBounds>().DoNotDestroyUp = true;
            //this.missile = this.sparkBody.GetComponent<Missile>();
            
        }
    }
}
