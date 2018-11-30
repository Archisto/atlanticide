using UnityEngine;
using System.Collections;

namespace Atlanticide
{
    public class DestructibleGameObject : LinkDestructible
    {
        public DestructibleGameObject[] links,
            suicidePartners;
        private PrefabSpawner[] _prefabSpawners;
        private ParticleSystem[] _particleSystems;
        private MeshRenderer _meshRenderer;
        private Collider _collider;
        public float destroyDelay;

        public Rigidbody RigidBody { get; private set; }

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            _prefabSpawners = GetComponentsInChildren<PrefabSpawner>(true);
            //Debug.Log("PrefabSpawners found: " + _prefabSpawners.Length);
            _meshRenderer = GetComponent<MeshRenderer>();
            _collider = GetComponent<Collider>();
            _particleSystems = GetComponentsInChildren<ParticleSystem>(true);
            //Debug.Log("ParticleSystems found: " + _particleSystems.Length);
            RigidBody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (links.Length > 0 && RigidBody.useGravity == false && !IsDestroyed)
            {
                int linksDestroyedOrFalling = 0;
                foreach (DestructibleGameObject destructibleGameObject in links)
                {
                    if (destructibleGameObject.IsDestroyed || destructibleGameObject.RigidBody.useGravity == true)
                    {
                        linksDestroyedOrFalling++;
                    }
                }
                if (linksDestroyedOrFalling == links.Length)
                {
                    RigidBody.useGravity = true;
                }
            }
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
                foreach (ParticleSystem prefabSpawner in _particleSystems)
                {
                    prefabSpawner.gameObject.SetActive(true);
                }
            }
            _meshRenderer.material.color = new Color(1f, 1f, 1f, 0f);
            _collider.enabled = false;
            //SFXPlayer.Instance.Play(Sound.Cyclops_Exploding, volumeFactor: 0.3f);
            Destroy(gameObject, destroyDelay);
            if (suicidePartners.Length > 0)
            {
                foreach(DestructibleGameObject destructibleGameObject in suicidePartners)
                {
                    destructibleGameObject.Destroy();
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.gameObject.layer == 16 || collision.collider.gameObject.layer == 10)
            {
                //Debug.Log("Destroy()");
                Destroy();
            }
        }
    }
}