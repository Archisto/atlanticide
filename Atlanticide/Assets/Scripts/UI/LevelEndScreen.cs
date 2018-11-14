using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Atlanticide.UI
{
    public class LevelEndScreen : MonoBehaviour
    {
        private const string LosingText = "Time's Up";

        [SerializeField]
        private Text title;

        [SerializeField]
        private Text _scoreText;

        [SerializeField]
        private Text _requiredScoreText;

        private Level _currentLevel;

        public void Activate(bool activate, bool levelWon = false)
        {
            gameObject.SetActive(activate);

            if (activate)
            {
                World.Instance.PauseGame(true);
                _currentLevel = GameManager.Instance.CurrentLevel;
                _scoreText.text = GameManager.Instance.CurrentScore.ToString();
                _requiredScoreText.text = GameManager.Instance.RequiredScore.ToString();

                if (levelWon)
                {
                    title.text = string.Format("Level {0} Completed!", _currentLevel.Number);
                }
                else
                {
                    title.text = LosingText;
                }
            }
        }

        public void NextLevel()
        {
            GameManager.Instance.GoToNextLevel();
        }

        private void ResumeGame()
        {
            World.Instance.PauseGame(false);
        }

        public void RestartLevel()
        {
            GameManager.Instance.StartSceneReset();
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
