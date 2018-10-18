using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide {

    public class SimplePlatformPath : MonoBehaviour {

        [Header("Objects")]

        [SerializeField]
        GameObject Object;

        [SerializeField]
        Transform PointA, PointB;

        [Header("Settings")]

        [SerializeField]
        float Speed;

        [SerializeField]
        bool ToPointA;

        [SerializeField]
        bool LockToPath;

        [SerializeField]
        float Accuracy;

        [Header("Activation type")]

        [SerializeField]
        bool UsingKey;

        [SerializeField]
        int KeyCode;

        [SerializeField]
        EnergyTarget Target;


        private bool OnTarget = true;

        // Use this for initialization
        void Start() {
            if (LockToPath)
            {
                FinishMovement();
            }
        }

        // Update is called once per frame
        void Update() {

            if (UsingKey)
            {
                CheckKey();
            } else
            {
                CheckEnergyTarget();
            }

            if (OnTarget)
            {
                return;
            }

            Moving();
            IsDone();
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
                    CallForMovement(ToPointA);
                }
            }

            if (!keyMatch)
            {
                CallForMovement(ToPointA);
            }
        }

        // Determines to what direction Object should move
        private void CallForMovement(bool _ToPointA)
        {
            ToPointA = _ToPointA;
            IsDone();
        }

        // Checks if energy target has max level charge
        private void CheckEnergyTarget()
        {
            if (Target.MaxCharge)
            {
                CallForMovement(ToPointA);
            }

            if(!World.Instance.EmittingEnergy)
            {
                CallForMovement(!ToPointA);
            }
        }

        // Snap the object to current target
        private void FinishMovement()
        {
            Object.transform.position = (ToPointA ? PointA.position : PointB.position);
            OnTarget = true;
        }

        // Move the Object towards the current target
        private void Moving()
        {
            Vector3.MoveTowards(Object.transform.position, (ToPointA ? PointA.position : PointB.position), Speed * Time.deltaTime);
        }

        // Check if movement is done
        private bool IsDone()
        {
            if (Vector3.Distance(Object.transform.position, (ToPointA ? PointA.position : PointB.position)) < Accuracy)
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

    }
}