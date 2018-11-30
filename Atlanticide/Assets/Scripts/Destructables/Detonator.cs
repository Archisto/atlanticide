using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class Detonator : MonoBehaviour
    {
        public DestructibleGameObject destructibleGameObject;
        public bool detonate = false;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (detonate)
            {
                destructibleGameObject.Destruction();
                detonate = false;
            }
        }
    }
}