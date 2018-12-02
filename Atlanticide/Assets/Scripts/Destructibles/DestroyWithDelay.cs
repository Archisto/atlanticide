using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class DestroyWithDelay : MonoBehaviour
    {
        public float destroyDelay;

        // Use this for initialization
        void Start()
        {
            Destroy(gameObject, destroyDelay);
        }
    }
}