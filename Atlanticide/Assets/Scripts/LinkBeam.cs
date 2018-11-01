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
        [SerializeField]
        private float _maxStrengthLength = 5f;

        [SerializeField]
        private float _strengthScaleLength = 5f;

        [Header("Debug")]
        public GameObject _target;
        public float _strengthRatio;

        private PlayerCharacter _player;
        private LineRenderer _lr;

        private Color _strongColor = Color.blue;
        private Color _weakColor = Color.red;

        public bool Active { get; private set; }

        public float BeamLength { get; private set; }

        public float StrengthRatio
        {
            get { return _strengthRatio; }
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        public void Init(PlayerCharacter player)
        {
            _player = player;
            _lr = GetComponent<LineRenderer>();
            Activate(false, null);
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (Active)
            {
                _lr.SetPosition(0, transform.position);
                _lr.SetPosition(1, _target.transform.position);
                BeamLength = Vector3.Distance(_lr.GetPosition(0), _lr.GetPosition(1));

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

                _lr.startColor = color;
                _lr.endColor = color;
            }
        }

        /// <summary>
        /// Activates or deactivates the object.
        /// </summary>
        public void Activate(bool activate, GameObject target)
        {
            if (activate && target != null)
            {
                Active = true;
                _target = target;
                _lr.SetPosition(1, _target.transform.position);
            }
            else
            {
                Active = false;
                _target = null;
                BeamLength = 0f;
            }

            _lr.enabled = Active;
        }

        /// <summary>
        /// Resets the object.
        /// </summary>
        public void ResetLink()
        {
            Activate(false, null);
        }
    }
}
