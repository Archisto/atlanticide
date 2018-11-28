using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Atlanticide
{
    public class OrichalcumCounter : MonoBehaviour
    {
        public TextMeshProUGUI orichalcumCounter;
        public int orichalcumCounterInt,
            plusOneFontSize,
            plusFiveFontSize;
        public float lerpMultiplier;

        private float startingFontSize,
            _currentFontSize;

        // Use this for initialization
        void Start()
        {
            startingFontSize = orichalcumCounter.fontSize;
            _currentFontSize = startingFontSize;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                orichalcumCounterInt += 1;
                orichalcumCounter.text = orichalcumCounterInt.ToString();

                _currentFontSize += plusOneFontSize;
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                orichalcumCounterInt += 5;
                orichalcumCounter.text = orichalcumCounterInt.ToString();
                _currentFontSize += plusFiveFontSize;
            }

            if (_currentFontSize > startingFontSize)
            {
                _currentFontSize = Mathf.Lerp(_currentFontSize, startingFontSize, Time.deltaTime * lerpMultiplier);
                orichalcumCounter.fontSize = _currentFontSize;
            }
        }
    }
}