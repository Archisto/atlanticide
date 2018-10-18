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

        private Action InputDeviceSwap;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            title.text = "Game Paused";
        }

        public void SetInputDeviceSwapAction(Action inputDeviceSwap)
        {
            InputDeviceSwap = inputDeviceSwap;
        }

        public void ResumeGame()
        {
            World.Instance.PauseGame(false);
        }

        public void SwapInputDevices()
        {
            if (InputDeviceSwap != null)
            {
                InputDeviceSwap.Invoke();
            }
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

        private void OnDestroy()
        {
            InputDeviceSwap = null;
        }
    }
}
