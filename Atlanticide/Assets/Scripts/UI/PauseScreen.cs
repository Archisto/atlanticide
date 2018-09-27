﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Atlanticide.UI
{
    public class PauseScreen : MonoBehaviour
    {
        [SerializeField]
        public Text title;

        [SerializeField]
        public Text pausingPlayerText;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            title.text = "Game Paused";
        }

        public void ResumeGame()
        {
            World.Instance.PauseGame(false);
        }

        public void RestartLevel()
        {
            GameManager.Instance.ResetLevel();
            ResumeGame();
        }

        public void ReturnToMainMenu()
        {
            GameManager.Instance.LoadMainMenu();
            ResumeGame();
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}