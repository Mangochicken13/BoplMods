using HarmonyLib;
using UnityEngine;
using BoplFixedMath;

namespace TripleProjectiles.Patches
{
    [HarmonyPatch(typeof(BowTransform))]
    public class Bow_Patches
    {
        static readonly int NumArrows = 3; // must be odd for now, will need extra logic for even
        static readonly Fix AngleBetween = (Fix)12;

        [HarmonyPostfix] // This being a postfix means it runs afterwards, and doesn't need to return false in a prefix causing incompatabilities
        [HarmonyPatch(nameof(BowTransform.Shoot))]
        public static void BowTransform_MoreArrows(BowTransform __instance, Vec2 dir)
        {
            if (!TripleProjectiles.IsFullGame)
            {
                return;
            }

            Vec2 pos = __instance.body.position + __instance.FirepointOffset.x * __instance.body.right + __instance.FirepointOffset.y * __instance.body.up;
            Fix fix = Fix.One + (__instance.body.fixtrans.Scale - Fix.One) / (Fix)2L;

            Fix angle = AngleBetween; // angle the arrows should be separated by
            Vec2 vec = Helpers.RotateBy(dir, -angle * (Fix)((NumArrows - 1) / 2));

            vec = Vec2.Normalized(vec);

            for (int i = 0; i < NumArrows; i++)
            {
                if (i == (NumArrows - 1) / 2)
                {
                    vec = Helpers.RotateBy(vec, angle);
                    continue;
                }

                TripleProjectiles.Log.LogInfo($"{i}: {vec}");

                // Put this in its own utility function
                // spawning the other arrows
                BoplBody boplBody = FixTransform.InstantiateFixed(__instance.Arrow, pos, __instance.body.rotation);
                boplBody.Scale = __instance.body.fixtrans.Scale;
                __instance.Arrows.Add(boplBody);
                boplBody.GetComponent<IPlayerIdHolder>().SetPlayerId(__instance.playerInfo.playerId);
                boplBody.GetComponent<SpriteRenderer>().material = __instance.playerInfo.playerMaterial;
                boplBody.StartVelocity = vec * ((Fix)__instance.loadingFrame + Fix.One) * __instance.ArrowSpeed * fix + __instance.body.selfImposedVelocity;
                boplBody.GetComponent<Projectile>().DelayedEnableHurtOwner(__instance.TimeBeforeArrowsHurtOwner * fix / Vec2.Magnitude(boplBody.StartVelocity));
                boplBody.rotation = __instance.body.rotation;

                vec = Helpers.RotateBy(vec, angle);
            }
            __instance.hasFired = true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(BowTransform.Fire))]
        static void ExtraKnockback_Bow(Vec2 inputVector, BowTransform __instance)
        {
            if (!TripleProjectiles.IsFullGame)
            {
                return;
            }

            __instance.body.AddForce(-inputVector * (Fix)0.5); // add extra logic if i 
        }
    }
}
