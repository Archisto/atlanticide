using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide.WaypointSystem
{
    public class Waypoint : MonoBehaviour
    {
        [SerializeField]
        private bool _isPartOfCurve;

        [SerializeField]
        private string _curveName = "";

        public bool IsPartOfCurve
        {
            get
            {
                return _isPartOfCurve;
            }
            set
            {
                _isPartOfCurve = value;
            }
        }

        public string CurveName
        {
            get
            {
                return _curveName;
            }
            set
            {
                _curveName = value;
            }
        }

        public Vector3 Position
        {
            get
            {
                return transform.position;
            }
        }
    }
}
