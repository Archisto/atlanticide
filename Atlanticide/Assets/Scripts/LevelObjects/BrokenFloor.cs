using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{

    public class BrokenFloor : LevelObject
    {
        [SerializeField]
        private Dissolve _Dissolve;

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

        #region Variables

        private float Dissolving;

        #endregion


        // Use this for initialization
        void Start()
        {
            _defaultPosition = transform.position;
            _State = FloorState.DOWN;
            Dissolving = 0;
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

        private void ListenForActivation()
        {

        }

        private void ListenForForce()
        {

        }

        // floor falls towards the Bedrock
        private void Fall()
        {
            transform.position += transform.TransformDirection(Vector3.down) * World.Instance.DeltaTime;
            if(transform.localPosition.y < Bedrock)
            {
                _State = FloorState.DOWN;
                transform.localPosition.Set(transform.localPosition.x, Bedrock, transform.localPosition.z);
            }
        }

        private void Dissolve()
        {
            if (Dissolving < 0.5f)
            {
                _Dissolve.SetProgress(Dissolving += World.Instance.DeltaTime / 5f);

            }
        }

        public override void ResetObject()
        {
            base.ResetObject();
            transform.position = _defaultPosition;
        }

    }
}