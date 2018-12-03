using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Atlanticide.Persistence;
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
        public const int LevelCount = 2;

        private const string PressStartKey = "PressStart";
        private const string MainMenuKey = "MainMenu";
        private const string LevelKey = "Level";

        public enum State
        {
            PressStart = 0,
            MainMenu = 1,
            Play = 2,
            LevelEnd = 3
        }

        public enum TransitionPhase
        {
            None = 0,
            ResetingScene = 1,
            ExitingScene = 2,
            StartingScene = 3
        }

        #region Fields

        private LevelManager _levelManager;
        private PlayerCharacter _playerPrefab;
        private PlayerCharacter[] _players;
        private PlayerTool[] _playerTools;
        private InputDevice[] _inputDevices;
        private NonPlayerCharacter[] _npcs;
        private UIController _ui;
        private List<Level> _levels;
        private LevelObject[] _levelObjects;
        private InputController _input;
        private SettingsManager _settings;
        private SaveSystem _saveSystem;
        private FadeToColor _fade;
        private AudioSource _levelResultAudioSrc;
        private bool _updateAtSceneStart = true;
        private bool _freshGameStart = true;
        private string _nextSceneName;
        private int _deadPlayerCount;

        public State GameState { get; set; }

        public TransitionPhase SceneTransition { get; set; }

        public int PlayerCount { get; private set; }

        public int DeadPlayerCount
        {
            get
            {
                return _deadPlayerCount;
            }
            set
            {
                if (value >= 0)
                {
                    _deadPlayerCount = value;
                    if (AllPlayersDied)
                    {
                        EndLevel(false);
                    }
                }
            }
        }

        public LevelManager LevelManager
        {
            get { return _levelManager; }
        }

        public int LevelsUnlocked { get; private set; }

        public Level CurrentLevel { get; private set; }

        public int CurrentScore { get; private set; }

        public SettingsManager Settings
        {
            get { return _settings; }
        }

        public bool SceneChanging
        {
            get
            {
                return SceneTransition == TransitionPhase.ExitingScene ||
                    SceneTransition == TransitionPhase.StartingScene;
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

        public bool AllPlayersDied
        {
            get
            {
                return DeadPlayerCount == PlayerCount;
            }
        }

        public PlayerInput MenuPlayerInput { get; set; }

        #endregion Fields

        /// <summary>
        /// Initializes the singleton instance.
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
                Init();
            }
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (SceneTransition == TransitionPhase.ExitingScene
                && _fade.FadedOut)
            {
                LoadScene(_nextSceneName);
            }
            else if (SceneTransition == TransitionPhase.ResetingScene
                     && _fade.FadedOut)
            {
                SceneTransition = TransitionPhase.None;
                World.Instance.PauseGame(false);
                ResetLevel();
            }
            else if (GameState == State.Play &&
                (_levelManager != null && !_levelManager.LevelActive))
            {
                StartLevel();
            }

            if (_updateAtSceneStart)
            {
                SceneStartInit();
            }
        }

        #region Initialization

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Init()
        {
            _saveSystem = new SaveSystem(new JSONPersistence(SavePath));
            InitSettings();
            SceneManager.activeSceneChanged += InitScene;

            if (SceneManager.GetActiveScene().name.Equals(MainMenuKey))
            {
                GameState = State.MainMenu;
            }
            else if (SceneManager.GetActiveScene().name.Equals(PressStartKey))
            {
                GameState = State.PressStart;
            }
            else
            {
                GameState = State.Play;
            }

            InitLevels();
            PlayerCount = 2;
            _playerTools = new PlayerTool[MaxPlayers];
            _inputDevices = new InputDevice[MaxPlayers];
            for (int i = 0; i < _inputDevices.Length; i++)
            {
                _inputDevices[i] = InputDevice.None;
            }

            _updateAtSceneStart = true;

            MenuPlayerInput = new PlayerInput(InputDevice.Keyboard);
        }

        private void SceneStartInit()
        {
            if (GameState == State.Play)
            {
                _freshGameStart = false;
                World.Instance.PauseGame(false);

                //if (RequiredScore == 0)
                //{
                //    RequiredScore = _levelManager.requiredScore;
                //    _levelManager.StartLevel();
                //}

                Debug.Log("[GameManager] Level scene init");
            }
            else
            {
                Debug.Log("[GameManager] Menu scene init");
            }

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
            InitInputController();
            MusicPlayer.Instance.fadeTime = _fade.FadeOutTime;

            if (GameState == State.Play)
            {
                InitLevelManager();
                InitPlayers();
                InitNPCs();
                _levelObjects = FindObjectsOfType<LevelObject>();
            }

            SceneTransition = TransitionPhase.None;
            _updateAtSceneStart = true;
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
                _fade.Init(_ui.fadeScreen);

                if (SceneTransition == TransitionPhase.StartingScene)
                {
                    _fade.StartFadeIn(true);
                }
            }
        }

        /// <summary>
        /// Initializes the input controller.
        /// </summary>
        private void InitInputController()
        {
            _input = FindObjectOfType<InputController>();
            if (_input == null)
            {
                Debug.LogError(Utils.GetObjectMissingString("InputController"));
            }
        }

        /// <summary>
        /// Initializes the level manager.
        /// </summary>
        private void InitLevelManager()
        {
            _levelManager = FindObjectOfType<LevelManager>();
            if (_levelManager == null)
            {
                Debug.LogError(Utils.GetObjectMissingString("LevelManager"));
            }
        }

        private void InitLevels()
        {
            _levels = new List<Level>();
            _levels.Add(new Level(0, "LinkProto", "Debug"));
            _levels.Add(new Level(1, "LinkProto Lauri", "Tutorial"));
            _levels.Add(new Level(2, "LinkProto", "Challenge"));

            LevelsUnlocked = 1;
            CurrentLevel = _levels[0];
        }

        /// <summary>
        /// Initializes the non-player characters.
        /// </summary>
        private void InitNPCs()
        {
            _npcs = FindObjectsOfType<NonPlayerCharacter>();
        }

        #endregion Initialization

        #region Player Setup

        /// <summary>
        /// Initializes the player characters.
        /// </summary>
        private void InitPlayers()
        {
            _players = new PlayerCharacter[MaxPlayers];
            _playerPrefab = Resources.Load<PlayerCharacter>
                ("New3_P1_PlayerCharacter");
            CreatePlayers();
            ActivatePlayers(PlayerCount);
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

                PlayerInput input = GetPlayerInput(i);
                _players[i].Input = input;
                Debug.Log(_players[i].name + " input: " + input.InputDevice);

                _players[i].transform.position = _levelManager.GetSpawnPoint(i);

                string creationMsg = "created";
                if (i >= PlayerCount)
                {
                    _players[i].gameObject.SetActive(false);
                    creationMsg = "created but set inactive";
                }

                Debug.Log("Player " + (i + 1) + " " + creationMsg);
            }

            if (_freshGameStart)
            {
                SaveInputDevices();
            }
        }

        /// <summary>
        /// Activates a player character for each player and deactives the rest.
        /// </summary>
        /// <param name="newPlayerCount">The player count</param>
        public void ActivatePlayers(int newPlayerCount)
        {
            PlayerCount = (newPlayerCount < MaxPlayers ? newPlayerCount : MaxPlayers);
            //Debug.Log("New player count: " + PlayerCount);

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

        #endregion Player Setup

        public UIController GetUI()
        {
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

        #region Player Methods

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
        /// Can return only a living player if necessary.
        /// Returns null if there is no valid player.
        /// </summary>
        /// <param name="includeDead">Are dead players included</param>
        /// <returns>A player character or null</returns>
        public PlayerCharacter GetAnyPlayer(bool includeDead)
        {
            return GetValidPlayer(p =>
                !p.IsDead || includeDead);
        }

        /// <summary>
        /// Returns the first player character which isn't the one given.
        /// Can return only a living player if necessary.
        /// Returns null if there is no valid player.
        /// </summary>
        /// <param name="invalidPlayer">A player that won't be returned</param>
        /// <param name="includeDead">Are dead players included</param>
        /// <returns>A player character or null</returns>
        public PlayerCharacter GetAnyOtherPlayer(PlayerCharacter invalidPlayer,
                                                 bool includeDead)
        {
            return GetValidPlayer(p =>
                p != invalidPlayer && (!p.IsDead || includeDead));
        }

        /// <summary>
        /// Legacy code; please remove.
        /// </summary>
        /// <param name="invalidPlayer"></param>
        /// <param name="includeDead"></param>
        /// <returns></returns>
        public PlayerCharacter GetPlayerWithTool(PlayerTool tool,
                                                 bool includeDead)
        {
            return GetValidPlayer(p =>
                p.Tool == tool && (!p.IsDead || includeDead));
        }

        /// <summary>
        /// Returns the first player character
        /// which fulfills all requirements.
        /// If none does, returns null.
        /// </summary>
        /// <param name="requirements">
        /// The requirements for the player character
        /// (tip: use a lambda expression)
        /// </param>
        /// <returns>A player character or null</returns>
        public PlayerCharacter GetValidPlayer(Predicate<PlayerCharacter> requirements)
        {
            for (int i = 0; i < PlayerCount; i++)
            {
                if (requirements(_players[i]))
                {
                    return _players[i];
                }
            }

            return null;
            //return Array.Find(_players, requirements);
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

        public PlayerInput GetPlayerInput(int playerID)
        {
            PlayerInput result = null;
            InputDevice device = InputDevice.Keyboard;

            if (playerID > 0)
            {
                int firstUnusedInputDeviceNum = GetFirstUnusedInputDeviceNum(_inputDevices);
                if (firstUnusedInputDeviceNum >= 0)
                {
                    device = (InputDevice) firstUnusedInputDeviceNum;
                }
            }

            if (playerID == 0)
            {
                result = MenuPlayerInput;
            }
            else
            {
                result = new PlayerInput(device);
            }

            _inputDevices[playerID] = result.InputDevice;
            return result;
        }

        public static int GetFirstUnusedInputDeviceNum(InputDevice[] usedInputDevices)
        {
            int inputDeviceIndex = 0;

            for (int i = 0; i < usedInputDevices.Length; i++)
            {
                if ((int) usedInputDevices[i] == inputDeviceIndex)
                {
                    inputDeviceIndex++;
                }
            }

            // NOTE: The next index points to "None".
            if (inputDeviceIndex > (int) InputDevice.Gamepad3)
            {
                Debug.LogError("All input devices are in use.");
                return -1;
            }
            else
            {
                return inputDeviceIndex;
            }
        }

        /// <summary>
        /// Saves which player is using which input device.
        /// </summary>
        public void SaveInputDevices()
        {
            for (int i = 0; i < _players.Length; i++)
            {
                _inputDevices[i] = _players[i].Input.InputDevice;
                //Debug.Log(string.Format("Input device saved: {0} has {1}",
                //    _players[i].name, _players[i].Input.InputDevice));
            }
        }

        #endregion Player Methods

        #region Scene Management

        private void StartFadeOut(int nextTrack = -1)
        {
            _fade.StartFadeOut(false);
            if (nextTrack >= 0)
            {
                MusicPlayer.Instance.StartFadeOut(nextTrack);
            }
            else
            {
                MusicPlayer.Instance.StartFadeOut();
            }
        }

        /// <summary>
        /// Starts reseting level with a fade-out.
        /// </summary>
        public void StartSceneReset()
        {
            SceneTransition = TransitionPhase.ResetingScene;
            StartFadeOut();
            Debug.Log("[GameManager] Resetting level");
        }

        /// <summary>
        /// Resets the current level.
        /// </summary>
        public void ResetLevel()
        {
            SetScore(0);
            _ui.ActivateLevelEndScreen(false);
            World.Instance.ResetWorld();
            _levelManager.ResetLevel();
            _input.ResetInput();
            _players.ForEach(pc => pc.CancelActions());
            _players.ForEach
                (pc => pc.RespawnPosition =_levelManager.GetSpawnPoint(pc.ID));
            ForEachActivePlayerChar(pc => pc.Respawn());
            _npcs.ForEach(npc => npc.Respawn());
            _levelObjects.ForEach(obj => obj.ResetObject());
            _ui.ResetUI();
            DeadPlayerCount = 0;
            _fade.StartFadeIn(true);

            if (GameState == State.Play || GameState == State.LevelEnd)
            {
                StartLevel();
            }
        }

        /// <summary>
        /// Loads the main menu scene.
        /// </summary>
        public void LoadMainMenu()
        {
            StartLoadingScene(MainMenuKey);
            GameState = State.MainMenu;

            _levelManager.StopTickingSound();

            StartFadeOut(0);
        }

        public void LoadLevel(int levelNum)
        {
            if (!SceneChanging)
            {
                if (LevelExists(levelNum))
                {
                    CurrentLevel = _levels[levelNum];
                    StartFadeOut(1);
                    LoadChangedCurrentLevel();
                }
                else if (levelNum == 0)
                {
                    CurrentLevel = _levels[0];
                    StartFadeOut(1);
                    LoadDebugLevel();
                }
                else
                {
                    Debug.LogError(string.Format("Invalid level number: {0}.", levelNum));
                }
            }
            else
            {
                Debug.LogWarning("Scene is already changing.");
            }
        }

        private void LoadChangedCurrentLevel()
        {
            Debug.Log(string.Format("[GameManager] Going to level {0}: {1}",
                    CurrentLevel.Number, CurrentLevel.LevelName));
            StartLoadingScene(CurrentLevel.LevelSceneName);
            GameState = State.Play;
        }

        public void GoToNextLevel()
        {
            int nextLevel = CurrentLevel.Number + 1;

            if (LevelExists(nextLevel))
            {
                _ui.ActivateLevelEndScreen(false);
                TryUnlockLevel(nextLevel);
                LoadLevel(nextLevel);
            }
        }

        private bool LevelExists(int levelNum)
        {
            return levelNum >= 1 && levelNum < _levels.Count;
        }

        /// <summary>
        /// Unlocks the given level if it exists
        /// and hasn't been unlocked yet.
        /// </summary>
        /// <param name="levelNum">A level number</param>
        private void TryUnlockLevel(int levelNum)
        {
            if (levelNum > LevelsUnlocked && LevelExists(levelNum))
            {
                LevelsUnlocked = levelNum;
                Debug.Log("[GameManager] Level " + levelNum + " unlocked");
            }
        }

        /// <summary>
        /// Testing.
        /// Loads a debug level: LinkProto.
        /// </summary>
        public void LoadDebugLevel()
        {
            LoadDebugLevel("LinkProto");
        }

        /// <summary>
        /// Testing. Loads a debug level.
        /// </summary>
        public void LoadDebugLevel(string sceneName)
        {
            CurrentLevel = _levels[0];
            StartLoadingScene(sceneName);
            GameState = State.Play;
        }

        /// <summary>
        /// Starts loading a scene with a fade-out.
        /// </summary>
        /// <param name="sceneName">The scene's name</param>
        public void StartLoadingScene(string sceneName)
        {
            if (SceneTransition != TransitionPhase.ExitingScene)
            {
                Debug.Log("[GameManager] Loading scene: " + sceneName);

                if (GameState == State.Play)
                {
                    ForEachActivePlayerChar(pc => pc.CancelActions());
                    World.Instance.ResetWorld();
                }

                SceneTransition = TransitionPhase.ExitingScene;
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
            SceneTransition = TransitionPhase.StartingScene;
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

        #endregion Scene Management

        #region Level Methods

        public void StartLevel()
        {
            Debug.Log("[GameManager] Level starts");
            GameState = State.Play;
            SetScore(0);
            _levelManager.StartLevel();
            ResetLevelResultAudioSource();

            int trackNum = 1; // TODO: Don't hardcode!
            if (!MusicPlayer.Instance.IsPlaying || MusicPlayer.Instance.currentTrack != trackNum)
            {
                MusicPlayer.Instance.Play(trackNum);
            }
        }

        public void EndLevel(bool levelWon)
        {
            GameState = State.LevelEnd;

            if (levelWon)
            {
                SaveGame();
                _ui.FlashLevelTimeBar(false);
            }

            Debug.Log("[GameManager] Level " + (levelWon ? "won" : "lost") +
                "! Score: " + CurrentScore + " / " + _levelManager.requiredScore);
            _ui.ActivateLevelEndScreen(true, levelWon);

            MusicPlayer.Instance.Stop();
            ResetLevelResultAudioSource();
            if (levelWon)
            {
                _levelResultAudioSrc = SFXPlayer.Instance.Play(Sound.Victory_Jingle);
            }
            else
            {
                _levelResultAudioSrc = SFXPlayer.Instance.Play(Sound.Loss_Jingle);
            }
        }

        public bool LevelWinConditionsMet()
        {
            return _levelManager != null && _levelManager.EnoughScore(CurrentScore);
        }

        private void ResetLevelResultAudioSource()
        {
            if (_levelResultAudioSrc != null && _levelResultAudioSrc.isPlaying)
            {
                _levelResultAudioSrc.Stop();
            }
        }

        public void CollectScorePickup(int score)
        {
            ChangeScore(score);
            _levelManager.PlayCollectSound();
            _levelManager.IncreaseMultiplier();
            _ui.SetMultiplierCounterValue(_levelManager.ScoreMultiplier);
        }

        public void ChangeScore(int score)
        {
            score = score * _levelManager.ScoreMultiplier;
            SetScore(CurrentScore + score);
        }

        public void SetScore(int score)
        {
            CurrentScore = score;
            _ui.SetScoreCounterValue(score);
        }

        public void ActivatePauseScreen(bool activate, string playerName)
        {
            if (activate || SceneTransition == TransitionPhase.None)
            {
                _ui.ActivatePauseScreen(activate, playerName);
            }
        }

        public void ActivateTargetIcon(bool activate, int playerID, Interactable interactable)
        {
            Vector3 position = (activate ? interactable.transform.position : Vector3.zero);
            if (!activate || interactable.ShowTargetIcon)
            {
                _ui.ActivateTargetIcon(activate, playerID, position);
            }
        }

        public void AddLevelObjectsToArray(LevelObject[] levelObjectsToAdd)
        {
            LevelObject[] newLevelObjects = new LevelObject[_levelObjects.Length + levelObjectsToAdd.Length];

            for (int i = 0; i < _levelObjects.Length; i++)
            {
                newLevelObjects[i] = _levelObjects[i];
            }

            for (int i = 0; i < levelObjectsToAdd.Length; i++)
            {
                newLevelObjects[_levelObjects.Length + i] = levelObjectsToAdd[i];
            }

            _levelObjects = newLevelObjects;
        }

        #endregion Level Methods

        #region Persistence

        /// <summary>
        /// The file path where the game is saved. On Windows, points to
        /// %userprofile%\AppData\LocalLow\<companyname>\<productname>\<savefile>.
        /// </summary>
        public string SavePath
        {
            get
            {
                return Path.Combine(Application.persistentDataPath, "save_data");
            }
        }

        /// <summary>
        /// Gets data from the game and stores it to a data object.
        /// </summary>
        public void SaveGame()
        {
            GameData data = new GameData();

            data.LevelsUnlocked = LevelsUnlocked;
            data.LevelNum = CurrentLevel.Number;
            data.Score = CurrentScore; // TODO: Highscores for levels

            _saveSystem.Save(data);
            Debug.Log(string.Format("[GameManager] Game saved (level {0})",
                CurrentLevel.Number));
        }

        /// <summary>
        /// Loads saved data.
        /// </summary>
        /// <returns>Loaded game data</returns>
        public GameData LoadGame()
        {
            GameData data = _saveSystem.Load();

            if (data == null)
            {
                Debug.LogWarning("Save data not loaded.");
                return null;
            }

            LevelsUnlocked = data.LevelsUnlocked;
            SetScore(data.Score); // TODO: Highscores for levels

            bool inLevel = false;
            if (LevelExists(data.LevelNum))
            {
                inLevel = true;
                CurrentLevel =
                    _levels.Find(level => level.Number == data.LevelNum);
            }

            if (inLevel)
            {
                Debug.Log(string.Format("Game loaded (level {0}: {1})",
                    CurrentLevel.Number, CurrentLevel.LevelName));

                //LoadLevel(CurrentLevel.Number);
            }
            else
            {
                Debug.Log("Game loaded (no level in progress)");
            }
            
            return data;
        }

        public void ResetSaveData()
        {
            LevelsUnlocked = 1;
            CurrentLevel = _levels[0];
            CurrentScore = 0;
            SaveGame();
        }

        #endregion Persistence

        private void OnDisable()
        {
            SceneManager.activeSceneChanged -= InitScene;
        }
    }
}
