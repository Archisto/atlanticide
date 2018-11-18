using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Atlanticide.UI;

namespace Atlanticide
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField]
        private Transform _player1SpawnPoint;

        [SerializeField]
        private Transform _player2SpawnPoint;

        [Header("LEVEL TIME")]

        [SerializeField]
        private float _levelTime = 120f;

        [SerializeField]
        private float _hurryUpWarnTime = 100f;

        [SerializeField]
        private float _hurryUpWarnFlashTime = 0.5f;

        [Header("SCORE")]

        public int requiredScore = 1000;

        [SerializeField]
        private float _scoreMultiplierDecayTime = 1f;

        [SerializeField]
        private float _minPitch = 0.3f;

        [SerializeField]
        private float _maxPitch = 2f;

        [SerializeField]
        private float _pitchRise = 0.1f;

        [Header("POOLS")]

        // Orichalcum pickup prefab and pool.
        public OrichalcumPickup orichalcumPickupPrefab;
        public Pool<OrichalcumPickup> orichalcumPickupPool;

        // Destructible GameObject debris prefabs and pools.
        public Debris[] stoneDebrisPrefabArray;
        public Pool<Debris> stoneDebrisPool;
        public Debris[] woodDebrisPrefabArray;
        public Pool<Debris> woodDebrisPool;
        public Debris[] terracottaDebrisPrefabArray;
        public Pool<Debris> terracottaDebrisPool;

        private UIController _ui;
        private Level _currentLevel;
        private Timer _levelTimer;
        private Timer _hurryUpWarnTimer;
        private Timer _scoreMultDecayTimer;
        private float _levelTimeElapsedRatio;
        private int _scoreMultiplier = 1;
        private float _pitch;
        private bool _flashLevelTimeBar;

        public bool LevelActive
        {
            get { return _levelTimer.Active; }
        }

        public int ScoreMultiplier
        {
            get { return _scoreMultiplier; }
        }

        public bool TimeIsRunningOut
        {
            get
            {
                return _levelTimer.elapsedTime > _hurryUpWarnTime;
            }
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            if (_player1SpawnPoint == null)
            {
                Debug.LogError(Utils.GetFieldNullString("Player 1 spawn point"));
            }
            if (_player2SpawnPoint == null)
            {
                Debug.LogError(Utils.GetFieldNullString("Player 2 spawn point"));
            }

            _ui = GameManager.Instance.GetUI();
            _currentLevel = GameManager.Instance.CurrentLevel;
            _ui.levelName.text = _currentLevel.LevelName;
            _levelTimer = new Timer(_levelTime, true);
            _hurryUpWarnTimer = new Timer(_hurryUpWarnFlashTime, true);
            _scoreMultDecayTimer = new Timer(_scoreMultiplierDecayTime, true);

            orichalcumPickupPool = new Pool<OrichalcumPickup>(64, true, orichalcumPickupPrefab);
            stoneDebrisPool = new Pool<Debris>(32, true, stoneDebrisPrefabArray);
            woodDebrisPool = new Pool<Debris>(32, true, woodDebrisPrefabArray);
            terracottaDebrisPool = new Pool<Debris>(32, true, terracottaDebrisPrefabArray);
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (!World.Instance.GamePaused)
            {
                if (LevelActive)
                {
                    if (_scoreMultDecayTimer.Check())
                    {
                        ResetScoreMultiplier();
                        ResetPickupSFX();
                    }

                    if (_levelTimer.Check())
                    {
                        LoseLevel();
                    }
                    else
                    {
                        _levelTimeElapsedRatio = _levelTimer.GetRatio();
                        UpdateHurryUpWarning();
                    }

                    _ui.UpdateLevelTimeBar(_levelTimeElapsedRatio);
                }
            }
        }

        private void UpdateHurryUpWarning()
        {
            if (TimeIsRunningOut)
            {
                if (_hurryUpWarnTimer.Check() ||
                    !_hurryUpWarnTimer.Active)
                {
                    _flashLevelTimeBar = !_flashLevelTimeBar;
                    _ui.FlashLevelTimeBar(_flashLevelTimeBar);
                    _hurryUpWarnTimer.Activate();
                }
            }
        }

        public Vector3 GetSpawnPoint(int playerNum)
        {
            switch (playerNum)
            {
                case 0:
                {
                    return _player1SpawnPoint.position;
                }
                case 1:
                {
                    return _player2SpawnPoint.position;
                }
                default:
                {
                    return Vector3.zero;
                }
            }
        }

        public void IncreaseMultiplier()
        {
            _scoreMultiplier++;
            _scoreMultDecayTimer.Activate();
        }

        public void PlayCollectSound()
        {
            SFXPlayer.Instance.Play(Sound.Clink, pitch: _pitch);
            _pitch += _pitchRise;
            if (_pitch > _maxPitch)
            {
                ResetPickupSFX();
            }
        }

        public bool EnoughScore(int score)
        {
            return score >= requiredScore;
        }

        public void StartLevel()
        {
            ResetLevel();
            _levelTimer.Activate();
        }

        public void LoseLevel()
        {
            _levelTimer.Reset();
            _levelTimeElapsedRatio = 1f;
            _ui.FlashLevelTimeBar(true);
            GameManager.Instance.EndLevel(false);
        }

        public void ResetLevel()
        {
            _levelTimer.Reset();
            _levelTimeElapsedRatio = 0f;
            _scoreMultDecayTimer.Reset();
            ResetHurryUpWarning();
            ResetScoreMultiplier();
            ResetPickupSFX();
        }

        private void ResetHurryUpWarning()
        {
            _hurryUpWarnTimer.Reset();
            _ui.FlashLevelTimeBar(false);
            _flashLevelTimeBar = false;
        }

        private void ResetScoreMultiplier()
        {
            _scoreMultiplier = 1;
            _ui.SetMultiplierCounterValue(1);
        }

        private void ResetPickupSFX()
        {
            _pitch = _minPitch;
        }
    }
}
