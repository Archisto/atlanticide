using UnityEngine;
using System.Collections;

namespace Atlanticide
{
    public class DestructibleGameObject : LinkDestructible
    {
        private PrefabSpawner[] _prefabSpawners;
        private ParticleSystem[] _particleSystems;
        private MeshRenderer _meshRenderer;
        private Collider _collider;
        public float destroyDelay;

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
        }

        public void Destruction()
        {
            _toughnessLeft = 0f;
            if (_prefabSpawners.Length != 0) {
                foreach (PrefabSpawner prefabSpawner in _prefabSpawners)
                {
                    prefabSpawner.gameObject.SetActive(true);
                }
            }
            if (_particleSystems.Length != 0) {
                foreach (ParticleSystem prefabSpawner in _particleSystems)
                {
                    prefabSpawner.gameObject.SetActive(true);
                }
            }
            _meshRenderer.material.color = new Color(1f, 1f, 1f, 0f);
            _collider.enabled = false;
            //SFXPlayer.Instance.Play(Sound.Cyclops_Exploding, volumeFactor: 0.3f);
            Destroy(gameObject, destroyDelay);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.gameObject.layer == 16)
            {
                Debug.Log("Destruction()");
                Destruction();
            }
        }
    }
}