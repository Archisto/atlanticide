using UnityEngine;
using System.Collections;

namespace Atlanticide
{
    public class DestructibleGameObject : LinkDestructible
    {
        public DestructibleGameObject[] supports;
        private PrefabSpawner[] _prefabSpawners;
        private ParticleSystem[] _particleSystems;

        private MeshRenderer _meshRenderer;
        private Collider _collider;
        public float destroyDelay;

        public bool applyForceOnFall,
            isWall;
        public float forceOnFall,
            torqueOnFall;

        public GameObject ruins;

        public Rigidbody RigidBody { get; private set; }

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            _prefabSpawners = GetComponentsInChildren<PrefabSpawner>(true);
            _meshRenderer = GetComponent<MeshRenderer>();
            _collider = GetComponent<Collider>();
            _particleSystems = GetComponentsInChildren<ParticleSystem>(true);
            RigidBody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (supports.Length > 0 && RigidBody.useGravity == false && !IsDestroyed)
            {
                int linksDestroyedOrFalling = 0;
                foreach (DestructibleGameObject destructibleGameObject in supports)
                {
                    if (destructibleGameObject.IsDestroyed || destructibleGameObject.RigidBody.useGravity == true)
                    {
                        linksDestroyedOrFalling++;
                    }
                }
                if (linksDestroyedOrFalling == supports.Length)
                {
                    EnableGravityAndRemoveConstraints();
                }
            }
        }

        private void EnableGravityAndRemoveConstraints()
        {
            RigidBody.useGravity = true;
            RigidBody.constraints = RigidbodyConstraints.None;
            if (applyForceOnFall)
            {
                Vector3 v_forceOnFall = new Vector3(Random.Range(-forceOnFall, forceOnFall),
                    Random.Range(-forceOnFall, forceOnFall / 2f),
                    Random.Range(-forceOnFall, forceOnFall));
                Vector3 v_torqueOnFall = new Vector3(Random.Range(-torqueOnFall, torqueOnFall),
                    Random.Range(-torqueOnFall, torqueOnFall),
                    Random.Range(-torqueOnFall, torqueOnFall));
                RigidBody.AddForce(v_forceOnFall, ForceMode.Impulse);
                RigidBody.AddTorque(v_torqueOnFall);
            }
        }

        public override void Destroy()
        {
            _collider.enabled = false;
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
            _meshRenderer.material.color = new Color(1f, 1f, 1f, 0f);
            SFXPlayer.Instance.Play(_destroySound, volumeFactor: _volumeFactor);
            if (isWall) {
                ruins.SetActive(true);
            } else {
                Destroy(gameObject, destroyDelay);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == 16 ||
                collision.gameObject.layer == 10 ||
                collision.gameObject.layer == 8)
            {
                //Debug.Log("Destroy()");
                Destroy();
            }
        }
    }
}