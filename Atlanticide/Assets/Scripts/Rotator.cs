using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class Rotator : MonoBehaviour
    {
        public float _rotationSpeed;
        public Vector3 _rotationVector3;

        // Update is called once per frame
        void Update()
        {
            transform.RotateAround(transform.position, transform.TransformDirection(Vector3.left), _rotationSpeed * Time.deltaTime);
        }
    }
}