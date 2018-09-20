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

        public const int MaxPlayers = 4;

        private PlayerCharacter _playerPrefab;
        private PlayerCharacter[] _players;
        private UIController _UI;

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

            InitPlayers();
            InitUI();
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
                _players[i].name = "Player " + (i + 1);
                _players[i].Input = new PlayerInput(i);
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
                Debug.LogError("A UIController object could not be found in the scene.");
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
    }
}
