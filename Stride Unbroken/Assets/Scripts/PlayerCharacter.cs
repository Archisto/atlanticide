using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StrideUnbroken
{
    public class PlayerCharacter : MonoBehaviour
    {
        [SerializeField]
        private float _bounceHeight;

        [SerializeField]
        private float _speed;

        [SerializeField]
        private Slider _energyBar;

        [SerializeField, Range(0.01f, 2f)]
        private float _energyDrainSpeed;

        [SerializeField, Range(0.01f, 2f)]
        private float _energyRechargeSpeed;

        [SerializeField, Range(0.01f, 1f)]
        private float _minRechargedEnergy;

        [SerializeField]
        private CameraController _camera;

        [SerializeField]
        private GameObject _pointShadow;

        private float _elapsedTime;
        private float _defaultBounceHeight;
        private bool _bouncing;
        private float _startY;
        private float _groundY;
        private float _pointShadowY;
        private bool _doubleTempo;
        private bool _outOfEnergy;
        private float _energy = 1;

        /// <summary>
        /// The bouncing tempo.
        /// </summary>
        private float Tempo
        {
            get
            {
                return GameManager.Instance.Tempo / (_doubleTempo ? 2 : 1);
            }
        }

        /// <summary>
        /// Activates the double tempo power.
        /// </summary>
        private void ActivateDoubleTempo()
        {
            _doubleTempo = true;
            _bounceHeight = _defaultBounceHeight / 2;
        }

        /// <summary>
        /// Deactivates the double tempo power.
        /// </summary>
        private void DeactivateDoubleTempo()
        {
            _doubleTempo = false;
            _bounceHeight = _defaultBounceHeight;
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _groundY = transform.position.y;
            _pointShadowY = _pointShadow.transform.position.y;
            _defaultBounceHeight = _bounceHeight;
            _bouncing = true;
            StartBounce();
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (_bouncing)
            {
                UpdateBounce();
            }

            UpdatePointShadow();
            UpdateEnergy();
        }

        /// <summary>
        /// Starts a bounce.
        /// </summary>
        private void StartBounce()
        {
            _startY = _groundY;
            _elapsedTime = 0;
        }

        /// <summary>
        /// Ends a bounce.
        /// </summary>
        private void EndBounce()
        {
            if (_bouncing)
            {
                StartBounce();
            }
        }

        /// <summary>
        /// Updates bouncing.
        /// </summary>
        private void UpdateBounce()
        {
            float ratio = _elapsedTime / Tempo;
            _elapsedTime += Time.deltaTime;

            Vector3 newPosition = transform.position;
            newPosition.y = _groundY + Mathf.Sin(ratio * Mathf.PI) * _bounceHeight;

            if (ratio >= 1f)
            {
                newPosition.y = _groundY;
                EndBounce();
            }

            transform.position = newPosition;
        }

        /// <summary>
        /// Updates the point shadow's position.
        /// </summary>
        private void UpdatePointShadow()
        {
            Vector3 newPosition = transform.position;
            newPosition.y = _pointShadowY;
            _pointShadow.transform.position = newPosition;
        }

        /// <summary>
        /// Updates the player's energy.
        /// </summary>
        private void UpdateEnergy()
        {
            // Drain
            if (_doubleTempo)
            {
                if (_energy > 0)
                {
                    _energy -= _energyDrainSpeed * Time.deltaTime;
                    if (_energy <= 0)
                    {
                        _energy = 0;
                        _outOfEnergy = true;
                    }

                    _energyBar.value = _energy;
                }
            }
            // Recharge
            else if (_energy < 1)
            {
                _energy += _energyRechargeSpeed * Time.deltaTime;

                if (_outOfEnergy && _energy >= _minRechargedEnergy)
                {
                    _outOfEnergy = false;
                }
                if (_energy > 1)
                {
                    _energy = 1;
                }

                _energyBar.value = _energy;
            }
        }

        /// <summary>
        /// Moves the player character.
        /// </summary>
        /// <param name="direction">The moving direction</param>
        public void MoveInput(Vector3 direction)
        {
            Vector3 newPosition = transform.position;
            newPosition.x += direction.x * _speed * Time.deltaTime;
            newPosition.z += direction.y * _speed * Time.deltaTime;

            transform.position = newPosition;
        }

        /// <summary>
        /// Activates or deactivates the double tempo power.
        /// </summary>
        /// <param name="activate">Should the double tempo
        /// power be activated</param>
        public void DoubleTempoInput(bool activate)
        {
            if (_outOfEnergy || (!activate && _doubleTempo))
            {
                DeactivateDoubleTempo();
            }
            else if (activate && !_doubleTempo)
            {
                ActivateDoubleTempo();
            }
        }
    }
}
