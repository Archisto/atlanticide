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

        public Image fadeScreen;

        public Image swapIcon;

        public Text levelName;

        [SerializeField]
        private Image[] _targetIcons;

        [SerializeField]
        private Vector2 _targetIconOffset = new Vector2(0, 50);

        [SerializeField]
        private List<Sprite> _toolImages;

        private Vector2 _canvasSize;
        private Vector2 _uiOffset;
        private Camera _camera;
        private InputController _input;
        private PauseScreen _pauseScreen;
        private PlayerCharacter[] _players;
        private List<PlayerStatus> _playerStatuses;
        private Vector3[] _targetPositions;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _canvasSize = GetComponent<Canvas>().pixelRect.size;
            _uiOffset = new Vector2(-0.5f * _canvasSize.x, -0.5f * _canvasSize.y);
            _camera = FindObjectOfType<CameraController>().GetComponent<Camera>();
            _input = FindObjectOfType<InputController>();
            _pauseScreen = GetComponentInChildren<PauseScreen>(true);

            if (_pauseScreen != null)
            {
                _pauseScreen.SetInput(_input);
            }

            _players = GameManager.Instance.GetPlayers();
            _playerStatuses = new List<PlayerStatus>();
            _targetPositions = new Vector3[_targetIcons.Length];

            InitUI();
            UpdateScoreCounter();
        }

        /// <summary>
        /// Updates the object after Update.
        /// </summary>
        private void LateUpdate()
        {
            if (!World.Instance.GamePaused)
            {
                UpdateTargetIcons();
            }
        }

        /// <summary>
        /// Initializes the UI.
        /// </summary>
        public void InitUI()
        {
            if (GameManager.Instance.GameState == GameManager.State.Play)
            {
                for (int i = 0; i < GameManager.Instance.PlayerCount; i++)
                {
                    PlayerStatus ps = Instantiate(_playerStatusPrefab, _playerStatusHandler);
                    ps.SetToolImage(_toolImages[(int) _players[i].Tool]);
                    ps.SetPlayerName(_players[i].name);
                    _playerStatuses.Add(ps);
                }

                UpdateEnergyBar(0f);
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

        public void ActivatePauseScreen(bool activate, string playerName)
        {
            _pauseScreen.gameObject.SetActive(activate);
            _pauseScreen.pausingPlayerText.text = playerName;
        }

        public void MoveUIObjToWorldPoint(Image uiObj,
                                          Vector3 worldPoint, 
                                          Vector2 screenSpaceOffset)
        {
            Vector2 viewPortPos = _camera.WorldToViewportPoint(worldPoint);
            Vector2 proportionalPosition = new Vector2
                (viewPortPos.x * _canvasSize.x, viewPortPos.y * _canvasSize.y);
            uiObj.transform.localPosition =
                proportionalPosition + _uiOffset + screenSpaceOffset;
        }

        public void ActivateTargetIcon(bool activate,
                                       int playerNum,
                                       Vector3 position)
        {
            if (playerNum < _targetIcons.Length)
            {
                Image targetIcon = _targetIcons[playerNum];
                _targetPositions[playerNum] = position;

                if (activate)
                {
                    MoveUIObjToWorldPoint(targetIcon,
                        position, _targetIconOffset);
                }

                targetIcon.gameObject.SetActive(activate);
            }
        }

        private void UpdateTargetIcons()
        {
            // TODO: The target icons' positioning looks a bit weird
            // on the far sides of the camera. Fix it.

            for (int i = 0; i < _targetIcons.Length; i++)
            {
                if (_targetIcons[i].gameObject.activeSelf)
                {
                    MoveUIObjToWorldPoint(_targetIcons[i],
                        _targetPositions[i], _targetIconOffset);
                }
            }
        }

        private void ClearTargetIcons()
        {
            for (int i = 0; i < _targetIcons.Length; i++)
            {
                if (_targetIcons[i].gameObject.activeSelf)
                {
                    _targetIcons[i].gameObject.SetActive(false);
                }
            }
        }

        public void ResetUI()
        {
            ClearTargetIcons();
        }
    }
}
