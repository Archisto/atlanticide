using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class Pushable : LevelObject
    {
        [SerializeField]
        private bool _cardinalDirsOnly = true;

        [SerializeField]
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
            Vector3 movement = _pushDirection *
                World.Instance.pushSpeed * World.Instance.DeltaTime;
            movement.y = 0f;

            // TODO: Check for wall collisions

            transform.position += movement;
        }

        private bool PlayerCanPush(PlayerCharacter character)
        {
            return (character!= null && !character.IsDead &&
                    !character.IsImmobile && !character.Climbing);
        }

        /// <summary>
        /// Handles colliding with the player characters.
        /// </summary>
        /// <param name="collision">The collision</param>
        protected virtual void OnCollisionStay(Collision collision)
        {
            if (!IsBeingPushed)
            {
                PlayerCharacter pc = collision.gameObject.GetComponent<PlayerCharacter>();
                if (PlayerCanPush(pc))
                {
                    _pc = pc;
                    _pushDirection = (transform.position - collision.transform.position).normalized;
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

                    _pc.StartPush(this);
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
