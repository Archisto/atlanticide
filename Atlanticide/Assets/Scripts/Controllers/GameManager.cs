using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        private PlayerCharacter _playerPrefab;
        private PlayerCharacter[] _players;
        private NonPlayerCharacter[] _npcs;
        private UIController _UI;
        private Transform[] _telegrabs;
        private FadeToColor _fade;
        private bool _sceneChanged;
        private string _nextSceneName;

        public int CurrentLevel { get; set; }

        public int CurrentScore { get; set; }

        public bool SceneChanging { get; private set; }

        public bool FadeActive
        {
            get
            {
                return _fade.Active;
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

                CurrentLevel = 1;
            }
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (SceneChanging && _fade.FadedOut)
            {
                LoadScene(_nextSceneName);
            }
        }

        private void InitScene()
        {
            InitUI();
            InitPlayers();
            InitNPCs();
            InitFade();
            _sceneChanged = false;
        }

        /// <summary>
        /// Initializes the player characters.
        /// </summary>
        private void InitPlayers()
        {
            _players = new PlayerCharacter[MaxPlayers];
            _playerPrefab = Resources.Load<PlayerCharacter>("PlayerCharacter");
            CreatePlayers();
            ActivatePlayers(2);

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

                // Testing
                _players[i].transform.position = new Vector3(i * 2 - 4, 0.5f, 4);
            }
        }

        /// <summary>
        /// Activates a player character for each player and deactives the rest.
        /// </summary>
        /// <param name="playerCount">The player count</param>
        public void ActivatePlayers(int playerCount)
        {
            for (int i = 0; i < MaxPlayers; i++)
            {
                _players[i].gameObject.SetActive(i < playerCount);
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

        private void InitFade()
        {
            _fade = FindObjectOfType<FadeToColor>();
            _fade.Init(_UI.GetFade());

            if (_sceneChanged)
            {
                _fade.StartFadeIn();
            }
        }

        public bool GameOver
        {
            get { return false; }
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
        /// Resets the current level.
        /// </summary>
        public void ResetLevel()
        {
            foreach (PlayerCharacter pc in _players)
            {
                pc.Respawn();
            }

            foreach (NonPlayerCharacter npc in _npcs)
            {
                npc.Respawn();
            }
        }

        /// <summary>
        /// Loads the main menu scene.
        /// </summary>
        public void LoadMainMenu()
        {
            StartLoadingScene(MainMenuKey);
        }

        /// <summary>
        /// Loads a level.
        /// </summary>
        /// <param name="levelNum">The level number</param>
        public void LoadLevel(int levelNum)
        {
            if (!SceneChanging && levelNum >= 1 && levelNum <= LastLevel)
            {
                CurrentLevel = levelNum;
                StartLoadingScene(LevelKey + levelNum);
            }
            else
            {
                Debug.LogWarning(string.Format("Invalid level number ({0}).", levelNum));
            }
        }

        /// <summary>
        /// Starts loading a scene with a fade-out.
        /// </summary>
        /// <param name="sceneName">The scene's name</param>
        public void StartLoadingScene(string sceneName)
        {
            if (!SceneChanging)
            {
                Debug.Log("Loading scene: " + sceneName);

                Utils.ForEach(_players, p => p.CancelActions());

                SceneChanging = true;
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
            SceneChanging = false;
            _sceneChanged = true;
            SceneManager.LoadScene(sceneName);
        }

        private void InitScene(Scene prev, Scene next)
        {
            if (this != instance)
            {
                return;
            }

            //Debug.Log("Loaded: " + next.name);
            InitScene();
        }

        private void OnDisable()
        {
            SceneManager.activeSceneChanged -= InitScene;
        }
    }
}
