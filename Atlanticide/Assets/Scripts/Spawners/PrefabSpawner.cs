using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class PrefabSpawner : MonoBehaviour
    {
        public enum Type
        {
            Orichalcum,
            Stone,
            Wood,
            Terracotta
        }

        public Type type;

        public int spawnAmount;

        public float ejectForce,
            ejectAngle,
            torqueAmount;

        [Range(0f, 2f)]
        public float ejectForceVariance = 0.5f;

        [Range(0f, 2f)]
        public float baseScale = 1f;

        [Range(0f, 1f)]
        public float randomScale = 0.25f;

        private MeshRenderer _meshRenderer;

        private Vector3 _boundsMin,
            _boundsMax;

        private LevelManager _levelManager;

        private OrichalcumPickup _pooledOrichalcumPickup;

        private Debris _pooledDebris;

        private int _defaultSpawnAmount;

        // Use this for initialization
        void Start()
        {
            _meshRenderer = transform.parent.gameObject.GetComponent<MeshRenderer>();
            _boundsMin = _meshRenderer.bounds.min;
            _boundsMax = _meshRenderer.bounds.max;
            _levelManager = FindObjectOfType<LevelManager>();
            _defaultSpawnAmount = spawnAmount;

            // Null
            _pooledOrichalcumPickup = null;
            _pooledDebris = null;
        }

        // Update is called once per frame
        void Update()
        {
            while(spawnAmount > 0)
            {
                // Randomized quaternion for the pooled object.
                Quaternion randomQuaternion = new Quaternion(Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f));

                // Randomized position for the pooled object.
                Vector3 positionWithinBounds = new Vector3(Random.Range(_boundsMin.x, _boundsMax.x),
                    Random.Range(_boundsMin.y, _boundsMax.y),
                    Random.Range(_boundsMin.z, _boundsMax.z));

                //Randomized scale for the pooled object
                float randomizedScale = Random.Range(baseScale - randomScale, baseScale + randomScale);
                Vector3 v_randomScale = new Vector3(randomizedScale, randomizedScale, randomizedScale);

                //Randomized torque for the pooled object
                Vector3 randomTorque = new Vector3(Random.Range(-torqueAmount, torqueAmount),
                    Random.Range(-torqueAmount, torqueAmount),
                    Random.Range(-torqueAmount, torqueAmount));

                switch (type)
                {
                    case (Type.Orichalcum):
                        _pooledOrichalcumPickup = _levelManager.orichalcumPickupPool.GetPooledObject();
                        break;
                    case (Type.Stone):
                        _pooledDebris = _levelManager.stoneDebrisPool.GetPooledObject();
                        break;
                    case (Type.Wood):
                        _pooledDebris = _levelManager.woodDebrisPool.GetPooledObject();
                        break;
                    case (Type.Terracotta):
                        _pooledDebris = _levelManager.terracottaDebrisPool.GetPooledObject();
                        break;
                }

                if (type != Type.Orichalcum) {
                    //Randomized force for the pooled object.
                    Vector3 randomForce = Vector3.up + new Vector3(Random.Range(-ejectAngle, ejectAngle),
                        Random.Range(-ejectAngle, ejectAngle),
                        Random.Range(-ejectAngle, ejectAngle));
                    // Apply new position, rotation, scale, force and torque to the pooled object.
                    _pooledDebris.transform.position = positionWithinBounds;
                    _pooledDebris.transform.localRotation = randomQuaternion;
                    _pooledDebris.transform.localScale = v_randomScale;
                    _pooledDebris.GetComponent<Rigidbody>().AddForce(randomForce, ForceMode.Impulse);
                    _pooledDebris.GetComponent<Rigidbody>().AddTorque(randomTorque, ForceMode.Impulse);
                } else
                {
                    //Randomized force for the pooled object.
                    Vector3 randomForce = new Vector3(Random.Range(-ejectAngle, ejectAngle),
                        Random.Range(-ejectAngle / 2f, ejectAngle / 2f),
                        Random.Range(-ejectAngle, ejectAngle));
                    // Apply new position, rotation, scale, force and torque to the pooled object.
                    _pooledOrichalcumPickup.transform.position = positionWithinBounds;
                    _pooledOrichalcumPickup.transform.localRotation = randomQuaternion;
                    _pooledOrichalcumPickup.transform.localScale = v_randomScale;
                    _pooledOrichalcumPickup.GetComponent<Rigidbody>().AddForce(randomForce, ForceMode.Impulse);
                    _pooledOrichalcumPickup.GetComponent<Rigidbody>().AddTorque(randomTorque, ForceMode.Impulse);
                }

                spawnAmount--;
            }
        }

        public void ResetSpawner()
        {
            spawnAmount = _defaultSpawnAmount;
        }
    }
}