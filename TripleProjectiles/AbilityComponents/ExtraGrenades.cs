using BoplFixedMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TripleProjectiles.AbilityComponents
{
    public class ExtraGrenades : MonoBehaviour
    {
        internal ThrowItem2 parent;

        internal Grenade leftGrenade;
        internal BoplBody leftGrenadeBody;
        internal Grenade rightGrenade;
        internal BoplBody rightGrenadeBody;

        public void InitializeGrenades(Grenade grenade, BoplBody body, bool right)
        {
            if (right)
            {
                rightGrenade = grenade;
                rightGrenadeBody = body;
            }
            else
            {
                leftGrenade = grenade;
                leftGrenadeBody = body;
            }
        }

        public void UpdateGrenades()
        {
            if (parent.grenade == null)
            {
                return;
            }
            if (!parent.hasFired && !parent.grenade.hasBeenThrown)
            {
                leftGrenadeBody.position = parent.dummyPos();
                rightGrenadeBody.position = parent.dummyPos();
            }
        }

        public void FireGrenade(Fix angleBetween, bool right)
        {
            Grenade grenade;
            BoplBody grenadeBody;
            Vec2 vec;
            if (right)
            {
                grenade = rightGrenade;
                grenadeBody = rightGrenadeBody;
                vec = Helpers.RotateBy(parent.dir, angleBetween);
            }
            else
            {
                grenade = leftGrenade;
                grenadeBody = leftGrenadeBody;
                vec = Helpers.RotateBy(parent.dir, -angleBetween);
            }

            //this.dummy.SetActive(false);
            //this.aimIndicator.SetActive(false);
            Vec2 vec2 = parent.body.position + Vec2.ComplexMul(new Vec2(parent.body.rotation), parent.player.Scale * (parent.isGroundedThrow ? parent.gThrowPosition : parent.aThrowPosition) * (Fix)((parent.transform.localScale.x < 0f) ? (-1L) : 1L));
            if (grenade != null && !grenade.hasBeenThrown)
            {
                grenade.DelayedEnableHurtOwner(parent.DisableGrenadeHitBoxTime * Fix.Max(parent.body.fixtrans.Scale, Fix.One));
                grenadeBody.GetComponent<DPhysicsCircle>().ManualInit();
                grenadeBody.position = vec2;
                grenade.hasBeenThrown = true;
                grenadeBody.UpdateSim(Fix.Zero);
                Fix fix = Fix.Lerp(parent.minThrowForce, parent.maxThrowForce, Fix.Clamp01(parent.FireInputTimeStamp * parent.ThrowForceGainSpeed));
                grenadeBody.AddForce(parent.body.fixtrans.Scale * fix * vec, ForceMode2D.Impulse);
                //grenadeBody.AddForce(vec * (Fix)10);
                //this.timeSinceFired = (Fix)0L;
                parent.body.selfImposedVelocity -= parent.knockback * vec;
                parent.physics.gravity_modifier = (Fix)1L;
                //this.hasFired = true;
                //AudioManager.Get().Play("throwSmallItem");
                //grenade.UpdateSim(grenade.detonationTime - grenade.selfDestructDelay);
                return;
            }
        }
    }
}
