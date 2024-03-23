using HarmonyLib;
using BoplFixedMath;
using UnityEngine;

namespace TripleProjectiles.Patches
{
    // Remember to mention that this is not good to use on small platforms
    [HarmonyPatch]
    public class SpikePatch
    {
        static readonly Fix AngleBetween = (Fix)22;
        static int NumSpikes = 3;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Spike), nameof(Spike.Awake))]
        public static void AddExtraVisuals(Spike __instance)
        {
            var cp = __instance.gameObject.AddComponent<ExtraSpikeReferences>();
            cp.parent = __instance;
            cp.InstantiatePrefabs(__instance.dustParticle2Prefab);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Spike), nameof(Spike.CastSpike))]
        public static void CastSpikeTriple(Spike __instance)
        {
            var cp = __instance.GetComponent<ExtraSpikeReferences>();

            TripleProjectiles.Log.LogInfo(__instance);
            StickyRoundedRectangle attachedGround = __instance.playerPhysics.getAttachedGround();
            TripleProjectiles.Log.LogInfo(attachedGround);
            if (attachedGround == null)
            {
                return;
            }

            cp.DestroyOldSpikes();

            DPhysicsRoundedRect rect = attachedGround.GetComponent<DPhysicsRoundedRect>();
            Vec2 dir = attachedGround.currentNormal(__instance.body);
            dir = Helpers.RotateBy(dir, -AngleBetween * (Fix)((NumSpikes - 1) / 2));

            Fix far = Fix.Zero;
            Fix near = Fix.Zero;

            for (int i = 0; i < NumSpikes; i++)
            {
                if (i == (NumSpikes - 1) / 2)
                {
                    dir = Helpers.RotateBy(dir, AngleBetween);
                    continue;
                }
                if (!Raycast.RayCastRoundedRect(__instance.body.position, -dir, rect.GetRoundedRect(), out near, out far))
                {
                    MonoBehaviour.print("Spike raycast inexplicibly missed");
                    __instance.ExitAbility();
                    continue;
                }
                Vec2 vec2 = __instance.body.position - dir * far;

                bool right = i < (NumSpikes - 1) / 2;
                cp.DirectionalSpike(dir, vec2, attachedGround, rect, right);

                dir = Helpers.RotateBy(dir, AngleBetween);
            }
        }
    }
}
