using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class Climbable : MonoBehaviour
    {
        /// <summary>
        /// The top point (local position)
        /// </summary>
        [SerializeField]
        protected Vector3 _topPoint;

        /// <summary>
        /// The bottom point (local position)
        /// </summary>
        [SerializeField]
        protected Vector3 _bottomPoint;

        public Vector3 TopPoint { get; protected set; }

        public Vector3 BottomPoint { get; protected set; }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            TopPoint = transform.position + _topPoint;
            BottomPoint = transform.position + _bottomPoint;
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {

        }

        /// <summary>
        /// Handles colliding with the player characters.
        /// </summary>
        /// <param name="collision">The collision</param>
        protected virtual void OnCollisionEnter(Collision collision)
        {
            PlayerCharacter pc = collision.gameObject.GetComponent<PlayerCharacter>();
            if (pc != null && !pc.Climbing)
            {
                pc.StartClimb(this);
            }
        }

        public float GetClimbProgress(Vector3 position)
        {
            return Utils.Ratio(position.y, BottomPoint.y, TopPoint.y);
        }

        public Vector3 GetPositionOnClimbable(float climbProgress)
        {
            return Vector3.Lerp(BottomPoint, TopPoint, climbProgress);
        }
    }
}
