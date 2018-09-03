using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrideUnbroken
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
                        Debug.LogError("A GameManager object could not be found in the scene.");
                    }
                }

                return instance;
            }
        }
        #endregion Statics

        [SerializeField, Range(0.1f, 2f)]
        private float _tempo;

        private GameUI _UI;

        public float Tempo
        {
            get { return _tempo; }
        }

        public bool GameOver
        {
            get { return false; }
        }

        /// <summary>
        /// Initializes the game.
        /// </summary>
        void Awake()
        {
            _UI = FindObjectOfType<GameUI>();
            if (_UI == null)
            {
                Debug.LogError("GameUI object could not be found in the scene.");
            }
        }
    }
}
