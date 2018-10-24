using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class ChildGameObjectFetcher : MonoBehaviour
    {
        public GameObject _forceFieldGameObject;

        public GameObject GetForceField()
        {
            return _forceFieldGameObject;
        }
    }
}