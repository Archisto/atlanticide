using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Atlanticide.UI
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

        private Vector2 _canvasSize;
        private Vector2 _uiOffset;
        private Camera _camera;
        private PauseScreen _pauseScreen;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            InitUI();
            UpdateUI();
        }

        /// <summary>
        /// Initializes the UI.
        /// </summary>
        private void InitUI()
        {
            _canvasSize = GetComponent<Canvas>().pixelRect.size;
            _uiOffset = new Vector2(-0.5f * _canvasSize.x, -0.5f * _canvasSize.y);
            _camera = FindObjectOfType<CameraController>().GetComponent<Camera>();
            _pauseScreen = GetComponentInChildren<PauseScreen>(true);
        }

        /// <summary>
        /// Updates the UI.
        /// </summary>
        public void UpdateUI()
        {
            if (_scoreText != null)
            {
                _scoreText.text = "Score: " + GameManager.Instance.CurrentScore;
            }
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

        public void ActivatePauseScreen(bool activate, string playerName)
        {
            _pauseScreen.gameObject.SetActive(activate);
            _pauseScreen.pausingPlayerText.text = playerName;
        }

        public void MoveUIObjToWorldPoint(Image uiObj, Vector3 worldPoint)
        {
            Vector2 viewPortPos = _camera.WorldToViewportPoint(worldPoint);
            Vector2 proportionalPosition =
                new Vector2(viewPortPos.x * _canvasSize.x, viewPortPos.y * _canvasSize.y);
            uiObj.transform.localPosition = proportionalPosition + _uiOffset;
        }

        public void ResetUI()
        {
        }
    }
}
