using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class ShieldForceField : MonoBehaviour
    {
        public GameObject _shieldForceField;
        private Animator _shieldAnimator;

        private void Start()
        {
            _shieldAnimator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            if (_shieldAnimator.GetCurrentAnimatorStateInfo(0).IsName("Shield Open Idle"))
            {
                _shieldForceField.SetActive(true);
            } else
            {
                _shieldForceField.SetActive(false);
            }
        }
    }
}