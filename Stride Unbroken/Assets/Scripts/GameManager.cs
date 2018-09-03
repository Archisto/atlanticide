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

        private GameUI UI;

        public bool GameOver
        {
            get { return false; }
        }

        /// <summary>
        /// Initializes the game.
        /// </summary>
        void Awake()
        {
            UI = FindObjectOfType<GameUI>();
            if (UI == null)
            {
                Debug.LogError("GameUI object could not be found in the scene.");
            }
        }
    }
}
