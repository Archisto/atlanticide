using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Atlanticide.UI;

namespace Atlanticide
{
    public class GameManager : MonoBehaviour
    {
        #region Statics
        private static GameManager instance;
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GameManager>();

                    if (instance == null)
                    {
                        instance = Resources.Load<GameManager>("GameManager");
                    }
                }

                return instance;
            }
        }
        #endregion Statics

        public const int MaxPlayers = 2;
        public const int LastLevel = 1;

        private const string MainMenuKey = "MainMenu";
        private const string LevelKey = "Level";

        private Level _level;
        private PlayerCharacter _playerPrefab;
        private PlayerCharacter[] _players;
        private NonPlayerCharacter[] _npcs;
        private UIController _UI;
        private LevelObject[] _levelObjects;
        private Transform[] _telegrabs;
        private SettingsManager _settings;
        private FadeToColor _fade;
        private bool _exitingScene;
        private bool _startingScene;
        private string _nextSceneName;

        public enum State
        {
            MainMenu = 0,
            Play = 1,
            LevelEnd = 2
        }

        public State GameState { get; set; }

        public int PlayerCount { get; private set; }

        public int CurrentLevel { get; set; }

        public int CurrentScore { get; private set; }

        public SettingsManager Settings
        {
            get { return _settings; }
        }

        public bool SceneChanging
        {
            get
            {
                return _exitingScene || _startingScene;
            }
        }

        public bool FadeActive
        {
            get
            {
                return _fade.Active;
            }
        }

        public bool PlayReady
        {
            get
            {
                return GameState == State.Play &&
                       !SceneChanging;
            }
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            if (instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
                SceneManager.activeSceneChanged += InitScene;
                InitSettings();

                if (SceneManager.GetActiveScene().name == MainMenuKey)
                {
                    GameState = State.MainMenu;
                }
                else
                {
                    GameState = State.Play;
                }

                PlayerCount = 2;
                CurrentLevel = 1;
            }
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (_exitingScene && _fade.FadedOut)
            {
                LoadScene(_nextSceneName);
            }
        }

        private void InitSettings()
        {
            _settings = new SettingsManager();
        }

        private void InitScene()
        {
            InitUI();
            InitFade();

            if (GameState == State.Play)
            {
                InitLevel();
                InitPlayers();
                InitNPCs();
                _levelObjects = FindObjectsOfType<LevelObject>();
            }

            _startingScene = false;
        }

        /// <summary>
        /// Initializes the level controller.
        /// </summary>
        private void InitLevel()
        {
            _level = FindObjectOfType<Level>();
            if (_level == null)
            {
                Debug.LogError(Utils.GetFieldNullString("Level controller"));
            }
        }

        /// <summary>
        /// Initializes the player characters.
        /// </summary>
        private void InitPlayers()
        {
            _players = new PlayerCharacter[MaxPlayers];
            _playerPrefab = Resources.Load<PlayerCharacter>("PlayerCharacter");
            CreatePlayers();
            ActivatePlayers(PlayerCount);

            // Test
            _telegrabs = new Transform[MaxPlayers];
        }

        /// <summary>
        /// Initializes the non-player characters.
        /// </summary>
        private void InitNPCs()
        {
            _npcs = FindObjectsOfType<NonPlayerCharacter>();
        }

        /// <summary>
        /// Creates the player characters.
        /// </summary>
        /// <param name="playerCount">The player count</param>
        private void CreatePlayers()
        {
            for (int i = 0; i < MaxPlayers; i++)
            {
                _players[i] = Instantiate(_playerPrefab);
                _players[i].ID = i;
                _players[i].name = "Player " + (i + 1);
                _players[i].Input = new PlayerInput(i);
                _players[i].EnergyBar = _UI.GetEnergyBar(i);
                _players[i].transform.position = _level.GetSpawnPoint(i);
            }
        }

        /// <summary>
        /// Activates a player character for each player and deactives the rest.
        /// </summary>
        /// <param name="newPlayerCount">The player count</param>
        public void ActivatePlayers(int newPlayerCount)
        {
            // TODO: Are player characters set inactive when they die?
            // Do they die? Can player count be changed at any point?
            // Setting a dead PC inactive and then active again
            // should not make them respawn.

            PlayerCount = (newPlayerCount < MaxPlayers ? newPlayerCount : MaxPlayers);
            for (int i = 0; i < MaxPlayers; i++)
            {
                _players[i].gameObject.SetActive(i < PlayerCount);
            }
        }

        /// <summary>
        /// Initializes the UI.
        /// </summary>
        private void InitUI()
        {
            _UI = FindObjectOfType<UIController>();
            if (_UI == null)
            {
                Debug.LogError(Utils.GetObjectMissingString("UIController"));
            }
        }

        /// <summary>
        /// Initializes fading to a color.
        /// </summary>
        private void InitFade()
        {
            _fade = FindObjectOfType<FadeToColor>();
            if (_fade != null)
            {
                _fade.Init(_UI.GetFade());

                if (_startingScene)
                {
                    _fade.StartFadeIn();
                }
            }
        }

        public UIController GetUI()
        {
            return _UI;
        }

        public PlayerCharacter[] GetPlayers()
        {
            return _players;
        }

        public NonPlayerCharacter[] GetNPCs()
        {
            return _npcs;
        }

        public Transform[] GetTelegrabs()
        {
            return _telegrabs;
        }

        public void UpdateTelegrab(int playerNum, Transform telegrab, bool active)
        {
            if (active)
            {
                _telegrabs[playerNum] = telegrab;
            }
            else
            {
                _telegrabs[playerNum] = null;
            }
        }

        public void UpdateScore(int score)
        {
            SetScore(CurrentScore + score);
        }

        public void SetScore(int score)
        {
            CurrentScore = score;
            _UI.UpdateUI();
        }

        /// <summary>
        /// Checks how many players there are within range.
        /// </summary>
        /// <param name="position">A position</param>
        /// <param name="range">Allowed distance from the position</param>
        /// <returns>How many players are there within range.</returns>
        public int PlayersWithinRange(Vector3 position, float range)
        {
            int result = 0;

            foreach (PlayerCharacter pc in _players)
            {
                if (!pc.IsDead &&
                    Vector3.Distance(position, pc.transform.position) <= range)
                {
                    result++;
                }
            }

            return result;
        }

        /// <summary>
        /// Checks if there is one or all players within range.
        /// </summary>
        /// <param name="position">A position</param>
        /// <param name="range">Allowed distance from the position</param>
        /// <param name="allPlayers">Are all players needed</param>
        /// <returns>Are there any/all players in range.</returns>
        public bool PlayersAreWithinRange(Vector3 position, float range, bool allPlayers)
        {
            int playersWithinRange = PlayersWithinRange(position, range);

            if (allPlayers)
            {
                return playersWithinRange == PlayerCount;
            }
            else
            {
                return playersWithinRange >= 1;
            }
        }

        /// <summary>
        /// Gets the first player within range.
        /// </summary>
        /// <param name="position">A position</param>
        /// <param name="range">Allowed distance from the position</param>
        /// <returns>A player character within range.</returns>
        public PlayerCharacter GetPlayerWithinRange(Vector3 position, float range)
        {
            foreach (PlayerCharacter pc in _players)
            {
                if (!pc.IsDead &&
                    Vector3.Distance(position, pc.transform.position) <= range)
                {
                    return pc;
                }
            }

            return null;
        }

        /// <summary>
        /// Resets the current level.
        /// </summary>
        public void ResetLevel()
        {
            World.Instance.ResetWorld();
            _players.ForEach(pc => pc.Respawn());
            _npcs.ForEach(npc => npc.Respawn());
            _levelObjects.ForEach(obj => obj.ResetObject());
            SetScore(0);
        }

        /// <summary>
        /// Loads the main menu scene.
        /// </summary>
        public void LoadMainMenu()
        {
            StartLoadingScene(MainMenuKey);
            GameState = State.MainMenu;
        }

        /// <summary>
        /// Loads a level.
        /// </summary>
        /// <param name="levelNum">The level number</param>
        public void LoadLevel(int levelNum)
        {
            if (!SceneChanging)
            {
                if (levelNum >= 1 && levelNum <= LastLevel)
                {
                    CurrentLevel = levelNum;
                    StartLoadingScene(LevelKey + levelNum);
                    GameState = State.Play;
                }
                else
                {
                    Debug.LogWarning(string.Format("Invalid level number ({0}).", levelNum));
                }
            }
            else
            {
                Debug.LogWarning("Scene is already changing.");
            }
        }

        /// <summary>
        /// Testing.
        /// Loads a level: Lauri's Colosseum.
        /// </summary>
        public void LoadTestLevel()
        {
            StartLoadingScene("Lauri's Colosseum");
            GameState = State.Play;
        }

        /// <summary>
        /// Starts loading a scene with a fade-out.
        /// </summary>
        /// <param name="sceneName">The scene's name</param>
        public void StartLoadingScene(string sceneName)
        {
            if (!_exitingScene)
            {
                Debug.Log("Loading scene: " + sceneName);

                if (GameState == State.Play)
                {
                    _players.ForEach(p => p.CancelActions());
                }

                _exitingScene = true;
                _nextSceneName = sceneName;
                _fade.StartFadeOut();
            }
        }

        /// <summary>
        /// Loads a scene.
        /// </summary>
        /// <param name="sceneName">The scene's name</param>
        private void LoadScene(string sceneName)
        {
            _exitingScene = false;
            _startingScene = true;
            SceneManager.LoadScene(sceneName);
        }

        private void InitScene(Scene prev, Scene next)
        {
            if (this != instance)
            {
                return;
            }

            InitScene();
        }

        public void ActivatePauseScreen(bool activate, string playerName)
        {
            if (activate || !_exitingScene)
            {
                _UI.ActivatePauseScreen(activate, playerName);
            }
        }

        private void OnDisable()
        {
            SceneManager.activeSceneChanged -= InitScene;
        }
    }
}
