using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StrideUnbroken
{
    public class UIController : MonoBehaviour
    {
        [SerializeField]
        private Text _scoreText;

        [SerializeField]
        private Slider _energyBar;

        [SerializeField]
        private Slider _tickRatioBar;

        [SerializeField]
        private Slider _tickRatioBar2;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            UpdateUI();
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            _tickRatioBar.value = GameManager.Instance.PlayerTickRatio;
            _tickRatioBar2.value = (Metronome.CurrentTick % 2 == 0 ? Metronome.TickRatio : 1 - Metronome.TickRatio);
        }

        /// <summary>
        /// Updates the UI.
        /// </summary>
        public void UpdateUI()
        {
            _scoreText.text = "Score: " + GameManager.Instance.CurrentScore;
        }
    }
}
