using System;
using System.Collections;
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

        private InputController _input;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            title.text = "Game Paused";
        }

        public void SetInput(InputController input)
        {
            _input = input;
        }

        public void ResumeGame()
        {
            World.Instance.PauseGame(false);
        }

        public void SwapInputDevices()
        {
            _input.SwapInputDevices();
        }

        public void RestartPuzzle()
        {
            GameManager.Instance.StartSceneReset();
            ResumeGame();
        }

        public void RestartLevel()
        {
            GameManager.Instance.LoadPuzzle(1);
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
