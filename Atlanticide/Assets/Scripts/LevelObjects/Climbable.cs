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
        private Vector3 _topPoint;

        /// <summary>
        /// The bottom point (local position)
        /// </summary>
        [SerializeField]
        public Vector3 _bottomPoint;

        [SerializeField]
        public Vector2 _inputForwardVector = new Vector2(0f, 1f);

        public Vector3 TopPointWorldSpace
        {
            get { return _topPoint + transform.position; }
        }

        public Vector3 BottomPointWorldSpace
        {
            get { return _bottomPoint + transform.position; }
        }

        public Vector2 InputForwardVector
        {
            get { return _inputForwardVector; }
        }

        public void SetPoints(Vector3 globalTopPoint, Vector3 globalBottomPoint)
        {
            _topPoint = globalTopPoint - transform.position;
            _bottomPoint = globalBottomPoint - transform.position;
        }

        /// <summary>
        /// Handles colliding with a player character.
        /// </summary>
        /// <param name="collision">The collision</param>
        protected virtual void OnCollisionEnter(Collision collision)
        {
            foreach (ContactPoint cp in collision.contacts)
            {
                PlayerCharacter pc = cp.otherCollider.gameObject.
                    GetComponentInParent<PlayerCharacter>();
                if (pc != null)
                {
                    pc.StartClimb(this);
                    break;
                }
            }
        }

        public float GetClimbProgress(Vector3 position)
        {
            return Utils.Ratio(position.y, BottomPointWorldSpace.y, TopPointWorldSpace.y);
        }

        public Vector3 GetPositionOnClimbable(float climbProgress)
        {
            return Vector3.Lerp(BottomPointWorldSpace, TopPointWorldSpace, climbProgress);
        }

        public bool ForwardDirection(Vector2 direction)
        {
            return (Vector3.Angle(direction, _inputForwardVector) < 80f);
        }

        private void Reset()
        {
            _topPoint = Vector3.up * 1;
            _bottomPoint = Vector3.up * -1;
        }
    }
}
