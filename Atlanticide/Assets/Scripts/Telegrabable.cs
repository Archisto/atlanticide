using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class Telegrabable : MonoBehaviour
    {
        public bool telegrabbed;
        private Vector3 _telegrabPosition;

        private void Update()
        {
            telegrabbed = false;

            foreach (Transform telegrab in GameManager.Instance.GetTelegrabs())
            {
                if (telegrab != null &&
                    Vector3.Distance(transform.position, telegrab.position) <= World.Instance.telegrabRadius)
                {
                    telegrabbed = true;
                    _telegrabPosition = telegrab.position;
                    break;
                }
            }

            if (telegrabbed)
            {
                transform.position = _telegrabPosition;
            }
        }
    }
}
