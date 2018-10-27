using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{

    public class BrokenFloor : LevelObject
    {
        #region Public

        [SerializeField]
        private Dissolve _Dissolve;

        [SerializeField]
        private bool _BreakImmediately;

        [SerializeField]
        private float _Resistance;

        [SerializeField]
        private float _FallSpeed;

        #endregion

        #region Variables, A

        // when true, floor changes to Breaking
        public bool Break
        {
            get; set;
        }

        // Local Y pos where floor stops going down (floor)
        public float Bedrock
        {
            get; set;
        }

        // Different states for the BrokenFloor
        public enum FloorState
        {
            UNBROKEN, BREAKING, BROKEN, DOWN
        }

        // Current state
        private FloorState _State;

        #endregion

        #region Variables, B

        // progress of _Dissolve
        private float Dissolving;

        // taken pressure, when reaches Resistance, object is Broken
        private float TakenPressure;

        // dissolve progress when floor enters breaking
        private float BreakingDissolve = 0.2f;

        // dissolve progress when floor enters down
        private float DownDissolve = 0.5f;

        #endregion


        // Use this for initialization
        void Start()
        {
            _defaultPosition = transform.position;
            _State = FloorState.UNBROKEN;
            Dissolving = 0;
            TakenPressure = 0;
        }

        protected override void UpdateObject()
        {
            base.UpdateObject();

            switch (_State)
            {
                case FloorState.UNBROKEN:
                    ListenForActivation();
                    break;
                case FloorState.BREAKING:
                    ListenForForce();
                    break;
                case FloorState.BROKEN:
                    Fall();
                    break;
                case FloorState.DOWN:
                    Dissolve();
                    break;
            }
        }

        // Wait for a signal to start breaking (breaking)
        private void ListenForActivation()
        {
            if (Break)
            {
                _State = FloorState.BREAKING;
                Dissolving = BreakingDissolve;
                _Dissolve.SetProgress(Dissolving);
            }
        }

        // Wait for a signal to start going down (broken)
        private void ListenForForce()
        {
            // floor is broken immediately
            if (_BreakImmediately)
            {
                _State = FloorState.BROKEN;
                Dissolving = DownDissolve;
                _Dissolve.SetProgress(Dissolving);
            } else
            {
                TakenPressure += World.Instance.DeltaTime;
                if(TakenPressure >= _Resistance)
                {
                    _State = FloorState.BROKEN;
                    TakenPressure = _Resistance;
                }
                Dissolving = BreakingDissolve + (TakenPressure/_Resistance) * (DownDissolve - BreakingDissolve);
                _Dissolve.SetProgress(Dissolving);
            }
        }

        // floor falls towards the Bedrock
        private void Fall()
        {
            transform.localPosition += transform.TransformDirection(Vector3.down) * _FallSpeed * World.Instance.DeltaTime;
            if(transform.localPosition.y < Bedrock)
            {
                _State = FloorState.DOWN;
                transform.localPosition.Set(transform.localPosition.x, Bedrock, transform.localPosition.z);
            }
        }

        // object dissolves
        private void Dissolve()
        {
            if (Dissolving < 1)
            {
                _Dissolve.SetProgress(Dissolving += World.Instance.DeltaTime / 1.5f);
            }
        }

        public override void ResetObject()
        {
            base.ResetObject();
            transform.position = _defaultPosition;
        }

    }
}