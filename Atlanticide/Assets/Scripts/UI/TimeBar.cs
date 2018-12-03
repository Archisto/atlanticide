using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Atlanticide
{
    public class TimeBar : MonoBehaviour
    {
        [SerializeField]
        private Image _fill;

        [SerializeField]
        private TextMeshProUGUI _counter;

        [SerializeField]
        private float _fillXMin;

        [SerializeField]
        private float _fillXMax;

        [SerializeField]
        private Color _timeBarNormalColor = Color.white;

        [SerializeField]
        private Color _timeBarFlashColor = Color.red;

        public bool active;

        private LevelManager _levelManager;
        private Timer _levelTimer;
        private RectTransform _fillRectTransform;
        private float _timeRatio = 0f;

        public float lerpModifier;

        // Use this for initialization
        void Start()
        {
            _levelManager = FindObjectOfType<LevelManager>();
            _levelTimer = _levelManager.LevelTimer;
            _fillRectTransform = _fill.rectTransform;
            _fillRectTransform.offsetMax = new Vector2(_fillXMin, _fillRectTransform.offsetMax.y);
        }

        // Update is called once per frame
        void Update()
        {
            active = _levelManager.LevelActive;

            if (active)
            {
                _timeRatio = _levelManager.LevelTimeElapsedRatio;
                float fillX = Mathf.Lerp(-1 * _fillXMin, -1 * _fillXMax, _timeRatio);
                _fillRectTransform.offsetMax = new Vector2(fillX, _fillRectTransform.offsetMax.y);

                if (_timeRatio == 1f)
                {
                    _counter.text = "0";
                }
                else
                {
                    _counter.text = ((int) (_levelTimer.targetTime - _levelTimer.elapsedTime) + 1).ToString();
                }
            }
            else if (_timeRatio < 1f && _levelManager.LevelTimeElapsedRatio == 1f)
            {
                _timeRatio = 1f;
                _counter.text = "0";
            }
        }

        public void FlashLevelTimeBar(bool flash)
        {
            _fill.color = (flash ?
                _timeBarFlashColor : _timeBarNormalColor);
        }

        public void ResetBar()
        {
            _fill.color = _timeBarNormalColor;
        }
    }
}