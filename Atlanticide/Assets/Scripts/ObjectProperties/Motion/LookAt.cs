using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class LookAt : MonoBehaviour
    {
        public GameObject targetObject;
        public Vector3 targetDirection = Vector3.forward;

        [SerializeField]
        private bool _lockX;

        [SerializeField]
        private bool _lockY;

        [SerializeField]
        private bool _lockZ;

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (!World.Instance.GamePaused)
            {
                UpdateRotation();
            }
        }

        private void UpdateRotation()
        {
            Vector3 newRotation;
            if (targetObject != null)
            {
                newRotation = GetLookRotationAtObject();
            }
            else if (targetDirection != Vector3.zero)
            {
                newRotation = GetLookRotationAtDirection();
            }
            else
            {
                return;
            }

            if (_lockX)
            {
                newRotation.x = transform.rotation.eulerAngles.x;
            }
            if (_lockY)
            {
                newRotation.y = transform.rotation.eulerAngles.y;
            }
            if (_lockZ)
            {
                newRotation.z = transform.rotation.eulerAngles.z;
            }

            transform.rotation = Quaternion.Euler(newRotation);
        }

        private Vector3 GetLookRotationAtObject()
        {
            Vector3 direction = targetObject.transform.position - transform.position;
            return Quaternion.LookRotation(direction, Vector3.up).eulerAngles;
        }

        private Vector3 GetLookRotationAtDirection()
        {
            return Quaternion.LookRotation(targetDirection, Vector3.up).eulerAngles;
        }
    }
}
