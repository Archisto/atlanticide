using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    [RequireComponent(typeof(KeyCodeSwitch))]
    [RequireComponent(typeof(Dissolve))]
    public class AppearingPlatform : LevelObject
    {
        private const string DefaultKey = "Default";
        private const string PlatformKey = "Platform";

        [SerializeField, Range(0.1f, 10f)]
        private float _appearTime = 0.3f;

        [SerializeField, Range(0.1f, 10f)]
        private float _vanishTime = 1f;

        [SerializeField]
        private bool _activeByDefault;

        [SerializeField]
        private bool _permanent;

        private KeyCodeSwitch _switch;
        private Dissolve _dissolve;
        private bool _active;
        private bool _changingVisibility;
        private float _elapsedTime;

        private bool StateChangeIsAllowed()
        {
            return (_active == _activeByDefault || !_permanent) &&
                _active == (_activeByDefault ?
                _switch.Activated : !_switch.Activated);
        }

        private float GetTargetProgress(float activeProg, float inactiveProg)
        {
            return (_active ? activeProg : inactiveProg);
        }

        private int GetTargetLayer()
        {
            return LayerMask.NameToLayer(_active ? PlatformKey : DefaultKey);
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _switch = GetComponent<KeyCodeSwitch>();
            _dissolve = GetComponent<Dissolve>();
            Init();
        }

        /// <summary>
        /// Initializes the object at the start and when reset.
        /// </summary>
        private void Init()
        {
            _active = _activeByDefault;
            _dissolve.SetProgress(GetTargetProgress(0f, 1f));
            gameObject.layer = GetTargetLayer();
        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        protected override void UpdateObject()
        {
            if (_changingVisibility)
            {
                UpdateVisibility();
            }
            else if (StateChangeIsAllowed())
            {
                _active = !_active;
                _changingVisibility = true;

                // Makes the platform walkable before the appearing
                if (_active)
                {
                    gameObject.layer = GetTargetLayer();
                }
            }

            base.UpdateObject();
        }

        private void UpdateVisibility()
        {
            float targetTime = (_active ? _appearTime : _vanishTime);
            _elapsedTime += World.Instance.DeltaTime;
            float ratio = (_elapsedTime / targetTime);
            if (_elapsedTime >= targetTime)
            {
                _elapsedTime = 0f;
                _changingVisibility = false;
                _dissolve.SetProgress(GetTargetProgress(0f, 1f));

                // Makes the platform unwalkable after the vanishing
                if (!_active)
                {
                    gameObject.layer = GetTargetLayer();
                }
            }
            else
            {
                _dissolve.SetProgress(GetTargetProgress(1f - ratio, ratio));
            }
        }

        public override void ResetObject()
        {
            _elapsedTime = 0f;
            _changingVisibility = false;
            Init();
            base.ResetObject();
        }
    }
}
