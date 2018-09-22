using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Atlanticide
{
    public class UIController : MonoBehaviour
    {
        [SerializeField]
        private Text _scoreText;

        [SerializeField]
        private Slider _energyBar1;

        [SerializeField]
        private Slider _energyBar2;

        [SerializeField]
        private Image _fade;

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
        }

        /// <summary>
        /// Updates the UI.
        /// </summary>
        public void UpdateUI()
        {
            _scoreText.text = "Score: " + GameManager.Instance.CurrentScore;
        }

        public Slider GetEnergyBar(int playerNum)
        {
            switch (playerNum)
            {
                case 0:
                {
                    return _energyBar1;
                }
                case 1:
                {
                    return _energyBar2;
                }
                default:
                {
                    return null;
                }
            }
        }

        public Image GetFade()
        {
            return _fade;
        }
    }
}
