using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class HoverContainer : MonoBehaviour
    {
        public float hoverHeight,
            hoverSpeed;

        public Transform _rootObj;
        private float time;
        private float sineWave;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!World.Instance.GamePaused)
            {
                time += World.Instance.DeltaTime;
                Vector3 newPosition = transform.position;
                sineWave = Mathf.Sin(time * hoverSpeed);
                newPosition.y = _rootObj.position.y + hoverHeight * sineWave;
                transform.position = newPosition;
            }
        }
    }
}