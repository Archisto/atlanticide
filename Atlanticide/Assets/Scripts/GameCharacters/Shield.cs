using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class Shield : MonoBehaviour
    {
        private const string DefaultKey = "Default";
        private const string InteractableKey = "Interactable";
        private const string PlatformKey = "Platform";

        [SerializeField]
        private bool _stayInvisible = true;

        [SerializeField, Range(0.1f, 5f)]
        private float _openTime = 0.5f;

        [SerializeField, Range(0.1f, 5f)]
        private float _bashTime = 0.25f;

        [SerializeField, Range(0.1f, 5f)]
        private float _bashRecoveryTime = 0.5f;

        [SerializeField, Range(0.1f, 1f)]
        private float _playerSpeedModifier = 0.5f;

        private PlayerCharacter _player;
        private Transparency _tp; // testing
        private Vector3 _defaultPosition; // testing
        private Vector3 _raisedPosition; // testing
        private Quaternion _defaultRotation; // testing
        private Quaternion _raisedRotation; // testing
        private bool _updateOpen;
        private bool _updateBash;
        public float _openProgress;
        public float _bashProgress;
        private float _elapsedTime;

        /// <summary>
        /// Is the shield in an idle state.
        /// </summary>
        public bool IsIdle
        {
            get { return !Active && !_updateOpen && !_updateBash; }
        }

        /// <summary>
        /// Is the shield open and ready to block damage.
        /// </summary>
        public bool BlocksDamage
        {
            get { return Active && !_updateOpen && !_updateBash && !Raised; }
        }

        public bool Active { get; private set; }

        public bool BashActive { get; private set; }

        public bool Raised { get; private set; }

        public float PlayerSpeedModifier
        {
            get { return _playerSpeedModifier; }
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _player = GetComponentInParent<PlayerCharacter>();
            _defaultPosition = transform.localPosition;
            _raisedPosition = Vector3.up * ((_player.Size.y / 2f) + 0.2f);
            _defaultRotation = transform.localRotation;
            _raisedRotation = Quaternion.Euler(Vector3.zero);
            gameObject.layer = LayerMask.NameToLayer(DefaultKey);

            // Testing shield opening with transparency;
            // by default, the shield is invisible
            _tp = GetComponent<Transparency>();
            _tp.SetAlpha(_openProgress);
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (_updateBash)
            {
                UpdateShieldBash();
            }
            else if (_updateOpen)
            {
                UpdateOpenProgress();

                if (!_stayInvisible)
                {
                    _tp.SetAlpha(_openProgress);
                }
            }
        }

        private void UpdateOpenProgress()
        {
            _elapsedTime += World.Instance.DeltaTime;
            _openProgress = (Active ?
                _elapsedTime / _openTime : (_openTime - _elapsedTime) / _openTime);
            if (_elapsedTime >= _openTime)
            {
                _elapsedTime = 0f;
                _updateOpen = false;
                _openProgress = Mathf.Clamp01(_openProgress);
            }
        }

        private void UpdateShieldBash()
        {
            _elapsedTime += World.Instance.DeltaTime;
            float targetTime = (BashActive ? _bashTime : _bashRecoveryTime);
            _bashProgress = (BashActive ?
                _elapsedTime / targetTime : (targetTime - _elapsedTime) / targetTime);
            if (_elapsedTime >= targetTime)
            {
                _elapsedTime = 0f;
                if (BashActive)
                {
                    _bashProgress = 1f;
                    BashClimax();
                }
                else
                {
                    _bashProgress = 0f;
                    _updateBash = false;
                }
            }
        }

        private void BashClimax()
        {
            BashActive = false;
            World.Instance.ShieldBashing = false;
        }

        public bool Activate(bool activate)
        {
            if (Active != activate)
            {
                Active = activate;
                StartOpeningOrClosing();

                if (!activate)
                {
                    RaiseAboveHead(false);
                }

                gameObject.layer = LayerMask.NameToLayer
                    (activate ? InteractableKey : DefaultKey);
            }

            return Active;
        }

        public void ActivateInstantly(bool activate)
        {
            Active = activate;
            _updateOpen = false;
            _openProgress = (Active ? 1f : 0f);

            if (!_stayInvisible)
            {
                _tp.SetAlpha(_openProgress);
            }

            gameObject.layer = LayerMask.NameToLayer
                (activate ? InteractableKey : DefaultKey);
        }

        private void StartOpeningOrClosing()
        {
            _updateOpen = true;
            if (!_updateBash && _elapsedTime > 0f)
            {
                _elapsedTime = _openTime - _elapsedTime;
            }
        }

        public bool Bash()
        {
            if (BlocksDamage)
            {
                BashActive = true;
                _updateBash = true;
                _elapsedTime = 0f;
                World.Instance.ShieldBashing = true;

                SFXPlayer.Instance.Play(Sound.Shield_Bash);
                return true;
            }

            return false;
        }

        public void BashInstantly()
        {
            if (!BashActive &&
                BlocksDamage)
            {
                BashActive = true;
                BashClimax();

                SFXPlayer.Instance.Play(Sound.Shield_Bash);
            }
        }

        public void CancelBash()
        {
            BashActive = false;
            _updateBash = false;
            _bashProgress = 0f;
            _elapsedTime = 0f;
            World.Instance.ShieldBashing = false;
        }

        /// <summary>
        /// If <paramref name="raise"/> is true, raises the shield above the
        /// player character's head and creates a platform for the other player.
        /// If <paramref name="raise"/> is false, brings the shield back to it's
        /// normal position.
        /// </summary>
        /// <param name="raise">Should the shield be raised</param>
        /// <returns>Is the shield raised</returns>
        public bool RaiseAboveHead(bool raise)
        {
            if (Raised != raise)
            {
                Raised = raise;

                // Testing
                if (Raised)
                {
                    transform.localPosition = _raisedPosition;
                    transform.localRotation = _raisedRotation;
                    gameObject.layer = LayerMask.NameToLayer(PlatformKey);
                }
                else
                {
                    transform.localPosition = _defaultPosition;
                    transform.localRotation = _defaultRotation;
                    gameObject.layer = LayerMask.NameToLayer(InteractableKey);
                }
            }

            return Raised;
        }

        // called when shield is hit by object (DamageDealer)
        public void Hit()
        {

        }

        /// <summary>
        /// Resets the shield to its default state.
        /// </summary>
        public void ResetShield()
        {
            RaiseAboveHead(false);
            ActivateInstantly(false);
            CancelBash();
            _elapsedTime = 0f;
        }

        /// <summary>
        /// Draws gizmos.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (_updateBash)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position,
                    transform.position + transform.up * 3f * _bashProgress);
            }
        }
    }
}