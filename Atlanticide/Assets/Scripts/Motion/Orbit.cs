using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class Orbit : MonoBehaviour
    {
        [SerializeField]
        private Vector3 _center;

        [SerializeField]
        private Utils.Axis _axis;

        [SerializeField]
        private float _radius = 3f;

        [SerializeField]
        private float _orbitTime = 5f;

        [SerializeField]
        private bool _clockwise = true;

        private float _angle;

        private void Update()
        {
            UpdateAngle();
            ChangePosition();
        }

        private void UpdateAngle()
        {
            if (_clockwise)
            {
                _angle += 2 * Mathf.PI * World.Instance.DeltaTime / _orbitTime;
                if (_angle >= 2 * Mathf.PI)
                {
                    _angle -= 2 * Mathf.PI;
                }
            }
            else
            {
                _angle -= 2 * Mathf.PI * World.Instance.DeltaTime / _orbitTime;
                if (_angle <= 0)
                {
                    _angle += 2 * Mathf.PI;
                }
            }
        }

        private void ChangePosition()
        {
            Vector3 pointAroundCenter = Vector3.zero;

            switch (_axis)
            {
                case Utils.Axis.X:
                {
                    pointAroundCenter = new Vector3(Mathf.Sin(_angle), Mathf.Cos(_angle), 0);
                    break;
                }
                case Utils.Axis.Y:
                {
                    pointAroundCenter = new Vector3(Mathf.Sin(_angle), 0, Mathf.Cos(_angle));
                    break;
                }
                case Utils.Axis.Z:
                {
                    pointAroundCenter = new Vector3(0, Mathf.Sin(_angle), Mathf.Cos(_angle));
                    break;
                }
            }

            transform.position = _center + pointAroundCenter * _radius;
        }
    }
}
