using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class ObeliskDirectionalCollider : MonoBehaviour
    {
        public enum Type
        {
            West,
            East,
            North,
            South
        }

        private Obelisk obelisk;

        public Type type;

        // Use this for initialization
        void Start()
        {
            obelisk = transform.parent.GetComponent<Obelisk>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            obelisk.ObeliskDirectionalColliderCollision(type);
        }
    }
}