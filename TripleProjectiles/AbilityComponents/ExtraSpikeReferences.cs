using UnityEngine;

namespace TripleProjectiles.AbilityComponents
{
    public class ExtraSpikeReferences : MonoBehaviour
    {
        public SpikeAttack spikeLeft;
        public ParticleSystem farDustParticleLeft;

        public SpikeAttack spikeRight;
        public ParticleSystem farDustParticleRight;

        public Spike parent;

        public void InstantiatePrefabs(GameObject dustPrefab)
        {
            farDustParticleLeft = Instantiate(dustPrefab, parent.transform.position, Quaternion.identity).GetComponentInChildren<ParticleSystem>();
            farDustParticleRight = Instantiate(dustPrefab, parent.transform.position, Quaternion.identity).GetComponentInChildren<ParticleSystem>();
        }

        public void DestroyOldSpikes()
        {
            if (spikeLeft != null)
            {
                Updater.DestroyFix(spikeLeft.gameObject);
            }
            if (spikeRight != null)
            {
                Updater.DestroyFix(spikeRight.gameObject);
            }
        }

        public void OnDestroy()
        {
            if (farDustParticleLeft != null)
            {
                Destroy(farDustParticleLeft.gameObject);
            }
            if (farDustParticleRight != null)
            {
                Destroy(farDustParticleRight.gameObject);
            }
        }

        public void DirectionalSpike(Vec2 vec, Vec2 position, StickyRoundedRectangle attachedGround, DPhysicsRoundedRect rect2, bool right = false)
        {
            ParticleSystem _dustParticleFar;
            if (right)
            {
                _dustParticleFar = farDustParticleRight;
            }
            else
            {
                _dustParticleFar = farDustParticleLeft;
            }
            _dustParticleFar.transform.parent.position = (Vector3)position;
            _dustParticleFar.transform.parent.up = parent.transform.up;
            _dustParticleFar.Play();

            FixTransform component;

            if (right)
            {
                spikeRight = FixTransform.InstantiateFixed(parent.spikePrefab, position);
                component = spikeRight.GetComponent<FixTransform>();
                component.up = vec;
                component.transform.SetParent(rect2.transform);
                spikeRight.transform.localScale = new Vector3(1f / attachedGround.transform.localScale.x, 1f / attachedGround.transform.localScale.x, 1f);
                spikeRight.Initialize(position, parent.surfaceOffset, attachedGround);
                DetPhysics.Get().AddBoxToRR(spikeRight.gameObject.GetInstanceID(), attachedGround.gameObject.GetInstanceID());
                spikeRight.UpdateRelativeOrientation();
                spikeRight.UpdatePosition();
                spikeRight.GetHitbox().Scale = parent.body.fixtrans.Scale;
                attachedGround.GetGroundBody().AddForceAtPosition(vec * parent.castForce, parent.body.position, ForceMode2D.Force);
            }
            else
            {
                spikeLeft = FixTransform.InstantiateFixed(parent.spikePrefab, position);
                component = spikeLeft.GetComponent<FixTransform>();
                component.up = vec;
                component.transform.SetParent(rect2.transform);
                spikeLeft.transform.localScale = new Vector3(1f / attachedGround.transform.localScale.x, 1f / attachedGround.transform.localScale.x, 1f);
                spikeLeft.Initialize(position, parent.surfaceOffset, attachedGround);
                DetPhysics.Get().AddBoxToRR(spikeLeft.gameObject.GetInstanceID(), attachedGround.gameObject.GetInstanceID());
                spikeLeft.UpdateRelativeOrientation();
                spikeLeft.UpdatePosition();
                spikeLeft.GetHitbox().Scale = parent.body.fixtrans.Scale;
                attachedGround.GetGroundBody().AddForceAtPosition(vec * parent.castForce, parent.body.position, ForceMode2D.Force);
            }


        }
    }
}
