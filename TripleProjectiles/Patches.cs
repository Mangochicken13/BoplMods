using BoplFixedMath;
using HarmonyLib;
using UnityEngine;

namespace TripleProjectiles
{
    [HarmonyPatch(typeof(BowTransform), nameof(BowTransform.Shoot))]
    public class BowTransform_Shoot
    {
        static readonly int NumArrows = 3; // must be odd for now, will need extra logic for even
        static readonly Fix AngleBetween = (Fix)12;

        [HarmonyPostfix] // This being a postfix means it runs 
        public static void BowTransform_Prefix(BowTransform __instance, Vec2 dir,
            ref PlayerBody ___body, ref BoplBody ___Arrow, ref RingBuffer<BoplBody> ___Arrows, ref PlayerInfo ___playerInfo,
            ref Fix ___ArrowSpeed, ref int ___loadingFrame, ref Fix ___TimeBeforeArrowsHurtOwner, ref bool ___hasFired)
        {
            if (!TripleProjectiles.IsFullGame) 
            {
                return; 
            }

            Vec2 pos = ___body.position + __instance.FirepointOffset.x * ___body.right + __instance.FirepointOffset.y * ___body.up;
            Fix fix = Fix.One + (___body.fixtrans.Scale - Fix.One) / (Fix)2L;

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

                // spawning the other arrows
                BoplBody boplBody = FixTransform.InstantiateFixed<BoplBody>(___Arrow, pos, ___body.rotation);
                boplBody.Scale = ___body.fixtrans.Scale;
                ___Arrows.Add(boplBody);
                boplBody.GetComponent<IPlayerIdHolder>().SetPlayerId(___playerInfo.playerId);
                boplBody.GetComponent<SpriteRenderer>().material = ___playerInfo.playerMaterial;
                boplBody.StartVelocity = vec * ((Fix)((long)___loadingFrame) + Fix.One) * ___ArrowSpeed * fix + ___body.selfImposedVelocity;
                boplBody.GetComponent<Projectile>().DelayedEnableHurtOwner(___TimeBeforeArrowsHurtOwner * fix / Vec2.Magnitude(boplBody.StartVelocity));
                boplBody.rotation = ___body.rotation;

                vec = Helpers.RotateBy(vec, angle);
            }
            ___hasFired = true;
        }
    }

    [HarmonyPatch(typeof(BowTransform), nameof(BowTransform.Fire))]
    public class BowTransform_Fire
    {
        [HarmonyPostfix]
        static void Postfix(Vec2 inputVector, ref PlayerBody ___body)
        {
            if (!TripleProjectiles.IsFullGame)
            {
                return;
            }
            ___body.AddForce(-inputVector * (Fix)0.5);
        }
    }

    [HarmonyPatch(typeof(MainMenu), nameof(MainMenu.Start))]
    public class MainMenu_Start
    {
        [HarmonyPostfix]
        static void Postfix()
        {
            if (!TripleProjectiles.HasCheckedDemo)
            {
                TripleProjectiles.IsFullGame = SteamManager.instance.dlc.HasDLC();
                TripleProjectiles.HasCheckedDemo = true;
            }
        }
    }

    [HarmonyPatch(typeof(ShootScaleChange), nameof(ShootScaleChange.Shoot))]
    public class ShootScaleChange_Shoot
    {
        // try and call it three times with different angles??
        static readonly Fix AngleBetween = (Fix)20;

        [HarmonyPrefix]
        public static bool Prefix(Vec2 firepointFIX, Vec2 directionFIX, ref bool hasFired, int playerId, bool alreadyHitWater = false)
        {
            if (!TripleProjectiles.IsFullGame)
            {
                return true;
            }

            if (alreadyHitWater)
            {
                return true;
            }

            var vec2 = Helpers.RotateBy(directionFIX, AngleBetween);
            

            return true;
        }
    }
}
