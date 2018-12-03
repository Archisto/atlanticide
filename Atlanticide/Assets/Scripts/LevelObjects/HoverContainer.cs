using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class HoverContainer : MonoBehaviour
    {
        public float hoverHeight,
            hoverSpeed;

        private float time;
        private float sineWave;

        // Update is called once per frame
        void Update()
        {
            if (!World.Instance.GamePaused)
            {
                time += World.Instance.DeltaTime;
                Vector3 newPosition = transform.position;
                sineWave = Mathf.Sin(time * hoverSpeed);
                newPosition.y = transform.position.y + hoverHeight * sineWave;
                transform.position = newPosition;
            }
        }
    }
}