using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class EnergyCollector : MonoBehaviour
    {
        public enum ECMode
        {
            Idle = 0,
            Draining = 1,
            Emitting = 2
        }

        [SerializeField]
        private GameObject _energyObject;

        [SerializeField]
        private float _drainTime = 1f;

        [SerializeField]
        private float _emitTime = 1f;

        private float _elapsedTime;

        /// <summary>
        /// The energy node target which is currently
        /// being used by the active energy collector.
        /// </summary>
        private EnergyNode _activeTarget;

        public ECMode Mode { get; private set; }

        public EnergyNode Target { get; set; }

        /// <summary>
        /// Is the energy collector in idle state.
        /// </summary>
        public bool IsIdle { get { return Mode == ECMode.Idle; } }

        /// <summary>
        /// Is the energy collector in draining state.
        /// </summary>
        public bool IsDraining { get { return Mode == ECMode.Draining; } }

        /// <summary>
        /// Is the energy collector in emitting state.
        /// </summary>
        public bool IsEmitting { get { return Mode == ECMode.Emitting; } }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            ReturnToIdle();
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (!World.Instance.GamePaused)
            {
                if (Mode != ECMode.Idle)
                {
                    UpdateDrainingOrEmitting();
                }

                UpdateTarget();
            }
        }

        private void UpdateDrainingOrEmitting()
        {
            _elapsedTime += World.Instance.DeltaTime;
            float targetTime = (Mode == ECMode.Draining ? _drainTime : _emitTime);
            if (_elapsedTime >= targetTime)
            {
                ReturnToIdle();
            }
        }

        public void UpdateTarget()
        {
            if (Target != null &&
                !Target.PositionWithinRange(transform.position))
            {
                Target = null;
                //Debug.Log("Node lost, too far");
            }
        }

        public void SetTarget(EnergyNode node)
        {
            if (Target != node)
            {
                Target = node;

                //if (node != null)
                //{
                //    Debug.Log("New node: " + Target);
                //}
            }
        }

        public void TryDrainingOrEmitting()
        {
            if (Mode == ECMode.Idle && Target != null)
            {
                if (!Target.Usable)
                {
                    Target = null;
                    return;
                }

                _activeTarget = Target;
                if (Target is EnergySource)
                {
                    if (!Target.ZeroCharge)
                    {
                        StartDraining();
                    }
                }
                else
                {
                    if (!Target.MaxCharge)
                    {
                        StartEmitting();
                    }
                }
            }
        }

        private bool CanDrainOrEmit()
        {
            if (Mode == ECMode.Idle && Target != null)
            {
                if (!Target.Usable)
                {
                    Target = null;
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        public bool TryDraining()
        {
            if (CanDrainOrEmit() && Target.IsValidEnergySource())
            {
                _activeTarget = Target;
                StartDraining();
                return true;
            }

            return false;
        }

        public bool TryEmitting()
        {
            if (CanDrainOrEmit() && Target.IsValidEnergyTarget())
            {
                _activeTarget = Target;
                StartEmitting();
                return true;
            }

            return false;
        }

        protected void StartDraining()
        {
            if (World.Instance.CurrentEnergyCharges < World.Instance.MaxEnergyCharges)
            {
                //Debug.Log("Draining energy");
                _elapsedTime = 0f;
                Mode = ECMode.Draining;
                _energyObject.SetActive(true);
                Drain();
            }
        }

        protected void StartEmitting()
        {
            if (World.Instance.CurrentEnergyCharges > 0)
            {
                //Debug.Log("Emitting energy");
                _elapsedTime = 0f;
                Mode = ECMode.Emitting;
                _energyObject.SetActive(true);
                Emit();
            }
        }

        private void Drain()
        {
            World.Instance.DrainingEnergy = true;
            ChangeCharges(1);
            _activeTarget.LoseCharge();
        }

        private void Emit()
        {
            World.Instance.EmittingEnergy = true;
            ChangeCharges(-1);
            _activeTarget.GainCharge();
        }

        public void ChangeCharges(int charges)
        {
            World.Instance.SetEnergyChargesAndUpdateUI
                (World.Instance.CurrentEnergyCharges + charges);
        }

        public void ReturnToIdle()
        {
            World.Instance.DrainingEnergy = false;
            World.Instance.EmittingEnergy = false;
            Mode = ECMode.Idle;
            _energyObject.SetActive(false);
            _activeTarget = null;
        }

        public void ResetEnergyCollector()
        {
            ReturnToIdle();
            Target = null;
            _elapsedTime = 0f;
        }

        /// <summary>
        /// Draws gizmos.
        /// </summary>
        private void OnDrawGizmos()
        {
            //if (_mode != Mode.Idle)
            //{
            //    Gizmos.color = Color.yellow;
            //    Gizmos.DrawSphere(transform.position, World.Instance.energyCollectRadius);
            //}
        }
    }
}
