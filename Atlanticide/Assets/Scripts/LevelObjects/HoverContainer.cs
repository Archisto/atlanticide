using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class HoverContainer : MonoBehaviour
    {
        public float hoverHeight,
            hoverSpeed;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            this.transform.position = (Vector3.up * hoverHeight) * Mathf.Cos(Time.time * hoverSpeed);
        }
    }
}