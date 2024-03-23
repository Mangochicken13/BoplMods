using HarmonyLib;
using BoplFixedMath;

namespace TripleProjectiles.Patches
{
    [HarmonyPatch]
    public class ShootQuantumPatch
    {
        static readonly Fix AngleBetween = (Fix)20;

        const int NumShots = 3;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ShootQuantum), nameof(ShootQuantum.Awake))]
        public static void AddExtraVisuals(ShootQuantum __instance)
        {
            var cp = __instance.gameObject.AddComponent<ShootQuantumExtraVisuals>();
            cp.parent = __instance;
            cp.InstantiatePrefabs(__instance.RaycastParticlePrefab, __instance.RaycastParticleHitPrefab);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ShootQuantum), nameof(ShootQuantum.Shoot))]
        public static bool TripleScaleRays_Prefix(ShootQuantum __instance, Vec2 directionFIX, Vec2 firepointFIX, ref bool hasFired, int playerId, bool alreadyHitWater = false)
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

                var ev = __instance.gameObject.GetComponent<ShootQuantumExtraVisuals>();

                bool right = i < (NumShots - 1) / 2;
                ev.DirectionalShoot(firepointFIX, vec, ref hasFired, playerId, alreadyHitWater, right);

                vec = Helpers.RotateBy(vec, AngleBetween);
            }
            return true;
        }
    }
}
