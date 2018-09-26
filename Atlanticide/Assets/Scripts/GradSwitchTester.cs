using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class GradSwitchTester : MonoBehaviour
    {
        [SerializeField]
        float _maxDist = 5;

        [SerializeField]
        bool _increment;

        [SerializeField]
        Rotate _rotator;

        [SerializeField]
        Transparency _transparency;

        [SerializeField]
        Scale _scale;

        GradualSwitch _mySwitch;
        PlayerCharacter[] _players;
        bool _stayedOn;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _mySwitch = GetComponent<GradualSwitch>();
            _players = GameManager.Instance.GetPlayers();
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            float dist = _maxDist - _players[0].transform.position.y;

            if (!_increment)
            {
                _mySwitch.SetProgress((_maxDist - dist) / _maxDist);
            }
            else
            {
                _mySwitch.AdjustProgress(dist / 10000f);
            }

            if (_mySwitch.Activated && !_stayedOn)
            {
                _stayedOn = true;

                if (_rotator != null)
                {
                    if (_rotator.Active)
                    {
                        _rotator.StopMoving(false);
                    }
                    else
                    {
                        _rotator.StartMoving(false);
                    }
                }
            }
            else if (!_mySwitch.Activated && _stayedOn)
            {
                _stayedOn = false;
            }

            if (_transparency != null)
            {
                _transparency.SetAlpha(_mySwitch.Progress);
            }

            if (_scale != null)
            {
                _scale.SetScale(_mySwitch.Progress, 0, 0);
            }
        }
    }
}
