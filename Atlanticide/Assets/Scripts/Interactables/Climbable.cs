﻿using System.Collections;
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

        public Vector3 TopPointWorldSpace
        {
            get { return _topPoint + transform.position; }
        }

        public Vector3 BottomPointWorldSpace
        {
            get { return _bottomPoint + transform.position; }
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
        }

        public void SetPoints(Vector3 globalTopPoint, Vector3 globalBottomPoint)
        {
            _topPoint = globalTopPoint - transform.position;
            _bottomPoint = globalBottomPoint - transform.position;
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
            return Utils.Ratio(position.y, BottomPointWorldSpace.y, TopPointWorldSpace.y);
        }

        public Vector3 GetPositionOnClimbable(float climbProgress)
        {
            return Vector3.Lerp(BottomPointWorldSpace, TopPointWorldSpace, climbProgress);
        }

        private void Reset()
        {
            _topPoint = Vector3.up * 1;
            _bottomPoint = Vector3.up * -1;
        }
    }
}