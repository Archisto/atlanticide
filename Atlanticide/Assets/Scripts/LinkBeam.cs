using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// A link beam from a player to the other.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class LinkBeam : MonoBehaviour
    {
        private const string WallKey = "Wall";

        [Header("STRENGTH PROPERTIES")]

        [SerializeField]
        private float _zeroRatioStrength = 0.5f;

        [SerializeField]
        private float _oneRatioStrength = 2f;

        [SerializeField]
        private float _maxStrengthLength = 5f;

        [SerializeField]
        private float _strengthScaleLength = 5f;

        [Header("BEAM PROPERTIES")]

        [SerializeField]
        private float _minLength = 0.5f;

        [SerializeField]
        private float _pulseStrength = 1.5f;

        [SerializeField]
        private float _pulseCooldownTime = 1f;

        [SerializeField]
        private float _wallHitShutdownTime = 1f;

        [SerializeField]
        private Vector3 _hitboxStartPoint;

        [SerializeField]
        private Vector3 _hitboxExtents = new Vector3(0.1f, 0.3f, 0f);

        [SerializeField]
        private LayerMask _layerMask;

        [Header("DEBUG")]

        public bool _useLineRenderer;
        public float _strengthRatio;

        public ILinkTarget _target;
        public List<ILinkInteractable> linkInteractables;

        private LineRenderer _lr;
        private Timer _pulseCooldownTimer;
        private Timer _shutdownTimer;
        private Color _color;
        private Color _strongColor = Color.blue;
        private Color _weakColor = Color.red;
        private Color _shutdownWarningColor = Color.yellow;

        public bool Active { get; private set; }

        public PlayerCharacter Player { get; private set; }

        public float BeamLength { get; private set; }

        public Vector3 StartPosition { get; private set; }

        public Vector3 TargetPosition { get; private set; }

        public Vector3 BeamCenter { get; private set; }

        public Vector3 BeamDirection { get; private set; }

        public float StrengthRatio
        {
            get { return _strengthRatio; }
        }

        public float Strength
        {
            get
            {
                return _zeroRatioStrength + StrengthRatio *
                    (_oneRatioStrength - _zeroRatioStrength);
            }
        }

        public delegate bool Activation(bool activate);
        private Activation stateChangeCallback;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        public void Init(PlayerCharacter player, Activation shieldActivation)
        {
            Player = player;
            linkInteractables = new List<ILinkInteractable>();
            _lr = GetComponent<LineRenderer>();
            _lr.enabled = false;
            _pulseCooldownTimer = new Timer(_pulseCooldownTime, true);
            _shutdownTimer = new Timer(_wallHitShutdownTime, true);
            stateChangeCallback = shieldActivation;
        }

        /// <summary>
        /// Activates or deactivates the object.
        /// </summary>
        public void Activate(bool activate, ILinkTarget target = null)
        {
            if (activate && target != null)
            {
                // TODO: Player must face the target before activating the beam
                //float playerWrongRotOffset = 1f; 
                UpdateBeamEnds(target);
                RaycastHit[] hits = GetRaycastHits();
                if (BeamLength > _minLength &&
                    (hits.Length == 0 || !CheckIfWallHit(hits)))
                {
                    Active = true;
                    //Player.IsLinkTarget = false;
                    _target = target;
                    _target.IsLinkTarget = true;
                    _target.LinkedLinkBeam = this;
                    _lr.SetPosition(1, TargetPosition);
                    SFXPlayer.Instance.Play(Sound.Cyclops_Laser_Start);

                    if (_target.LinkBeam != null)
                    {
                        _target.LinkBeam.ActivatePlayerShield(true);
                    }
                }
            }
            else
            {
                Active = false;
                BeamLength = 0f;
                _pulseCooldownTimer.Reset();
                _shutdownTimer.Reset();
                linkInteractables.Clear();

                if (_target != null)
                {
                    if (_target.LinkBeam != null)
                    {
                        _target.LinkBeam.ActivatePlayerShield(false);
                    }

                    _target.IsLinkTarget = false;
                    _target.LinkedLinkBeam = null;
                    _target = null;
                }

                SFXPlayer.Instance.Play(Sound.Cyclops_Shortcircuit);
            }

            ActivatePlayerShield(Active);

            if (_useLineRenderer)
            {
                _lr.enabled = Active;
            }
        }

        /// <summary>
        /// Activates or deactivates the shield of
        /// the player who owns this link beam.
        /// </summary>
        /// <param name="activate">Should the player's
        /// shield be activated</param>
        public void ActivatePlayerShield(bool activate)
        {
            if (stateChangeCallback != null)
            {
                stateChangeCallback.Invoke(activate);
            }
        }

        public bool Pulse()
        {
            if (!_pulseCooldownTimer.Active)
            {
                SFXPlayer.Instance.Play(Sound.Shield_Bash);
                _pulseCooldownTimer.Activate();
                if (Active)
                {
                    GivePulseToLinkInteractables(true);
                }
                else if (Player.IsLinkTarget)
                {
                    Player.LinkedLinkBeam.
                        GivePulseToLinkInteractables(false);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public void GivePulseToLinkInteractables(bool towardsTarget)
        {
            foreach (ILinkInteractable li in linkInteractables)
            {
                li.GivePulse(this, (towardsTarget ? 1f : -1f) * _pulseStrength);
            }
        }

        #region Updating

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (Active)
            {
                UpdateBeamEnds(_target);
                UpdateStrength();
                UpdateHits();

                if (BeamLength < _minLength ||
                    _shutdownTimer.Check())
                {
                    Activate(false);
                }
            }

            if (Player.BeamOn)
            {
                if (_pulseCooldownTimer.Check())
                {
                    _pulseCooldownTimer.Reset();
                }
            }
        }

        private void UpdateBeamEnds(ILinkTarget target)
        {
            StartPosition = transform.position;
            TargetPosition = target.LinkObject.transform.position;
            _lr.SetPosition(0, StartPosition);
            _lr.SetPosition(1, TargetPosition);
            BeamLength = Vector3.Distance(StartPosition, TargetPosition);
            BeamCenter = (StartPosition + TargetPosition) * 0.5f;
        } 

        private void UpdateStrength()
        {
            _color = _strongColor;
            if (BeamLength > _maxStrengthLength)
            {
                if (BeamLength < _maxStrengthLength + _strengthScaleLength)
                {
                    _strengthRatio = 1 - ((BeamLength - _maxStrengthLength) / _strengthScaleLength);
                    _strengthRatio = Mathf.Clamp01(_strengthRatio);
                    _color = _strengthRatio * _strongColor + (1 - _strengthRatio) * _weakColor;
                    _color.a = 1f;
                }
                else
                {
                    _strengthRatio = 0f;
                    _color = _weakColor;
                }
            }
            else
            {
                _strengthRatio = 1f;
            }

            if (_shutdownTimer.Active)
            {
                float shutdownRatio = _shutdownTimer.GetRatio();
                _color = shutdownRatio * _shutdownWarningColor +
                    (1 - shutdownRatio) * _color;
            }

            _lr.startColor = _color;
            _lr.endColor = _color;
        }

        #endregion Updating

        #region Hitting Objects

        private void UpdateHits()
        {
            if (BeamLength <= 0f)
            {
                return;
            }

            RaycastHit[] hits = GetRaycastHits();
            if (hits.Length > 0)
            {
                HandleHits(hits);
            }
            else if (_shutdownTimer.Active)
            {
                _shutdownTimer.Reset();
            }
        }

        private RaycastHit[] GetRaycastHits()
        {
            BeamDirection = (TargetPosition - StartPosition).normalized;
            Quaternion orientation = Quaternion.
                LookRotation(TargetPosition - StartPosition, Vector3.up);
            RaycastHit[] hits = Physics.BoxCastAll(transform.position + _hitboxStartPoint,
                 _hitboxExtents, BeamDirection, orientation, BeamLength, _layerMask);
            return hits;
        }

        private void HandleHits(RaycastHit[] hits)
        {
            bool wallHit = false;
            foreach (RaycastHit hit in hits)
            {
                Collider collider = hit.collider;
                if (collider != null)
                {
                    //Debug.Log("hit.collider: " + hit.collider.transform.name);
                    Pickup pickup = collider.transform.GetComponent<Pickup>();
                    ILinkInteractable linkInteractable = null;
                    if (pickup != null)
                    {
                        HandlePickupHit(pickup);
                    }
                    else
                    {
                        linkInteractable = collider.transform.GetComponent<ILinkInteractable>();
                        if (linkInteractable != null)
                        {
                            HandleLinkInteractableHit(linkInteractable);
                        }
                    }

                    if (pickup == null && linkInteractable == null && !wallHit)
                    {
                        if (CheckIfWallHit(collider))
                        {
                            if (!_shutdownTimer.Active)
                            {
                                _shutdownTimer.Activate();
                            }

                            wallHit = true;
                        }
                    }
                }
            }

            if (!wallHit && _shutdownTimer.Active)
            {
                _shutdownTimer.Reset();
            }
        }

        private void HandlePickupHit(Pickup pickup)
        {
            ToughPickup tp = pickup as ToughPickup;
            if (tp != null)
            {
                tp.TryInteract(this);
                //tp.TryCollectInstant(_player, CollectStrength);
            }
            else
            {
                pickup.Collect(Player);
            }
        }

        private void HandleLinkInteractableHit(ILinkInteractable linkInteractable)
        {
            linkInteractable.TryInteract(this);
        }

        private bool CheckIfWallHit(Collider collider)
        {
            LinkDestructible bd = collider.transform.
                GetComponent<LinkDestructible>();
            return bd == null && WallKey.Equals
                (LayerMask.LayerToName(collider.gameObject.layer));
        }

        private bool CheckIfWallHit(RaycastHit[] hits)
        {
            foreach (RaycastHit hit in hits)
            {
                Collider collider = hit.collider;
                if (collider != null)
                {
                    LinkDestructible bd = collider.transform.
                        GetComponent<LinkDestructible>();
                    if (bd == null && WallKey.Equals
                        (LayerMask.LayerToName(collider.gameObject.layer)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion Hitting Objects

        /// <summary>
        /// Resets the object.
        /// </summary>
        public void ResetLinkBeam()
        {
            Activate(false);
            _pulseCooldownTimer.Reset();
            _shutdownTimer.Reset();
            linkInteractables.Clear();
        }

        private void OnDrawGizmos()
        {
            if (Active)
            {
                // Simple beam and beam center lines
                Gizmos.color = _color;
                Gizmos.DrawLine(BeamCenter, BeamCenter + Vector3.up * 1f);
                Gizmos.DrawLine(StartPosition, TargetPosition);

                // Hitbox
                DrawHitboxGizmo();
            }
        }

        private void DrawHitboxGizmo()
        {
            Gizmos.color = Color.green;
            Vector3 trueHitboxStartPoint = transform.position + _hitboxStartPoint;
            Vector3 hitboxTargetPoint =
                trueHitboxStartPoint + BeamDirection * BeamLength;
            Utils.DrawRotatedBoxGizmo(trueHitboxStartPoint, hitboxTargetPoint,
                _hitboxExtents, transform);
        }
    }
}
