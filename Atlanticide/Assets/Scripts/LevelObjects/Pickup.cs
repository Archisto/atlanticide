using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// A score pickup that is collected by walking over it.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Pickup : LevelObject
    {
        [SerializeField]
        private int _score = 100;

        [SerializeField]
        private bool _bigScoreGain;

        [SerializeField]
        private MoveToBeamActivator _moveToBeam;

        protected PickupExpansion _expansion;
        private int _charLayer;

        public bool IsCollected { get; protected set; }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected virtual void Start()
        {
            _expansion = GetComponent<PickupExpansion>();
            _charLayer = LayerMask.NameToLayer("GameCharacter");
            _defaultPosition = transform.position;

            if (_moveToBeam != null)
            {
                _moveToBeam.Init(transform);
            }

            ResetObject();
        }

        protected override void UpdateObject()
        {
            if (_moveToBeam != null && _moveToBeam.BeamReached)
            {
                Collect(null);
            }

            base.UpdateObject();
        }

        /// <summary>
        /// Handles colliding with the player characters.
        /// </summary>
        /// <param name="collision">The collision</param>
        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (!IsCollected && collision.gameObject.layer == _charLayer)
            {
                PlayerCharacter pc = collision.gameObject.
                    GetComponentInParent<PlayerCharacter>();
                if (pc != null)
                {
                    Collect(pc);
                }
            }
        }

        /// <summary>
        /// Gives score and destroys the pickup.
        /// </summary>
        /// <param name="character">A player character</param>
        public virtual void Collect(PlayerCharacter character)
        {
            GameManager.Instance.CollectScorePickup(_score, _bigScoreGain);
            IsCollected = true;

            if (_expansion != null)
            {
                _expansion.OnPickupCollected();
            }

            DestroyObject();
        }

        /// <summary>
        /// Destroys the pickup.
        /// </summary>
        public override void DestroyObject()
        {
            if (_moveToBeam != null)
            {
                _moveToBeam.ResetMoveToBeam();
                //_moveToBeam.stayUpright = false;
            }

            gameObject.SetActive(false);
            base.DestroyObject();
        }

        public void SetDefaultPosition(Vector3 position)
        {
            _defaultPosition = position;
        }

        /// <summary>
        /// Resets the pickup.
        /// </summary>
        public override void ResetObject()
        {
            IsCollected = false;
            if (_moveToBeam != null)
            {
                _moveToBeam.ResetMoveToBeam();
                _moveToBeam.stayUpright = true;
            }
            SetToDefaultPosition();
            gameObject.SetActive(true);
            base.ResetObject();
        }
    }
}
