using BoplFixedMath;
using UnityEngine;

namespace TripleProjectiles.AbilityComponents
{
    public class ScaleChangeExtraVisuals : MonoBehaviour
    {
        public ParticleSystem rayParticleLeft;
        public ParticleSystem rayParticleChildLeft;
        public ParticleSystem hitParticleLeft;
        public GrowthFlare growthFlareLeft;

        public ParticleSystem rayParticleRight;
        public ParticleSystem rayParticleChildRight;
        public ParticleSystem hitParticleRight;
        public GrowthFlare growthFlareRight;

        public ShootScaleChange parent;

        public void InstantiatePrefabs(ParticleSystem rayPrefab, ParticleSystem hitPrefab, GrowthFlare flarePrefab)
        {
            rayParticleLeft = Instantiate(rayPrefab);
            rayParticleChildLeft = rayParticleLeft.transform.GetChild(0).GetComponent<ParticleSystem>();
            hitParticleLeft = Instantiate(hitPrefab);
            growthFlareLeft = Instantiate(flarePrefab);

            rayParticleRight = Instantiate(rayPrefab);
            rayParticleChildRight = rayParticleRight.transform.GetChild(0).GetComponent<ParticleSystem>();
            hitParticleRight = Instantiate(hitPrefab);
            growthFlareRight = Instantiate(flarePrefab);
        }

        private void Awake()
        {
            parent = transform.gameObject.GetComponent<ShootScaleChange>();
            if (parent == null)
            {
                TripleProjectiles.Log.LogInfo("null scale change");
            }
        }

        // Clean up the evidence /j
        private void OnDestroy()
        {
            if (rayParticleLeft != null)
            {
                Destroy(rayParticleLeft.gameObject);
            }
            if (hitParticleLeft != null)
            {
                Destroy(hitParticleLeft.gameObject);
            }
            if (rayParticleRight != null)
            {
                Destroy(rayParticleRight.gameObject);
            }
            if (hitParticleRight != null)
            {
                Destroy(hitParticleRight.gameObject);
            }
            if (growthFlareLeft != null)
            {
                Destroy(growthFlareLeft.gameObject);
            }
            if (growthFlareRight != null)
            {
                Destroy(growthFlareRight.gameObject);
            }
        }

        // This is a clone of the original behaviour, but put in here so it doesn't interfere with the original shot and no recursion shenanagins happen
        public void DirectionalShoot(Vec2 firepointFIX, Vec2 directionFIX, ref bool hasFired, int playerId, bool alreadyHitWater = false, bool right = false)
        {
            Vec2 vec = directionFIX;
            //AudioManager.Get().Play("fireRaygun");

            Debug.DrawRay((Vector2)firepointFIX, (float)parent.maxDistance * (Vector2)vec, new Color(255f, 255f, 0f));

            LayerMask mask = parent.collisionMask;
            if (alreadyHitWater)
            {
                mask = parent.collisionMaskNoWater;
            }

            RaycastInformation raycastInformation = DetPhysics.Get().PointCheckAllRoundedRects(firepointFIX);
            bool flag = !raycastInformation;
            if (flag)
            {
                raycastInformation = DetPhysics.Get().RaycastToClosest(firepointFIX, vec, parent.maxDistance, mask);
            }

            bool flag2 = raycastInformation;
            if (flag2)
            {
                Player player = PlayerHandler.Get().GetPlayer(playerId);
                //
                bool flag3 = !AchievementHandler.IsAchieved(AchievementHandler.AchievementEnum.GrowthRayYourself) && parent.blackHoleGrowthInverse01 > 0L && raycastInformation.layer == LayerMask.NameToLayer("Player") && raycastInformation.pp.fixTrans.GetComponent<IPlayerIdHolder>().GetPlayerId() == playerId && player != null && player.IsLocalPlayer;
                if (flag3)
                {
                    AchievementHandler.TryAwardAchievement(AchievementHandler.AchievementEnum.GrowthRayYourself);
                }
                bool hitWater = raycastInformation.layer == LayerMask.NameToLayer("Water");
                if (hitWater)
                {
                    SpawnRayCastEffect((Vector3)firepointFIX, (Vector3)vec, (float)raycastInformation.nearDist, false, true, right);
                    StopDirectionalRayParticle(right);
                    vec = new Vec2(vec.x, vec.y * -Fix.One);

                    DirectionalShoot(raycastInformation.nearPos, vec, ref hasFired, playerId, true, right);
                    Debug.DrawRay((Vector2)raycastInformation.nearPos, (Vector2)(vec * parent.maxDistance), Color.magenta);

                    GameObject obj = Instantiate(parent.WaterRing);

                    //AudioManager.Get().Play("waterExplosion");
                    obj.transform.position = new Vector3(parent.WaterRing.transform.position.x + (float)raycastInformation.nearPos.x, parent.WaterRing.transform.position.y, parent.WaterRing.transform.position.z);
                    return;
                }
                bool HitObject = raycastInformation.layer == LayerMask.NameToLayer("RigidBodyAffector");
                if (HitObject)
                {
                    BlackHole component3 = raycastInformation.pp.fixTrans.gameObject.GetComponent<BlackHole>();
                    if (component3 != null)
                    {
                        component3.Grow(parent.blackHoleGrowthInverse01, Fix.Zero);
                    }
                }
                else
                {
                    GameObject obj2 = raycastInformation.pp.fixTrans.gameObject;
                    ScaleChanger scaleChanger = FixTransform.InstantiateFixed(parent.ScaleChangerPrefab, Vec2.zero);
                    scaleChanger.victim = raycastInformation.pp.monobehaviourCollider;
                    Ability component = obj2.GetComponent<Ability>();
                    bool flag6 = component != null;
                    if (flag6)
                    {
                        scaleChanger.player = PlayerHandler.Get().GetPlayer(component.GetPlayerId());
                    }
                    SlimeController component2 = obj2.GetComponent<SlimeController>();
                    bool flag7 = component2 != null;
                    if (flag7)
                    {
                        scaleChanger.player = PlayerHandler.Get().GetPlayer(component2.GetPlayerId());
                    }
                    FixTransform fixTrans = raycastInformation.pp.fixTrans;
                    SpriteRenderer spriteRenderer = fixTrans != null ? fixTrans.GetComponent<SpriteRenderer>() : null;
                    bool flag8 = spriteRenderer != null;
                    if (flag8)
                    {
                        InitDirectionalGrowthFlare(spriteRenderer, right);
                        parent.growthFlare.Init(spriteRenderer);
                    }
                }
                bool flag9 = parent.blackHoleGrowthInverse01 > 0L;
                if (flag9)
                {
                    AudioManager.Get().Play("grow");
                }
                else
                {
                    AudioManager.Get().Play("shrink");
                }
                SpawnRayCastEffect((Vector3)firepointFIX, (Vector3)vec, (float)raycastInformation.nearDist, true, false, right);
            }
            else
            {
                SpawnRayCastEffect((Vector2)firepointFIX, (Vector2)vec, (float)parent.maxDistance, false, false, right);
            }
            hasFired = true;
        }

        public void SpawnRayCastEffect(Vector2 start, Vector2 dir, float dist, bool didHit, bool reflected = false, bool right = false)
        {
            ParticleSystem _rayParticle;
            ParticleSystem _rayParticleChild;
            ParticleSystem _hitParticle;

            if (right)
            {
                _rayParticle = rayParticleRight;
                _rayParticleChild = rayParticleChildRight;
                _hitParticle = hitParticleRight;
            }
            else
            {
                _rayParticle = rayParticleLeft;
                _rayParticleChild = rayParticleChildLeft;
                _hitParticle = hitParticleLeft;
            }

            ParticleSystem.ShapeModule shape = _rayParticle.shape;
            ParticleSystem.EmissionModule emission = _rayParticle.emission;
            ParticleSystem.Burst burst = emission.GetBurst(0);
            shape.scale = new Vector3(dist, shape.scale.y, shape.scale.z);
            _rayParticle.transform.right = dir;
            _rayParticle.transform.position = start + dir * dist * 0.5f;
            burst.count = dist * parent.rayDensity;
            emission.SetBurst(0, burst);
            ParticleSystem.ShapeModule shape2 = _rayParticleChild.shape;
            ParticleSystem.EmissionModule emission2 = _rayParticleChild.emission;
            ParticleSystem.Burst burst2 = emission2.GetBurst(0);
            shape2.scale = new Vector3(dist, shape2.scale.y, shape2.scale.z);
            burst2.count = dist * parent.rayDensityChild;
            emission2.SetBurst(0, burst2);
            if (reflected)
            {
                _rayParticle.Simulate(0.16f);
            }
            _rayParticle.Play();
            if (didHit)
            {
                _hitParticle.transform.position = start + dir * dist;
                _hitParticle.Play();
            }
        }

        public void StopDirectionalRayParticle(bool right = false)
        {
            ParticleSystem _rayParticle;
            if (right)
            {
                _rayParticle = rayParticleRight;
            }
            else
            {
                _rayParticle = rayParticleLeft;
            }
            _rayParticle.Stop();
        }

        public void InitDirectionalGrowthFlare(SpriteRenderer spriteRenderer, bool right = false)
        {
            GrowthFlare _growthFlare;
            if (right)
            {
                _growthFlare = growthFlareRight;
            }
            else
            {
                _growthFlare = growthFlareLeft;
            }
            _growthFlare.Init(spriteRenderer);
        }
    }
}
