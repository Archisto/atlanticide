using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class Pushable : LevelObject
    {
        [SerializeField]
        private bool _cardinalDirsOnly = true;

        [SerializeField, Range(0.01f, 10f)]
        private float _weight = 1f;

        private PlayerCharacter _pc;
        private Vector3 _pushDirection;
        private Vector3 _startPosition;
        private Vector3 _playerPushPosition;

        public bool IsBeingPushed
        {
            get { return _pc != null; }
        }

        public Vector3 PushDirection
        {
            get { return _pushDirection; }
        }

        public float Weight
        {
            get { return _weight; }
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _startPosition = transform.position;
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
        }

        public void Move()
        {
            // TODO: Player speed is the minimum speed

            Vector3 movement = (1f / _weight) * _pushDirection *
                World.Instance.pushSpeed * World.Instance.DeltaTime;
            movement.y = 0f;

            // TODO: Check for wall collisions

            transform.position += movement;
        }

        /// <summary>
        /// Handles colliding with the player characters.
        /// </summary>
        /// <param name="collision">The collision</param>
        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (!IsBeingPushed)
            {
                PlayerCharacter pc = collision.gameObject.GetComponent<PlayerCharacter>();
                if (pc != null && pc.IsAvailableForActions())
                {
                    _pc = pc;
                    _pushDirection = transform.position - collision.transform.position;
                    _pushDirection.y = 0;
                    _pushDirection.Normalize();
                    if (_cardinalDirsOnly)
                    {
                        if (Mathf.Abs(_pushDirection.x) > Mathf.Abs(_pushDirection.z))
                        {
                            _pushDirection = new Vector3(_pushDirection.x, 0, 0);
                        }
                        else
                        {
                            _pushDirection = new Vector3(0, 0, _pushDirection.z);
                        }
                    }

                    //_pc.StartPush(this);
                }
            }
        }

        public void EndPush()
        {
            if (IsBeingPushed)
            {
                _pc = null;
            }
        }

        public override void ResetObject()
        {
            transform.position = _startPosition;
        }
    }
}
