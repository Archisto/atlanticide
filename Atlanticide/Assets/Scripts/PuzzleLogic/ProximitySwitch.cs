using UnityEngine;

namespace Atlanticide
{
    public class ProximitySwitch : Switch
    {
        [SerializeField]
        private float _range = 3f;

        [SerializeField]
        private Transform _targetTransform;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            if (_targetTransform == null)
            {
                Debug.LogError(Utils.GetFieldNullString("Target transform"));
            }
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if ((!Activated || !_permanent) && _targetTransform != null)
            {
                CheckObjectProximity();
            }
        }

        /// <summary>
        /// Checks if the target object is within range
        /// and updates the switch's activation.
        /// </summary>
        private void CheckObjectProximity()
        {
            Activated =
                (Vector3.Distance(transform.position, _targetTransform.position) <= _range);
        }

        /// <summary>
        /// Draws gizmos.
        /// </summary>
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            Gizmos.DrawWireSphere(transform.position, _range);

            if (_targetTransform != null)
            {
                Gizmos.DrawLine(transform.position, _targetTransform.position);
            }
        }
    }
}
