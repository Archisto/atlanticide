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
        private Text _scoreMultiplierText;

        public Image fadeScreen;

        public Text levelName;

        [SerializeField]
        private Image[] _targetIcons;

        [SerializeField]
        private Vector2 _targetIconOffset = new Vector2(0, 50);

        private Canvas _canvas;
        private Vector2 _canvasSize;
        private Vector2 _uiOffset;
        private Camera _camera;
        private InputController _input;
        private PauseScreen _pauseScreen;
        private LevelEndScreen _levelEndScreen;
        private PlayerCharacter[] _players;
        private List<PlayerStatus> _playerStatuses;
        private Vector3[] _targetPositions;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _canvas = GetComponent<Canvas>();
            UpdateCanvasSize();
            _camera = FindObjectOfType<CameraController>().GetComponent<Camera>();
            _input = FindObjectOfType<InputController>();
            _pauseScreen = GetComponentInChildren<PauseScreen>(true);
            _levelEndScreen = GetComponentInChildren<LevelEndScreen>(true);

            if (_pauseScreen != null)
            {
                _pauseScreen.Input = _input;
            }

            _players = GameManager.Instance.GetPlayers();
            _playerStatuses = new List<PlayerStatus>();
            _targetPositions = new Vector3[_targetIcons.Length];

            InitUI();
            SetScoreCounterValue(0);
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
            //if (GameManager.Instance.GameState == GameManager.State.Play)
            //{
            //    for (int i = 0; i < GameManager.Instance.PlayerCount; i++)
            //    {
            //        CreatePlayerStatusUIElement(_players[i]);
            //    }
            //}
        }

        private PlayerStatus CreatePlayerStatusUIElement(PlayerCharacter player)
        {
            PlayerStatus ps = Instantiate(_playerStatusPrefab, _playerStatusHandler);
            ps.SetPlayerName(player.name);
            _playerStatuses.Add(ps);
            return ps;
        }

        public void UpdateCanvasSize()
        {
            _canvasSize = _canvas.pixelRect.size;
            _uiOffset = new Vector2(-0.5f * _canvasSize.x, -0.5f * _canvasSize.y);
        }

        /// <summary>
        /// Sets the value of the score counter.
        /// </summary>
        public void SetScoreCounterValue(int score)
        {
            if (_scoreText != null)
            {
                _scoreText.text = "Score: " + score;
            }
        }

        /// <summary>
        /// Sets the value of the score multiplier counter.
        /// </summary>
        public void SetMultiplierCounterValue(int multiplier)
        {
            if (_scoreMultiplierText != null)
            {
                _scoreMultiplierText.text = "Multiplier: " + multiplier;
            }
        }

        public void ActivatePauseScreen(bool activate, string playerName)
        {
            UpdateCanvasSize();

            if (_pauseScreen != null)
            {
                _pauseScreen.pausingPlayerText.text = playerName;
                _pauseScreen.Activate(activate);
            }
        }

        public void ActivateLevelEndScreen(bool activate, bool levelWon = false)
        {
            _levelEndScreen.Activate(activate, levelWon);
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
