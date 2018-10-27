using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Atlanticide.WaypointSystem;

namespace Atlanticide
{
    [RequireComponent(typeof(Activatable))]
    public class ActivatableTester : MonoBehaviour
    {
        [SerializeField]
        private bool  _printMessage;

        [SerializeField]
        private string message = "";

        private Activatable _activatable;
        private bool _activated;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _activatable = GetComponent<Activatable>();

            if (message.Length == 0)
            {
                message = _activatable.name + " was activated";
            }
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (_activatable.Activated != _activated)
            {
                _activated = _activatable.Activated;
                if (_activatable.Activated)
                {
                    if (_printMessage)
                    {
                        Debug.LogWarning(message);
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = (_activated ? Color.green : Color.black);
            Gizmos.DrawCube(transform.position, new Vector3(1.2f, 1.2f, 1.2f));
        }
    }
}
