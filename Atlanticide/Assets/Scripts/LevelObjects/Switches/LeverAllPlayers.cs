using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    [RequireComponent(typeof(PlayerProximitySwitch))]
    public class LeverAllPlayers : Lever
    {
        private PlayerProximitySwitch _playerProxSwitch;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            _playerProxSwitch = GetComponent<PlayerProximitySwitch>();
        }

        protected override bool CanBeOperated()
        {
            return _playerProxSwitch.Activated && base.CanBeOperated();
        }
    }
}

