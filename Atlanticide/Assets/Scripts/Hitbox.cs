using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class Hitbox : MonoBehaviour
    {
        public enum CollisionType
        {
            Enter = 0,
            Stay = 1,
            Exit = 2
        }

        [SerializeField]
        private bool _active = true;

        [SerializeField]
        private CollisionType _type;

        // Check if an object is in a layer
        // covered by the layermask like this:
        // layermask == (layermask | (1 << layer))
        public LayerMask mask;

        private Collision _collision;
        private int _maxWaitFrames = 3;
        private int _waitFrames;

        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }

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
                !World.Instance.GamePaused &&
                mask == (mask | (1 << collision.gameObject.layer)))
            {
                Collision = collision;
                //Debug.Log("Hit enter");
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            //Debug.Log("hit layer: " + collision.gameObject.layer);
            if (_type == CollisionType.Stay &&
                !World.Instance.GamePaused &&
                mask == (mask | (1 << collision.gameObject.layer)))
            {
                Collision = collision;
                //Debug.Log("Hit stay");
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (_type == CollisionType.Exit &&
                !World.Instance.GamePaused &&
                mask == (mask | (1 << collision.gameObject.layer)))
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
