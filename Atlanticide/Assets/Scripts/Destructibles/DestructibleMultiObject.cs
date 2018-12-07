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

        private Timer _destroyTimer;

        public Rigidbody RigidBody { get; private set; }

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            _prefabSpawners = GetComponentsInChildren<PrefabSpawner>(true);
            _collider = GetComponent<Collider>();
            _particleSystems = GetComponentsInChildren<ParticleSystem>(true);
            RigidBody = GetComponent<Rigidbody>();
            _destroyTimer = new Timer(destroyDelay, true);
        }

        protected override void UpdateObject()
        {
            base.UpdateObject();
            if (_destroyTimer.Active)
            {
                if (_destroyTimer.Check())
                {
                    gameObject.SetActive(false);
                    _destroyTimer.Reset();
                }
            }
        }

        public override void Destroy()
        {
            if (!IsDestroyed)
            {
                base.Destroy();
                SetSpawnedObjectsActive(true);
                foreach (MeshRenderer mr in _meshRenderers)
                {
                    mr.material.color = new Color(1f, 1f, 1f, 0f);
                }
                _collider.enabled = false;
                _destroyTimer.Activate();
                //Destroy(gameObject, destroyDelay);
            }
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
                Rigidbody rigidbody = collision.gameObject.GetComponent<Rigidbody>();
                if (rigidbody != null && rigidbody.angularVelocity.magnitude > 0f)
                {
                    Destroy();
                }
            }
        }

        public override void ResetObject()
        {
            base.ResetObject();
            SetSpawnedObjectsActive(false);
            foreach (MeshRenderer mr in _meshRenderers)
            {
                mr.material.color = new Color(1f, 1f, 1f, 1f);
            }
            _collider.enabled = true;
            _destroyTimer.Reset();
        }

        private void SetSpawnedObjectsActive(bool activate)
        {
            if (_prefabSpawners.Length != 0)
            {
                bool levelObject = _prefabSpawners[0].gameObject.GetComponent<LevelObject>() != null;
                foreach (PrefabSpawner prefabSpawner in _prefabSpawners)
                {
                    if (activate)
                    {
                        prefabSpawner.gameObject.SetActive(true);
                    }
                    else
                    {
                        prefabSpawner.gameObject.SetActive(false);
                        prefabSpawner.ResetSpawner();
                    }
                }
            }
            if (_particleSystems.Length != 0)
            {
                foreach (ParticleSystem particleSystem in _particleSystems)
                {
                    particleSystem.gameObject.SetActive(activate);
                }
            }
        }
    }
}