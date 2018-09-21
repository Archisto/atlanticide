using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        private PlayerCharacter _playerPrefab;
        private PlayerCharacter[] _players;
        private NonPlayerCharacter[] _npcs;
        private UIController _UI;
        private Transform[] _telegrabs;

        public int CurrentScore { get; set; }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);

            InitUI();
            InitPlayers();
            InitNPCs();
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

        public bool GameOver
        {
            get { return false; }
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
    }
}
