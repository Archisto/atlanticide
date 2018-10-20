using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{

    public class SimplePlatformPath : MonoBehaviour
    {

        [Header("Objects")]

        [SerializeField]
        GameObject Object;

        [SerializeField]
        Transform NormalPoint;

        [SerializeField]
        Transform TargetPoint;

        [Header("Settings")]

        [SerializeField]
        float Speed = 1;

        [SerializeField]
        bool LockToPath = true;

        [SerializeField]
        float Accuracy = 0.1f;

        [SerializeField]
        bool LockToTarget;

        [SerializeField]
        bool MustFinishTarget;

        [SerializeField]
        bool MustFinishNormal;

        [Header("Activation type")]

        [SerializeField]
        bool UsingKey;

        [SerializeField]
        int KeyCode;

        [SerializeField]
        EnergyTarget Target;

        private bool TowardsTarget;

        private bool OnTarget = true;

        // Use this for initialization
        void Start()
        {
            TowardsTarget = false;
            if (LockToPath)
            {
                FinishMovement();
            }
        }

        // Update is called once per frame
        void Update()
        {
            // Do nothing if object is on target and lock is ON
            if(IsAtTarget() && LockToTarget)
            {
                return;
            }

            // Check whether key/energytarget should be checked
            if (IsDone() || AllowCheck()) {
                if (UsingKey)
                {
                    CheckKey();
                }
                else
                {
                    CheckEnergyTarget();
                }
            }

            // If object is on target, do not move
            if (OnTarget)
            {
                return;
            }

            // move and check if is done
            Moving();
            IsDone();
        }

        /// <summary>
        /// Checks some preferences to see if CheckKey/CheckEnergyTarget should be called
        /// </summary>
        /// <returns></returns>
        private bool AllowCheck()
        {
            if(MustFinishTarget && MustFinishNormal)
            {
                return false;
            }

            if(!MustFinishTarget && !MustFinishNormal)
            {
                return true;
            }

            if(TowardsTarget && !MustFinishTarget)
            {
                return true;
            }

            if(!TowardsTarget && !MustFinishNormal)
            {
                return true;
            }

            return false;
        }

        // Checks if corresponding key is activated
        private void CheckKey()
        {
            bool keyMatch = false;

            foreach (int ownedKeyCode in World.Instance.keyCodes)
            {
                if (KeyCode == ownedKeyCode)
                {
                    keyMatch = true;
                    CallForMovement(true);
                }
            }

            if (!keyMatch)
            {
                CallForMovement(false);
            }
        }

        // Checks if energy target has max level charge
        private void CheckEnergyTarget()
        {
            if (Target.MaxCharge)
            {
                CallForMovement(true);
            }

            if (!World.Instance.EmittingEnergy)
            {
                Target.currentCharges = 0;
                CallForMovement(false);
            }
        }

        // Determines to what direction Object should move
        private void CallForMovement(bool toTarget)
        {
            TowardsTarget = toTarget;
            IsDone();
        }

        // Snap the object to current target
        private void FinishMovement()
        {
            Object.transform.position = (TowardsTarget ? TargetPoint.position : NormalPoint.position);
            OnTarget = true;
        }

        // Move the Object towards the current target
        private void Moving()
        {
            Object.transform.position = Vector3.MoveTowards(Object.transform.position, (TowardsTarget ? TargetPoint.position : NormalPoint.position), Speed * Time.deltaTime);
        }

        // Check if movement is done
        private bool IsDone()
        {
            if (Vector3.Distance(Object.transform.position, (TowardsTarget ? TargetPoint.position : NormalPoint.position)) <= Accuracy)
            {
                OnTarget = true;

                if (LockToPath)
                {
                    FinishMovement();
                }
                return true;
            }
            OnTarget = false;
            return false;
        }

        private bool IsAtTarget()
        {
            return TowardsTarget && IsDone();
        }

        private bool IsAtNormal()
        {
            return !TowardsTarget && IsDone();
        }

    }
}