using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class ObeliskBroken : MonoBehaviour
    {
        public string[] collisionLayerStrings;

        private int[] _collisionLayerInts;

        public Obelisk obelisk;

        public ParticleSystem dustParticleSystemPrefab;

        // Use this for initialization
        void Start()
        {
            _collisionLayerInts = new int[collisionLayerStrings.Length];
            for (int i = 0; i < _collisionLayerInts.Length; i++)
            {
                _collisionLayerInts[i] = LayerMask.NameToLayer(collisionLayerStrings[i]);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnCollisionEnter(Collision collision)
        {
            for (int i = 0; i < _collisionLayerInts.Length; i++)
            {
                if (collision.gameObject.layer == _collisionLayerInts[i])
                {
                    obelisk.StopFallingObelisk();
                    ContactPoint[] contactPoints = collision.contacts;
                    foreach (ContactPoint cp in contactPoints)
                    {
                        Instantiate(dustParticleSystemPrefab, cp.point, Quaternion.identity);
                    }
                }
            }
        }
    }
}