using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// An object that can be destroyed with the link beam.
    /// </summary>
    public class BeamDestructible : LevelObject
    {
        [SerializeField, Range(1f, 30f)]
        private float _toughness = 10f;

        [SerializeField]
        private float _hitInterval = 0.3f;

        private Timer _hitTimer;
        private float _toughnessLeft; 
        private bool _isDestroyed;

        public float ToughnessRatio
        {
            get { return _toughnessLeft / _toughness; }
        }

        public bool MaxToughness
        {
            get { return _toughnessLeft == _toughness; }
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _toughnessLeft = _toughness;
            _hitTimer = new Timer(_hitInterval, true);
        }

        protected override void UpdateObject()
        {
            if (_hitTimer.Active)
            {
                if (_hitTimer.Check())
                {
                    _hitTimer.Reset();
                }
            }

            base.UpdateObject();
        }

        public void TryDestroy(float collectStrength)
        {
            if (!_isDestroyed && !_hitTimer.Active)
            {
                //Debug.Log("collectStrength: " + collectStrength);
                _toughnessLeft -= collectStrength;
                if (_toughnessLeft <= 0f)
                {
                    _toughnessLeft = 0f;
                    Destroy();
                }
                else
                {
                    _hitTimer.Activate();
                }
            }
        }

        public void TryDestroyInstant(float collectStrength)
        {
            if (collectStrength >= _toughness)
            {
                _toughnessLeft = 0f;
                Destroy();
            }
        }

        public void Destroy()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Resets the pickup.
        /// </summary>
        public override void ResetObject()
        {
            _toughnessLeft = _toughness;
            _hitTimer.Reset();
            gameObject.SetActive(true);
            base.ResetObject();
        }

        /// <summary>
        /// Draws gizmos.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!_isDestroyed)
            {
                Color color = (MaxToughness ? Color.green : Color.yellow);
                Utils.DrawProgressBarGizmo(transform.position + Vector3.back * 1f,
                    ToughnessRatio, color, Color.black);
            }
        }
    }
}
