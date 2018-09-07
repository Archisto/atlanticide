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
                        instance = Resources.Load<GameManager>("GameManager");
                    }
                }

                return instance;
            }
        }
        #endregion Statics

        private UIController _UI;
        private Metronome _metronome;

        public int CurrentScore { get; set; }

        public bool GameOver
        {
            get { return false; }
        }

        public float PlayerTickRatio { get; set; }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        void Awake()
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
            InitMetronome();
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

        /// <summary>
        /// Initializes the metronome.
        /// </summary>
        private void InitMetronome()
        {
            _metronome = FindObjectOfType<Metronome>();
            if (_metronome == null)
            {
                Debug.LogError("A Metronome object could not be found in the scene.");
            }
        }

        public void TickEnded()
        {
            _UI.UpdateUI();
        }
    }
}
