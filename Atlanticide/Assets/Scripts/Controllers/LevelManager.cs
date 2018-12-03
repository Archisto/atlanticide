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
        private TimeBar _timeBar;
        private Level _currentLevel;
        private Timer _levelTimer;
        private Timer _hurryUpWarnTimer;
        private Timer _scoreMultDecayTimer;
        private int _scoreMultiplier = 1;
        private float _pitch;
        private bool _flashLevelTimeBar;
        private bool _hasPlayedHurryUpSound;

        public bool LevelActive
        {
            get { return _levelTimer.Active; }
        }

        public Timer LevelTimer
        {
            get { return _levelTimer; }
        }

        public float LevelTimeElapsedRatio { get; private set; }

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

            orichalcumPickupPool = new Pool<OrichalcumPickup>(128, true, orichalcumPickupPrefab);
            stoneDebrisPool = new Pool<Debris>(128, true, stoneDebrisPrefabArray);
            woodDebrisPool = new Pool<Debris>(64, true, woodDebrisPrefabArray);
            terracottaDebrisPool = new Pool<Debris>(128, true, terracottaDebrisPrefabArray);
        }
        
        public void SetTimeBar(TimeBar timeBar)
        {
            _timeBar = timeBar;
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
                        LevelTimeElapsedRatio = _levelTimer.GetRatio();
                        UpdateHurryUpWarning();
                    }
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
                    _timeBar.FlashLevelTimeBar(_flashLevelTimeBar);
                    _hurryUpWarnTimer.Activate();

                    //if (_flashLevelTimeBar)
                    //{
                    //    SFXPlayer.Instance.Play(Sound.Clink);
                    //}
                    //else
                    //{
                    //    SFXPlayer.Instance.Play(Sound.Clink, pitch: 0.6f);
                    //}
                }

                if (!_hasPlayedHurryUpSound)
                {
                    SFXPlayer.Instance.Play(Sound.Trumpets);
                    SFXPlayer.Instance.PlayLooped(Sound.Clock_Ticking);

                    _hasPlayedHurryUpSound = true;
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
            LevelTimeElapsedRatio = 1f;
            GameManager.Instance.EndLevel(false);
            StopTickingSound();
        }

        public void ResetLevel()
        {
            _levelTimer.Reset();
            LevelTimeElapsedRatio = 0f;
            _scoreMultDecayTimer.Reset();
            ResetHurryUpWarning();
            ResetScoreMultiplier();
            ResetPickupSFX();
            orichalcumPickupPool.DeactivateAllObjects();
            stoneDebrisPool.DeactivateAllObjects();
            woodDebrisPool.DeactivateAllObjects();
            terracottaDebrisPool.DeactivateAllObjects();
        }

        private void ResetHurryUpWarning()
        {
            _hurryUpWarnTimer.Reset();
            _timeBar.FlashLevelTimeBar(false);
            _flashLevelTimeBar = false;
            _hasPlayedHurryUpSound = false;
            StopTickingSound();
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

        public void StopTickingSound()
        {
            SFXPlayer.Instance.StopIndividualSFX("Clock Ticking");
        }
    }
}
