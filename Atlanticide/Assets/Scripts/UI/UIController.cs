using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Atlanticide.UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField]
        private PlayerStatus _playerStatusPrefab;

        [SerializeField]
        private Transform _playerStatusHandler;

        [SerializeField]
        private Text _scoreText;

        [SerializeField]
        private Slider _energyBar;

        [SerializeField]
        private Image _fade;

        [SerializeField]
        private List<Sprite> _toolImages;

        private Vector2 _canvasSize;
        private Vector2 _uiOffset;
        private Camera _camera;
        private PauseScreen _pauseScreen;
        private List<PlayerStatus> _playerStatuses;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            InitUI();
            UpdateScoreCounter();
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

            PlayerCharacter[] players = GameManager.Instance.GetPlayers();
            _playerStatuses = new List<PlayerStatus>();

            if (GameManager.Instance.GameState == GameManager.State.Play)
            {
                for (int i = 0; i < GameManager.Instance.PlayerCount; i++)
                {
                    PlayerStatus ps = Instantiate(_playerStatusPrefab, _playerStatusHandler);
                    ps.SetToolImage(_toolImages[(int) players[i].Tool]);
                    ps.SetPlayerName(players[i].name);
                    _playerStatuses.Add(ps);
                }

                _energyBar.value = 0f;
            }
        }

        /// <summary>
        /// Updates the score counter.
        /// </summary>
        public void UpdateScoreCounter()
        {
            if (_scoreText != null)
            {
                _scoreText.text = "Score: " + GameManager.Instance.CurrentScore;
            }
        }

        public void UpdateEnergyBar(float energy)
        {
            energy = Mathf.Clamp01(energy);
            _energyBar.value = energy;
        }

        public void UpdatePlayerToolImage(int playerNum, PlayerTool tool)
        {
            if (_playerStatuses != null && _playerStatuses[playerNum] != null)
            {
                _playerStatuses[playerNum].SetToolImage(_toolImages[(int) tool]);
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
