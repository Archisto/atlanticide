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
            _input = FindObjectOfType<InputController>();
            title.text = "Game paused";
        }

        public void ResumeGame()
        {
            _input.TogglePause(-1);
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
