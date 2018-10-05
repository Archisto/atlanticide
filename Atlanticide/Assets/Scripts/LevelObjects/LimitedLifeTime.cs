using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// Destroys the level object this is attached to when the time is up.
    /// </summary>
    public class LimitedLifeTime : LevelObjectExpansion
    {
        [SerializeField]
        private float _lifeTime = 10f;

        public float _elapsedTime;
        public float _ratio;
        public bool _isDestroyed;

        public override void OnObjectUpdated()
        {
            if (!_isDestroyed && _lifeTime > 0f)
            {
                _elapsedTime += World.Instance.DeltaTime;
                _ratio = (_elapsedTime / _lifeTime);
                if (_ratio >= 1f)
                {
                    _ratio = 1f;
                    _isDestroyed = true;
                    _levelObj.DestroyObject();
                }
            }
        }

        public override void OnObjectDestroyed()
        {
            _isDestroyed = true;
            UpdateObjectActiveState();
        }

        public override void OnObjectReset()
        {
            _isDestroyed = false;
            _ratio = 0f;
            _elapsedTime = 0f;
            UpdateObjectActiveState();
        }

        private void UpdateObjectActiveState()
        {
            if (gameObject.activeSelf == _isDestroyed)
            {
                gameObject.SetActive(!_isDestroyed);
            }
        }

        /// <summary>
        /// Draws gizmos.
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position +
                Vector3.up * 1.5f, 0.5f * (1 - _ratio));
        }
    }
}
