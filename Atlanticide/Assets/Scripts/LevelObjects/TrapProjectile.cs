using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class TrapProjectile : LevelObject
    {
        [SerializeField]
        private bool _activatedOnStart;

        [SerializeField]
        private Activatable _activatable;

        [SerializeField]
        private Weapon Weapon;

        private bool Activated;

        // Use this for initialization
        void Start()
        {
            Activated = _activatedOnStart;
        }

        protected override void UpdateObject()
        {
            if (Activated)
            {
                base.UpdateObject();
                Weapon.Fire();
            }

            if (_activatable != null)
            {
                Activated = (_activatedOnStart ?
                    !_activatable.Activated : _activatable.Activated);
            }
        }

        public override void ResetObject()
        {
            Activated = _activatedOnStart;
            base.ResetObject();
        }
    }
}