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
        private PlayerCharacter[] _players = new PlayerCharacter[4];
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
            _playerPrefab = Resources.Load<PlayerCharacter>("PlayerCharacter");
            CreatePlayers(2);
        }

        /// <summary>
        /// Creates player characters.
        /// </summary>
        /// <param name="playerCount">The player count</param>
        public void CreatePlayers(int playerCount)
        {
            for (int i = 0; i < MaxPlayers; i++)
            {
                if (i < playerCount)
                {
                    _players[i] = Instantiate(_playerPrefab);
                    _players[i].CharacterName = "Player " + (i + 1);
                    _players[i].Input = new PlayerInput(i);
                }
                else if (_players[i] != null)
                {
                    Destroy(_players[i].gameObject);
                }
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
