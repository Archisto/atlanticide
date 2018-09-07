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
        private float _bounceDistance;

        [SerializeField]
        private float _speed;

        [SerializeField]
        private float _turningSpeed;

        [SerializeField]
        private Slider _energyBar;

        [SerializeField, Range(0.01f, 2f)]
        private float _energyDrainSpeed;

        [SerializeField, Range(0.01f, 2f)]
        private float _energyRechargeSpeed;

        [SerializeField, Range(0.01f, 1f)]
        private float _minRechargedEnergy;

        private float _defaultBounceHeight;
        private bool _bouncing;
        private Vector3 _spawnPosition;
        private Vector3 _startposition;
        private float _groundY;
        private Vector3 _movingDirection;
        private Vector3 _savedInput;
        private Vector3 _characterSize;
        private float _distRatio;
        private bool _bounceSuccess;
        private bool _startBounceNextFrame;
        private bool _secondTick;
        private bool _doubleTempo;
        private bool _doubleTempoRequest;
        private bool _doubleTempoTick;
        private bool _halfTempoOffset;
        private bool _outOfEnergy;
        private float _energy = 1;
        private bool _isDead;
        private float _hitDist = 3f;
        private LayerMask _platformLayerMask;
        private InputController _input;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _groundY = transform.position.y;
            _spawnPosition = transform.position;
            _characterSize = GetComponent<Renderer>().bounds.size;
            _defaultBounceHeight = _bounceHeight;
            _bouncing = true;
            _platformLayerMask = LayerMask.GetMask("Platform");
            _input = FindObjectOfType<InputController>();
            Metronome.Instance.OnTick += HandleTickEvent;

            StartBounce();
        }

        /// <summary>
        /// Gets the tick ratio with possible modifiers.
        /// </summary>
        /// <returns>The usable tick ratio</returns>
        private float GetTickRatio()
        {
            // TODO

            if (!_doubleTempo)
            {
                if (!_halfTempoOffset)
                {
                    return Metronome.TickRatio;
                }
                else
                {
                    if (Metronome.TickRatio > 0.5f)
                    {
                        Debug.Log("A");
                        return Metronome.TickRatio - 0.5f;
                    }
                    else
                    {
                        Debug.Log("B");
                        return Metronome.TickRatio + 0.5f;
                    }
                }
            }
            else
            {
                float result = Metronome.TickRatio;

                if (Metronome.TickRatio > 0.5f)
                {
                    result = Metronome.TickRatio - 0.5f;

                    if (!_doubleTempoTick)
                    {
                        _doubleTempoTick = true;
                        _halfTempoOffset = true;
                        HandleTickEvent();
                    }
                }
                else if (_doubleTempoTick)
                {
                    _doubleTempoTick = false;
                }

                return result * 2;
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
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (_isDead)
            {
                return;
            }

            if (_bouncing)
            {
                if (_startBounceNextFrame)
                {
                    _startBounceNextFrame = false;
                    StartBounce();
                }

                UpdateBounce();
            }

            if (_bounceSuccess)
            {
                _bounceSuccess = false;
                _startBounceNextFrame = true;
            }

            UpdateEnergy();
        }

        /// <summary>
        /// Handles the player character's movement on each tick.
        /// </summary>
        private void HandleTickEvent()
        {
            if (!_isDead)
            {
                _halfTempoOffset = false; // TODO
                EndBounce();
            }
        }

        /// <summary>
        /// Starts a bounce.
        /// </summary>
        private void StartBounce()
        {
            _startposition = new Vector3(transform.position.x, _groundY, transform.position.z);
            Vector3 currentInput = _input.GetMovementInput();
            _movingDirection = currentInput;
            //_movingDirection = (currentInput == Vector3.zero ? _savedInput : currentInput);
            //_savedInput = Vector3.zero;
            _movingDirection = new Vector3(_movingDirection.x, 0, _movingDirection.y);

            Debug.LogFormat("X: {0}, Z: {1}", _movingDirection.x, _movingDirection.z);

            if (_outOfEnergy || (!_doubleTempoRequest && _doubleTempo))
            {
                DeactivateDoubleTempo();
            }
            else if (_doubleTempoRequest && !_doubleTempo)
            {
                ActivateDoubleTempo();
            }
        }

        /// <summary>
        /// Moves the player character.
        /// </summary>
        /// <param name="direction">The moving direction</param>
        public void MoveInput(Vector3 direction)
        {
            //Vector3 newPosition = transform.position;
            //newPosition.x += direction.x * _speed * Time.deltaTime;
            //newPosition.z += direction.y * _speed * Time.deltaTime;

            //transform.position = newPosition;


            //if (GetTickRatio() > 0.5f)
            //{
            //    _savedInput = direction;
            //}

            // TODO: Fix
            if (_movingDirection != Vector3.zero)
            {
                _movingDirection.x += (direction.x > _movingDirection.x ? 1 : -1) * _turningSpeed * Time.deltaTime;
                _movingDirection.z -= (direction.y > _movingDirection.z ? 1 : -1) * _turningSpeed * Time.deltaTime;
            }
        }

        /// <summary>
        /// Ends a bounce.
        /// </summary>
        private void EndBounce()
        {
            if (_isDead)
            {
                return;
            }

            _movingDirection = Vector3.zero;
            Vector3 newPosition = transform.position;
            newPosition.y = _groundY;
            transform.position = newPosition;

            if (CheckCollision())
            {
                _bounceSuccess = true;
            }
            else
            {
                Die();
            }

            _secondTick = !_secondTick;
        }

        private bool CheckCollision()
        {
            Vector3 p1 = transform.position + new Vector3(-0.5f * _characterSize.x, 0, 0.5f * _characterSize.z);
            Vector3 p2 = transform.position + new Vector3(-0.5f * _characterSize.x, 0, 0.5f * _characterSize.z);
            Vector3 p3 = transform.position + new Vector3(0.5f * _characterSize.x, 0, 0.5f * _characterSize.z);
            Vector3 p4 = transform.position + new Vector3(0.5f * _characterSize.x, 0, -0.5f * _characterSize.z);
            Ray ray1 = new Ray(p1, Vector3.down);
            Ray ray2 = new Ray(p2, Vector3.down);
            Ray ray3 = new Ray(p3, Vector3.down);
            Ray ray4 = new Ray(p4, Vector3.down);
            RaycastHit hit;
            bool touchingPlatform =
                Physics.Raycast(ray1, out hit, _hitDist, _platformLayerMask) ||
                Physics.Raycast(ray2, out hit, _hitDist, _platformLayerMask) ||
                Physics.Raycast(ray3, out hit, _hitDist, _platformLayerMask) ||
                Physics.Raycast(ray4, out hit, _hitDist, _platformLayerMask);

            if (touchingPlatform)
            {
                Platform platform = hit.transform.GetComponent<Platform>();
                if (platform != null)
                {
                    platform.BouncedOn();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Updates bouncing.
        /// </summary>
        private void UpdateBounce()
        {
            float ratio = GetTickRatio();

            Vector3 newPosition = transform.position;
            newPosition.y = _groundY + Mathf.Sin(ratio * Mathf.PI) * _bounceHeight;

            if (_movingDirection != Vector3.zero)
            {
                newPosition.x = _startposition.x + ratio * _movingDirection.x * _bounceDistance;
                newPosition.z = _startposition.z + ratio * _movingDirection.z * _bounceDistance;
            }

            transform.position = newPosition;

            GameManager.Instance.PlayerTickRatio = (_secondTick ? ratio : 1 - ratio);
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

        private void Die()
        {
            // TODO
            _isDead = true;
            Debug.Log("Player died.");
            Respawn();
        }

        public void Respawn()
        {
            _isDead = false;
            _halfTempoOffset = false;
            transform.position = _spawnPosition;
            StartBounce();
            Debug.Log("Player respawned.");
        }

        /// <summary>
        /// Activates or deactivates the double tempo power.
        /// </summary>
        /// <param name="activate">Should the double tempo
        /// power be activated</param>
        public void DoubleTempoInput(bool activate)
        {
            _doubleTempoRequest = activate;
        }

        /// <summary>
        /// Disposes of everything necessary when the application is quit. 
        /// </summary>
        private void OnApplicationQuit()
        {
            Metronome.Instance.OnTick -= HandleTickEvent;
        }

        private void OnDrawGizmos()
        {
            // Distance circle
            Gizmos.color = Color.white;
            Vector3 point = new Vector3(_startposition.x, _groundY, _startposition.z);
            Gizmos.DrawWireSphere(point, _bounceDistance);

            // Character dimensions
            Gizmos.color = Color.blue;
            Vector3 p1 = transform.position + -0.5f * _characterSize;
            Vector3 p2 = transform.position + new Vector3(-0.5f * _characterSize.x, -0.5f * _characterSize.y, 0.5f * _characterSize.z);
            Vector3 p3 = transform.position + new Vector3(0.5f * _characterSize.x, -0.5f * _characterSize.y, 0.5f * _characterSize.z);
            Vector3 p4 = transform.position + new Vector3(0.5f * _characterSize.x, -0.5f * _characterSize.y, -0.5f * _characterSize.z);
            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, p3);
            Gizmos.DrawLine(p3, p4);
            Gizmos.DrawLine(p4, p1);
        }
    }
}
