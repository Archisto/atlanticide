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
        }


        public override void ResetObject()
        {
            base.ResetObject();
            Activated = _activatedOnStart;
        }
    }
}