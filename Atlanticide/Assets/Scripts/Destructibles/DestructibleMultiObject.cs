using UnityEngine;
using System.Collections;

namespace Atlanticide
{
    public class DestructibleMultiObject : LinkDestructible
    {
        public MeshRenderer[] _meshRenderers;
        private PrefabSpawner[] _prefabSpawners;
        private ParticleSystem[] _particleSystems;

        private Collider _collider;
        public float destroyDelay;

        public Rigidbody RigidBody { get; private set; }

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            _prefabSpawners = GetComponentsInChildren<PrefabSpawner>(true);
            _collider = GetComponent<Collider>();
            _particleSystems = GetComponentsInChildren<ParticleSystem>(true);
            RigidBody = GetComponent<Rigidbody>();
        }

        public override void Destroy()
        {
            IsDestroyed = true;
            _toughnessLeft = 0f;
            if (_prefabSpawners.Length != 0)
            {
                foreach (PrefabSpawner prefabSpawner in _prefabSpawners)
                {
                    prefabSpawner.gameObject.SetActive(true);
                }
            }
            if (_particleSystems.Length != 0)
            {
                foreach (ParticleSystem particleSystem in _particleSystems)
                {
                    particleSystem.gameObject.SetActive(true);
                }
            }
            foreach(MeshRenderer mr in _meshRenderers)
            {
                mr.material.color = new Color(1f, 1f, 1f, 0f);
            }
            _collider.enabled = false;
            //SFXPlayer.Instance.Play(Sound.Cyclops_Exploding, volumeFactor: 0.3f);
            Destroy(gameObject, destroyDelay);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == 16 ||
                collision.gameObject.layer == 10)
            {
                //Debug.Log("Destroy()");
                Destroy();
            }
            if (collision.gameObject.layer == 8)
            {
                if (collision.gameObject.GetComponent<Rigidbody>().angularVelocity.magnitude > 0f)
                {
                    Destroy();
                }
            }
        }
    }
}