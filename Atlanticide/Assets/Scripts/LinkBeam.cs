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

        [SerializeField]
        private float _zeroRatioStrength = 0.5f;

        [SerializeField]
        private float _oneRatioStrength = 2f;

        [SerializeField]
        private float _minLength = 0.5f;

        [SerializeField]
        private float _maxStrengthLength = 5f;

        [SerializeField]
        private float _strengthScaleLength = 5f;

        [SerializeField]
        private float _wallHitShutdownTime = 1f;

        [Header("BEAM PROPERTIES")]

        [SerializeField]
        private Vector3 _beamStartPoint;

        [SerializeField]
        private Vector3 _beamExtents = new Vector3(0.1f, 0.3f, 0f);

        [SerializeField]
        private LayerMask _layerMask;

        [Header("DEBUG")]

        public ILinkTarget _target;
        public float _strengthRatio;

        private PlayerCharacter _player;
        private LineRenderer _lr;
        private Timer _shutdownTimer;
        private Color _strongColor = Color.blue;
        private Color _weakColor = Color.red;
        private Color _shutdownWarningColor = Color.yellow;

        public bool Active { get; private set; }

        public float BeamLength { get; private set; }

        public Vector3 StartPosition { get; private set; }

        public Vector3 TargetPosition { get; private set; }

        public Vector3 BeamCenter { get; private set; }

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

        /// <summary>
        /// Initializes the object.
        /// </summary>
        public void Init(PlayerCharacter player)
        {
            _player = player;
            _lr = GetComponent<LineRenderer>();
            _shutdownTimer = new Timer(_wallHitShutdownTime, true);
            Activate(false);
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
                    _target = target;
                    _target.IsLinkTarget = true;
                    _target.LinkedLinkBeam = this;
                    _lr.SetPosition(1, _target.LinkObject.transform.position);
                }
            }
            else
            {
                Active = false;
                BeamLength = 0f;
                _shutdownTimer.Reset();

                if (_target != null)
                {
                    _target.IsLinkTarget = false;
                    _target.LinkedLinkBeam = null;
                    _target = null;
                }
            }

            _lr.enabled = Active;
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
            Color color = _strongColor;
            if (BeamLength > _maxStrengthLength)
            {
                if (BeamLength < _maxStrengthLength + _strengthScaleLength)
                {
                    _strengthRatio = 1 - ((BeamLength - _maxStrengthLength) / _strengthScaleLength);
                    _strengthRatio = Mathf.Clamp01(_strengthRatio);
                    color = _strengthRatio * _strongColor + (1 - _strengthRatio) * _weakColor;
                    color.a = 1f;
                }
                else
                {
                    _strengthRatio = 0f;
                    color = _weakColor;
                }
            }
            else
            {
                _strengthRatio = 1f;
            }

            if (_shutdownTimer.Active)
            {
                float shutdownRatio = _shutdownTimer.GetRatio();
                color = shutdownRatio * _shutdownWarningColor +
                    (1 - shutdownRatio) * color;
            }

            _lr.startColor = color;
            _lr.endColor = color;
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
            Vector3 direction = (TargetPosition - StartPosition).normalized;
            Quaternion orientation = Quaternion.
                LookRotation(TargetPosition - StartPosition, Vector3.up);
            RaycastHit[] hits = Physics.BoxCastAll(transform.position + _beamStartPoint,
                 _beamExtents, direction, orientation, BeamLength, _layerMask);
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
                    BeamDestructible destructible = null;
                    if (pickup != null)
                    {
                        HandlePickupHit(pickup);
                    }
                    else
                    {
                        destructible = collider.transform.GetComponent<BeamDestructible>();
                        if (destructible != null)
                        {
                            HandleDestructibleHit(destructible);
                        }
                    }

                    if (pickup == null && destructible == null && !wallHit)
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
                tp.TryCollect(_player, Strength);
                //tp.TryCollectInstant(_player, CollectStrength);
            }
            else
            {
                pickup.Collect(_player);
            }
        }

        private void HandleDestructibleHit(BeamDestructible destructible)
        {
            destructible.TryDestroy(Strength);
        }

        private bool CheckIfWallHit(Collider collider)
        {
            BeamDestructible bd = collider.transform.
                GetComponent<BeamDestructible>();
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
                    BeamDestructible bd = collider.transform.
                        GetComponent<BeamDestructible>();
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
            _shutdownTimer.Reset();
        }

        private void OnDrawGizmos()
        {
            if (Active)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(BeamCenter, BeamCenter + Vector3.up * 1f);
            }
        }
    }
}
