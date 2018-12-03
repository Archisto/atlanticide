using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Atlanticide
{
    public class ScoreCounter : MonoBehaviour
    {
        public TextMeshProUGUI orichalcumCounter;
        public int _normalFontSizeIncrease, _bigFontSizeIncrease;
        public float lerpMultiplier;

        private float _defaultFontSize,
            _currentFontSize,
            _maxFontSize;

        // Use this for initialization
        private void Start()
        {
            _defaultFontSize = orichalcumCounter.fontSize;
            _currentFontSize = _defaultFontSize;
            _maxFontSize = _defaultFontSize + _bigFontSizeIncrease + (_bigFontSizeIncrease - _normalFontSizeIncrease);
        }

        // Update is called once per frame
        private void Update()
        {
            if (_currentFontSize > _defaultFontSize)
            {
                _currentFontSize = Mathf.Lerp(_currentFontSize, _defaultFontSize, Time.deltaTime * lerpMultiplier);
                orichalcumCounter.fontSize = _currentFontSize;
            }
        }

        public void UpdateScore(int totalScore, bool bigScoreGain)
        {
            if (_currentFontSize < _maxFontSize)
            {
                if (bigScoreGain)
                {
                    _currentFontSize += _bigFontSizeIncrease;
                }
                else
                {
                    _currentFontSize += _normalFontSizeIncrease;
                }
            }

            orichalcumCounter.text = totalScore.ToString();
        }

        public void ResetCounter()
        {
            _currentFontSize = _defaultFontSize;
            orichalcumCounter.text = "0";
        }
    }
}