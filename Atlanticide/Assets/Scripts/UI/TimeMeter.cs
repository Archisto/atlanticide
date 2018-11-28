using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class TimeMeter : MonoBehaviour
    {
        public RectTransform timeMeterBarRectTransform;
        public float lerpModifier;
        private float currentTimeMeterBarScale;

        // Use this for initialization
        void Start()
        {
            currentTimeMeterBarScale = 1f;
        }

        // Update is called once per frame
        void Update()
        {
            if (currentTimeMeterBarScale > 0)
            {
                currentTimeMeterBarScale = Mathf.Lerp(currentTimeMeterBarScale, 0f, Time.deltaTime * lerpModifier);
                timeMeterBarRectTransform.localScale = new Vector3(currentTimeMeterBarScale, 1f, 1f);
            }
        }
    }
}