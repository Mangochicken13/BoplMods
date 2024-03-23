using UnityEngine;
using BoplFixedMath;

namespace TripleProjectiles
{
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
