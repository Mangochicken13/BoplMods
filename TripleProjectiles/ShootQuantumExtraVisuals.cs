using BoplFixedMath;
using UnityEngine;

namespace TripleProjectiles
{
    public class ShootQuantumExtraVisuals : MonoBehaviour
    {
        private ParticleSystem rayParticleLeft;
        private ParticleSystem rayParticleChildLeft;
        private ParticleSystem hitParticleLeft;

        private ParticleSystem rayParticleRight;
        private ParticleSystem rayParticleChildRight;
        private ParticleSystem hitParticleRight;

        public ShootQuantum parent;
        public void InstantiatePrefabs(ParticleSystem rayPrefab, ParticleSystem hitPrefab)
        {
            rayParticleLeft = Instantiate(rayPrefab);
            rayParticleLeft.transform.position.Set(1500, 1500, 1500);
            rayParticleChildLeft = Instantiate(rayPrefab);
            rayParticleChildLeft.transform.position.Set(1500, 1500, 1500);
            hitParticleLeft = Instantiate(hitPrefab);

            rayParticleRight = Instantiate(rayPrefab);
            rayParticleRight.transform.position.Set(1500, 1500, 1500);
            rayParticleChildRight = Instantiate(rayPrefab);
            rayParticleChildRight.transform.position.Set(1500, 1500, 1500);
            hitParticleRight = Instantiate(hitPrefab);
        }

        public void DirectionalShoot(Vec2 firepointFIX, Vec2 directionFIX, ref bool hasFired, int playerId, bool alreadyHitWater = false, bool right = false)
        {
            Vec2 vec = directionFIX;
            //AudioManager.Get().Play("laserShoot");
            Debug.DrawRay((Vector2)firepointFIX, (float)parent.maxDistance * (Vector2)vec, new Color(255f, 255f, 0f));
            RaycastInformation raycastInformation = DetPhysics.Get().PointCheckAllRoundedRects(firepointFIX);
            if (!raycastInformation)
            {
                raycastInformation = DetPhysics.Get().RaycastToClosest(firepointFIX, vec, parent.maxDistance, parent.collisionMask);
            }
            if (!raycastInformation && firepointFIX.y <= SceneBounds.WaterHeight && !alreadyHitWater)
            {
                SpawnRayCastEffect((Vector3)firepointFIX, (Vector3)vec, (float)raycastInformation.nearDist, false, Vec2.up, true, right);
                StopDirectionalRayParticle(right);
                //this.rayParticle.Stop();
                GameObject gameObject = Object.Instantiate<GameObject>(parent.WaterRing);
                //AudioManager.Get().Play("waterExplosion");
                gameObject.transform.position = new Vector3(parent.WaterRing.transform.position.x + (float)raycastInformation.nearPos.x, parent.WaterRing.transform.position.y, parent.WaterRing.transform.position.z);
                return;
            }
            if (raycastInformation)
            {
                if (raycastInformation.layer == LayerMask.NameToLayer("Player") && raycastInformation.pp.fixTrans.GetComponent<IPlayerIdHolder>().GetPlayerId() == playerId && !alreadyHitWater)
                {
                    DirectionalShoot(firepointFIX + directionFIX * (Fix)2f, directionFIX, ref hasFired, playerId, false, right);
                    return;
                }
                Vec2 vec2 = Vec2.NormalizedSafe(raycastInformation.pp.monobehaviourCollider.NormalAtPoint(raycastInformation.nearPos));
                if (raycastInformation.layer == LayerMask.NameToLayer("Water") && !alreadyHitWater)
                {
                    SpawnRayCastEffect((Vector3)firepointFIX, (Vector3)vec, (float)raycastInformation.nearDist, false, vec2, true, right);
                    StopDirectionalRayParticle(right);
                    //this.rayParticle.Stop();
                    vec = new Vec2(vec.x, vec.y * -Fix.One);
                    DirectionalShoot(raycastInformation.nearPos, vec, ref hasFired, playerId, true, right);
                    Debug.DrawRay((Vector2)raycastInformation.nearPos, (Vector2)(vec * parent.maxDistance), Color.magenta);
                    GameObject gameObject2 = Object.Instantiate<GameObject>(parent.WaterRing);
                    //AudioManager.Get().Play("waterExplosion");
                    gameObject2.transform.position = new Vector3(parent.WaterRing.transform.position.x + (float)raycastInformation.nearPos.x, parent.WaterRing.transform.position.y, parent.WaterRing.transform.position.z);
                    return;
                }
                if (raycastInformation.layer == LayerMask.NameToLayer("EffectorZone") || raycastInformation.layer == LayerMask.NameToLayer("weapon"))
                {
                    GameObject gameObject3 = raycastInformation.pp.fixTrans.gameObject;
                    QuantumTunnel quantumTunnel = FixTransform.InstantiateFixed<QuantumTunnel>(parent.QuantumTunnelPrefab, raycastInformation.pp.fixTrans.position);
                    if (!raycastInformation.pp.fixTrans.gameObject.CompareTag("InvincibilityZone"))
                    {
                        quantumTunnel.Init(gameObject3, parent.WallDuration, null, false);
                    }
                }
                else
                {
                    GameObject gameObject4 = raycastInformation.pp.fixTrans.gameObject;
                    QuantumTunnel quantumTunnel2 = null;
                    for (int i = 0; i < ShootQuantum.spawnedQuantumTunnels.Count; i++)
                    {
                        if (ShootQuantum.spawnedQuantumTunnels[i].Victim.GetInstanceID() == gameObject4.GetInstanceID())
                        {
                            quantumTunnel2 = ShootQuantum.spawnedQuantumTunnels[i];
                        }
                    }
                    if (quantumTunnel2 == null)
                    {
                        quantumTunnel2 = FixTransform.InstantiateFixed<QuantumTunnel>(parent.QuantumTunnelPrefab, raycastInformation.pp.fixTrans.position);
                        ShootQuantum.spawnedQuantumTunnels.Add(quantumTunnel2);
                    }
                    if (gameObject4.layer == LayerMask.NameToLayer("wall"))
                    {
                        ShakablePlatform component = gameObject4.GetComponent<ShakablePlatform>();
                        if (gameObject4.CompareTag("ResizablePlatform"))
                        {
                            Material material = parent.onHitResizableWallMaterail;
                            gameObject4.GetComponent<SpriteRenderer>();
                            DPhysicsRoundedRect component2 = gameObject4.GetComponent<DPhysicsRoundedRect>();
                            material.SetFloat("_Scale", gameObject4.transform.localScale.x);
                            material.SetFloat("_BevelRadius", (float)component2.radius);
                            Vec2 vec3 = component2.CalcExtents();
                            material.SetFloat("_RHeight", (float)vec3.y);
                            material.SetFloat("_RWidth", (float)vec3.x);
                            component.AddShake(parent.WallDelay, parent.WallShake, 10, material, null);
                            quantumTunnel2.DelayedInit(gameObject4, parent.WallDuration, parent.WallDelay, parent.onDissapearResizableWallMaterail);
                        }
                        else
                        {
                            component.AddShake(parent.WallDelay, parent.WallShake, 10, parent.onHitWallMaterail, null);
                            quantumTunnel2.DelayedInit(gameObject4, parent.WallDuration, parent.WallDelay, null);
                        }
                    }
                    else if (gameObject4.layer == LayerMask.NameToLayer("RigidBodyAffector"))
                    {
                        if (gameObject4.GetComponent<BlackHole>() != null)
                        {
                            quantumTunnel2.Init(gameObject4, parent.WallDuration, parent.onHitBlackHoleMaterial, false);
                        }
                        else
                        {
                            quantumTunnel2.Init(gameObject4, parent.WallDuration, null, false);
                        }
                    }
                    else if (gameObject4.layer == LayerMask.NameToLayer("Player"))
                    {
                        IPlayerIdHolder component3 = gameObject4.GetComponent<IPlayerIdHolder>();
                        Player player = PlayerHandler.Get().GetPlayer(component3.GetPlayerId());
                        if (player != null)
                        {
                            int timesHitByBlinkgunThisRound = player.timesHitByBlinkgunThisRound;
                            player.timesHitByBlinkgunThisRound = timesHitByBlinkgunThisRound + 1;
                        }
                        Fix fix = Fix.Max(parent.minPlayerDuration, (Fix)0.3 * parent.WallDuration);
                        quantumTunnel2.Init(gameObject4, fix, null, true);
                    }
                    else
                    {
                        quantumTunnel2.Init(gameObject4, parent.WallDuration, null, false);
                    }
                }
                SpawnRayCastEffect((Vector2)firepointFIX, (Vector2)vec, (float)raycastInformation.nearDist, true, vec2, false, right);
            }
            else
            {
                SpawnRayCastEffect((Vector2)firepointFIX, (Vector2)vec, (float)parent.maxDistance, false, Vec2.up, false, right);
            }
            hasFired = true;
        }

        private void SpawnRayCastEffect(Vector2 start, Vector2 dir, float dist, bool didHit, Vec2 normal, bool reflected = false, bool right = false)
        {
            ParticleSystem particleSystem;
            ParticleSystem _hitParticle;
            if (right)
            {
                particleSystem = (reflected ? rayParticleChildRight : rayParticleRight);
                _hitParticle = hitParticleRight;
            }
            else
            {
                particleSystem = (reflected ? rayParticleChildLeft : rayParticleLeft);
                _hitParticle = hitParticleLeft;
            }
                
            ParticleSystem.ShapeModule shape = particleSystem.shape;
            ParticleSystem.EmissionModule emission = particleSystem.emission;
            ParticleSystem.Burst burst = emission.GetBurst(0);
            shape.scale = new Vector3(dist, shape.scale.y, shape.scale.z);
            particleSystem.transform.right = dir;
            particleSystem.transform.position = start + dir * dist * 0.5f;
            burst.count = dist * parent.rayDensity;
            emission.SetBurst(0, burst);
            particleSystem.Play();
            if (reflected)
            {
                _hitParticle.transform.position = start + dir * dist;
                _hitParticle.transform.up = (Vector3)normal;
                _hitParticle.Simulate(0.16f);
                _hitParticle.Play();
            }
            if (didHit)
            {
                _hitParticle.Stop();
                _hitParticle.transform.position = start + dir * dist;
                _hitParticle.transform.up = (Vector3)normal;
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
    }
}
