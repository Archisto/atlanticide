using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class HitBox : MonoBehaviour
    {
        public enum CollisionType
        {
            Enter = 0,
            Stay = 1,
            Exit = 2
        }

        [SerializeField]
        private CollisionType _type;

        public LayerMask mask;

        private Collision _collision;
        private int _maxWaitFrames = 3;
        private int _waitFrames;

        public Collision Collision
        {
            get
            {
                return _collision;
            }
            private set
            {
                if (value != null)
                {
                    _waitFrames = _maxWaitFrames;
                }

                _collision = value;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_type == CollisionType.Enter &&
                !World.Instance.GamePaused)
            {
                Collision = collision;
                //Debug.Log("Hit enter");
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (_type == CollisionType.Stay &&
                !World.Instance.GamePaused)
            {
                Collision = collision;
                //Debug.Log("Hit stay");
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (_type == CollisionType.Exit &&
                !World.Instance.GamePaused)
            {
                Collision = collision;
                //Debug.Log("Hit exit");
            }
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (!World.Instance.GamePaused &&
                Collision != null)
            {
                // Makes sure that the object using this has time
                // to know about the collision before it's made null.
                // This is achieved by waiting some frames so it wouldn't
                // matter which object's Update method is called first.
                if (_waitFrames > 0)
                {
                    _waitFrames--;
                }
                else
                {
                    // Tells the object using this
                    // that the collision is over
                    Collision = null;
                }
            }
        }
    }
}
