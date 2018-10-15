using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// An object which can be broken with a shield bash.
    /// </summary>
    [RequireComponent(typeof(ShieldBashSwitch))]
    public class ShieldBashBreakable : LevelObject
    {
        [SerializeField, Range(1, 20)]
        private int _maxHitpoints = 1;

        [SerializeField]
        private int _score;

        [SerializeField]
        private GameObject _unbrokenObject;

        [SerializeField]
        private GameObject _brokenObject;

        private ShieldBashSwitch _sbSwitch;
        private Collider _collider;
        private int hitpoints;
        private bool _damageTaken;
        private bool _broken;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _sbSwitch = GetComponent<ShieldBashSwitch>();
            _collider = GetComponent<Collider>();
            hitpoints = _maxHitpoints;
            UpdateObjectLook();
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (!_broken)
            {
                if (!_damageTaken && _sbSwitch.Activated)
                {
                    _damageTaken = true;
                    hitpoints--;
                    if (hitpoints == 0)
                    {
                        DestroyObject();
                    }
                }
                else if (_damageTaken && !World.Instance.ShieldBashing)
                {
                    _damageTaken = false;
                    _sbSwitch.Activate(false);
                }
            }
        }

        /// <summary>
        /// Destroys the object.
        /// </summary>
        public override void DestroyObject()
        {
            _broken = true;
            UpdateObjectLook();
            GameManager.Instance.ChangeScore(_score);
            _collider.enabled = false;
            base.DestroyObject();
        }

        /// <summary>
        /// Updates the object's look based on whether it is broken.
        /// </summary>
        private void UpdateObjectLook()
        {
            _unbrokenObject.SetActive(!_broken);
            _brokenObject.SetActive(_broken);
        }

        /// <summary>
        /// Resets the object to its default state.
        /// </summary>
        public override void ResetObject()
        {
            _broken = false;
            _damageTaken = false;
            hitpoints = _maxHitpoints;
            UpdateObjectLook();
            _collider.enabled = true;
            base.ResetObject();
        }

        /// <summary>
        /// Draws gizmos.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!_broken)
            {
                Utils.DrawHPGizmo(transform.position + Vector3.up * 1.5f,
                    hitpoints, _maxHitpoints, Color.red);
            }
        }
    }
}
