using BoplFixedMath;
using HarmonyLib;

namespace TripleProjectiles.Patches
{
    [HarmonyPatch]
    public class ShootScaleChange_Shoot
    {
        // try and call it three times with different angles??
        static readonly Fix AngleBetween = (Fix)20;

        const int NumShots = 3;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ShootScaleChange), nameof(ShootScaleChange.Awake))]
        public static void AddExtraVisuals(ShootScaleChange __instance)
        {
            var cp = __instance.gameObject.AddComponent<ScaleChangeExtraVisuals>();
            cp.parent = __instance;
            cp.InstantiatePrefabs(__instance.RaycastParticlePrefab, __instance.RaycastParticleHitPrefab, __instance.flarePrefab);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ShootScaleChange), nameof(ShootScaleChange.Shoot))]
        public static bool TripleScaleRays_Prefix(ShootScaleChange __instance, Vec2 directionFIX, Vec2 firepointFIX, ref bool hasFired, int playerId, bool alreadyHitWater = false)
        {
            if (!TripleProjectiles.IsFullGame)
            {
                return true;
            }

            if (alreadyHitWater)
            {
                return true;
            }

            Vec2 vec = Helpers.RotateBy(directionFIX, -AngleBetween * (Fix)((NumShots - 1) / 2));
            vec = Vec2.Normalized(vec);

            for (int i = 0; i < NumShots; i++)
            {
                // skip shooting if its the middle number
                if (i == (NumShots - 1) / 2)
                {
                    vec = Helpers.RotateBy(vec, AngleBetween);
                    continue;
                }

                var ev = __instance.gameObject.GetComponent<ScaleChangeExtraVisuals>();

                bool right = i < (NumShots - 1) / 2;
                ev.DirectionalShoot(firepointFIX, vec, ref hasFired, playerId, alreadyHitWater, right);
                
                vec = Helpers.RotateBy(vec, AngleBetween);
            }
            return true;
        }
    }
}
