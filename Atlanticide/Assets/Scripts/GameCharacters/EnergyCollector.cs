using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class EnergyCollector : MonoBehaviour
    {
        private enum Mode
        {
            Idle = 0,
            Draining = 1,
            Emitting = 2
        }

        [SerializeField]
        private GameObject _energyObject;

        [SerializeField, Range(1, 20)]
        private int _maxCharges = 5;

        [SerializeField]
        private float _chargeTime = 1f;

        [SerializeField]
        private float _emitTime = 1f;

        private float _elapsedTime;
        private Mode _mode;
        private EnergyNode _node;
        private EnergyNode _tempNode;

        public int CurrentCharges { get; private set; }

        public int MaxCharges { get { return _maxCharges; } }

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
                if (_mode != Mode.Idle)
                {
                    UpdateChargingOrEmitting();
                }

                UpdateTarget();
            }
        }

        private void UpdateChargingOrEmitting()
        {
            _elapsedTime += World.Instance.DeltaTime;
            float targetTime = (_mode == Mode.Draining ? _chargeTime : _emitTime);
            if (_elapsedTime >= targetTime)
            {
                //if (_mode == Mode.Draining)
                //{
                //    Drain();
                //}
                //else
                //{
                //    Emit();
                //}

                ReturnToIdle();
            }
        }

        public void UpdateTarget()
        {
            if (_node != null &&
                Vector3.Distance(transform.position, _node.transform.position)
                    > World.Instance.energyCollectRadius)
            {
                _node.RemoveClosestPlayer();
                _node = null;
                Debug.Log("Node lost, too far");
            }
        }

        public void SetTarget(EnergyNode node)
        {
            if (_node != node)
            {
                _node = node;

                if (node == null)
                {
                    Debug.Log("Node lost, other player closer");
                }
                else
                {
                    Debug.Log("New node: " + _node);
                }
            }
        }

        public void StartChargingOrEmitting()
        {
            if (_mode == Mode.Idle && _node != null)
            {
                if (!_node.Active)
                {
                    _node = null;
                    return;
                }

                _tempNode = _node;
                if (_node is EnergySource)
                {
                    if (!_node.ZeroCharge)
                    {
                        StartDraining();
                    }
                }
                else
                {
                    if (!_node.MaxCharge)
                    {
                        StartEmitting();
                    }
                }
            }
        }

        protected void StartDraining()
        {
            if (CurrentCharges < _maxCharges)
            {
                Debug.Log("Draining energy; charges: " + (CurrentCharges + 1));
                _elapsedTime = 0f;
                _mode = Mode.Draining;
                _energyObject.SetActive(true);
                Drain();
            }
        }

        protected void StartEmitting()
        {
            if (CurrentCharges > 0)
            {
                Debug.Log("Emitting energy; charges: " + (CurrentCharges - 1));
                _elapsedTime = 0f;
                _mode = Mode.Emitting;
                _energyObject.SetActive(true);
                Emit();
            }
        }

        private void Drain()
        {
            CurrentCharges++;
            _tempNode.LoseCharge();
        }

        private void Emit()
        {
            CurrentCharges--;
            _tempNode.GainCharge();
        }

        public void ReturnToIdle()
        {
            _mode = Mode.Idle;
            _energyObject.SetActive(false);
            _tempNode = null;
        }

        public void ResetEnergyCollector()
        {
            ReturnToIdle();
            CurrentCharges = 0;
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
