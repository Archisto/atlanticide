using System;
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

        public enum State
        {
            MainMenu = 0,
            Play = 1,
            LevelEnd = 2
        }

        public enum TransitionPhase
        {
            None = 0,
            ResetingLevel = 1,
            ExitingScene = 2,
            StartingScene = 3
        }

        private Level _level;
        private PlayerCharacter _playerPrefab;
        private PlayerCharacter[] _players;
        private PlayerTool[] _playerTools;
        private NonPlayerCharacter[] _npcs;
        private UIController _ui;
        private LevelObject[] _levelObjects;
        private Transform[] _telegrabs;
        private InputController _input;
        private SettingsManager _settings;
        private FadeToColor _fade;
        private bool _updateAtSceneStart = true;
        private bool _freshGameStart = true;
        private string _nextSceneName;
        private int _deadPlayerCount;

        public State GameState { get; set; }

        public TransitionPhase Transition { get; set; }

        public int PlayerCount { get; private set; }

        public int DeadPlayerCount
        {
            get
            {
                return _deadPlayerCount;
            }
            set
            {
                _deadPlayerCount = value;
                if (LevelFailed)
                {
                    StartLevelReset();
                }
            }
        }

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
                return Transition == TransitionPhase.ExitingScene ||
                    Transition == TransitionPhase.StartingScene;
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

        public bool LevelFailed
        {
            get
            {
                return DeadPlayerCount == PlayerCount;
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
                _playerTools = new PlayerTool[MaxPlayers];
                _updateAtSceneStart = true;
            }
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (Transition == TransitionPhase.ExitingScene
                && _fade.FadedOut)
            {
                LoadScene(_nextSceneName);
            }
            else if (Transition == TransitionPhase.ResetingLevel
                     && _fade.FadedOut)
            {
                Transition = TransitionPhase.None;
                ActivatePauseScreen(false, "");
                ResetLevel();
            }

            if (_updateAtSceneStart)
            {
                LevelStartInit();
            }
        }

        private void LevelStartInit()
        {
            if (GameState == State.Play)
            {
                Debug.Log("Level starts");
                SetPlayerTools(_freshGameStart);
            }
            else
            {
                Debug.Log("Menu starts");
            }

            _freshGameStart = false;
            _updateAtSceneStart = false;
        }

        private void InitSettings()
        {
            _settings = new SettingsManager();
        }

        private void InitScene()
        {
            World.Instance.Init();
            InitUI();
            InitFade();
            InitInput();

            if (GameState == State.Play)
            {
                InitLevel();
                InitPlayers();
                InitNPCs();
                _levelObjects = FindObjectsOfType<LevelObject>();
            }

            Transition = TransitionPhase.None;
            _updateAtSceneStart = true;
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
                _players[i].transform.position = _level.GetSpawnPoint(i);
                SetPlayerTool(_players[i], i + 1);
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
            Debug.Log("New player count: " + PlayerCount);

            for (int i = 0; i < MaxPlayers; i++)
            {
                bool activate = (i < PlayerCount);
                bool alreadyActive = _players[i].gameObject.activeSelf;

                if (!activate && alreadyActive)
                {
                    _players[i].CancelActions();
                    _players[i].Respawn();
                }

                _players[i].gameObject.SetActive(activate);
            }
        }

        /// <summary>
        /// Initializes the UI.
        /// </summary>
        private void InitUI()
        {
            _ui = FindObjectOfType<UIController>();
            if (_ui == null)
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
                _fade.Init(_ui.GetFade());

                if (Transition == TransitionPhase.StartingScene)
                {
                    _fade.StartFadeIn();
                }
            }
        }

        /// <summary>
        /// Initializes the input controller.
        /// </summary>
        private void InitInput()
        {
            _input = FindObjectOfType<InputController>();
            if (_input == null)
            {
                Debug.LogError(Utils.GetObjectMissingString("InputController"));
            }
        }

        public UIController GetUI()
        {
            Debug.Log(_ui);
            return _ui;
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

        public void ChangeScore(int score)
        {
            SetScore(CurrentScore + score);
        }

        public void SetScore(int score)
        {
            CurrentScore = score;
            _ui.UpdateScoreCounter();
        }

        /// <summary>
        /// Returns the player character closest to the given position.
        /// The player character must be alive.
        /// </summary>
        /// <param name="position">A position</param>
        /// <returns>A player character.</returns>
        public PlayerCharacter GetClosestAblePlayer(Vector3 position)
        {
            PlayerCharacter result = null;
            float closestDist = 0f;

            for (int i = 0; i < PlayerCount; i++)
            {
                float distance = Vector3.Distance(position, _players[i].transform.position);
                if (!_players[i].IsDead && (result == null || distance < closestDist))
                {
                    result = _players[i];
                    closestDist = distance;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a list of player characters within range from position
        /// </summary>
        /// <param name="position">position of the point players are searched from</param>
        /// <param name="range">range of the area players are searched from</param>
        /// <returns></returns>
        public PlayerCharacter[] GetPlayersWithinRange(Vector3 position, float range)
        {
            PlayerCharacter[] result = new PlayerCharacter[MaxPlayers];
            int playersFound = 0;

            for (int i = 0; i < PlayerCount; i++)
            {

                if(!_players[i].IsDead && Vector3.Distance(position, _players[i].transform.position) <= range)
                {
                    result[playersFound] = _players[i];
                    playersFound++;
                }
            }

            return result;
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

            for (int i = 0; i < PlayerCount; i++)
            {
                if (!_players[i].IsDead &&
                    Vector3.Distance(position, _players[i].transform.position) <= range)
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
        /// Returns whether the given player characters are within range of each other.
        /// </summary>
        /// <param name="player">A player character</param>
        /// <param name="otherPlayer">Another player character</param>
        /// <param name="range">The range</param>
        /// <returns>Are the given player characters within range of each other
        /// </returns>
        public bool PlayersAreWithinRangeOfEachOther(
            PlayerCharacter player, PlayerCharacter otherPlayer, float range)
        {
            return Vector3.Distance
                (player.transform.position, otherPlayer.transform.position)
                <= range;
        }

        /// <summary>
        /// Returns the first player character in the array.
        /// </summary>
        /// <param name="includeDead">Are dead players included</param>
        /// <returns>A player character</returns>
        public PlayerCharacter GetAnyPlayer(bool includeDead)
        {
            foreach (PlayerCharacter pc in _players)
            {
                if (includeDead || !pc.IsDead)
                {
                    return pc;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the first player character that isn't the one given.
        /// Can return only a living character if necessary.
        /// </summary>
        /// <param name="invalidPlayer">A player that won't be returned</param>
        /// <param name="includeDead">Are dead players included</param>
        /// <returns>A player character other than the one given</returns>
        public PlayerCharacter GetAnyOtherPlayer(PlayerCharacter invalidPlayer, bool includeDead)
        {
            foreach (PlayerCharacter pc in _players)
            {
                if (pc != invalidPlayer && (includeDead || !pc.IsDead))
                {
                    return pc;
                }
            }

            return null;
        }

        public PlayerCharacter GetPlayerWithTool(PlayerTool tool, bool includeDead)
        {
            foreach (PlayerCharacter pc in _players)
            {
                if (pc.Tool == tool && (includeDead || !pc.IsDead))
                {
                    return pc;
                }
            }

            return null;
        }

        /// <summary>
        /// Invokes an action on each active player character.
        /// </summary>
        /// <param name="action">An action</param>
        public void ForEachActivePlayerChar(Action<PlayerCharacter> action)
        {
            for (int i = 0; i < PlayerCount; i++)
            {
                action(_players[i]);
            }
        }

        /// <summary>
        /// Saves the tools currently in use or gives each player character a saved tool.
        /// </summary>
        /// <param name="saveCurrent">Should the tools currently in use be saved</param>
        public void SetPlayerTools(bool saveCurrent)
        {
            for (int i = 0; i < _players.Length; i++)
            {
                if (saveCurrent)
                {
                    _playerTools[i] = _players[i].Tool;
                    //Debug.Log(string.Format("Tool saved: {0} has {1}", _players[i].name, _playerTools[i]));
                }
                else
                {
                    SetPlayerTool(_players[i], _playerTools[i]);
                    _ui.UpdatePlayerToolImage(i, _playerTools[i]);
                    //Debug.Log(string.Format("Tool set: {0} gets {1}", _players[i].name, _playerTools[i]));
                }
            }
        }

        /// <summary>
        /// Sets the given players tool.
        /// </summary>
        /// <param name="player">A player</param>
        /// <param name="tool">A tool</param>
        public void SetPlayerTool(PlayerCharacter player, PlayerTool tool)
        {
            if (player.Tool == PlayerTool.EnergyCollector)
            {
                player.EnergyCollector.ResetEnergyCollector();
            }
            else if (player.Tool == PlayerTool.Shield)
            {
                player.Shield.CancelBash();
                player.Shield.ActivateInstantly(false);
            }

            player.Tool = tool;

            // The UI controller first updates the tool images in its Start method.
            // This is for subsequent tool swaps.
            if (!_updateAtSceneStart)
            {
                _ui.UpdatePlayerToolImage(player.ID, player.Tool);
            }

            // Gives the same amount of charges to the new energy
            // collector player as the previous had before the tool swap
            if (World.Instance.CurrentEnergyCharges > 0 &&
                player.Tool == PlayerTool.EnergyCollector)
            {
                player.EnergyCollector.SetCharges(World.Instance.CurrentEnergyCharges, false);
            }
        }

        /// <summary>
        /// Sets the given players tool.
        /// </summary>
        /// <param name="player">A player</param>
        /// <param name="toolNum">A tool's enum number</param>
        public void SetPlayerTool(PlayerCharacter player, int toolNum)
        {
            if (Enum.IsDefined(typeof(PlayerTool), toolNum))
            {
                SetPlayerTool(player, (PlayerTool) toolNum);
            }
            else
            {
                Debug.LogWarning("Enum PlayerTool does not have a value at index " + toolNum);
            }
        }

        /// <summary>
        /// Starts reseting level with a fade-out.
        /// </summary>
        public void StartLevelReset()
        {
            Transition = TransitionPhase.ResetingLevel;
            _fade.StartFadeOut(LevelFailed);
            Debug.Log("Restarting level");
        }

        /// <summary>
        /// Resets the current level.
        /// </summary>
        public void ResetLevel()
        {
            World.Instance.ResetWorld();
            _input.ResetInput();
            _players.ForEach(pc => pc.CancelActions());
            _players.ForEach(pc => pc.RespawnPosition = _level.GetSpawnPoint(pc.ID));
            ForEachActivePlayerChar(pc => pc.Respawn());
            _npcs.ForEach(npc => npc.Respawn());
            _levelObjects.ForEach(obj => obj.ResetObject());
            _level.ResetLevel();
            _ui.ResetUI();
            World.Instance.SetEnergyChargesAndUpdateUI(0);
            SetScore(0);
            DeadPlayerCount = 0;
            _fade.StartFadeIn();
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
            if (Transition != TransitionPhase.ExitingScene)
            {
                Debug.Log("Loading scene: " + sceneName);

                if (GameState == State.Play)
                {
                    _players.ForEach(p => p.CancelActions());
                    World.Instance.ResetWorld();
                }

                Transition = TransitionPhase.ExitingScene;
                _nextSceneName = sceneName;
                _fade.StartFadeOut(false);
            }
        }

        /// <summary>
        /// Loads a scene.
        /// </summary>
        /// <param name="sceneName">The scene's name</param>
        private void LoadScene(string sceneName)
        {
            Transition = TransitionPhase.StartingScene;
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
            if (activate || Transition == TransitionPhase.None)
            {
                _ui.ActivatePauseScreen(activate, playerName);
            }
        }

        private void OnDisable()
        {
            SceneManager.activeSceneChanged -= InitScene;
        }
    }
}
