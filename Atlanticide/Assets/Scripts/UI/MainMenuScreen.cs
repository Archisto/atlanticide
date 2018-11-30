using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Atlanticide.UI
{
    public class MainMenuScreen : MonoBehaviour
    {
        [SerializeField]
        public Text title;

        [SerializeField]
        public Button continueButton;

        [SerializeField]
        public Button deleteSaveDataButton;

        [SerializeField]
        private int levelNumber;

        private EventSystem es;

        private StandaloneInputModule sim;

        public GameObject mainMenuScreen;
        public GameObject settingsScreen;

        private PlayerInput playerInput;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            title.text = "Main Menu";

            EventSystem es = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            es.SetSelectedGameObject(null);
            es.SetSelectedGameObject(es.firstSelectedGameObject);

            sim = es.gameObject.GetComponent<StandaloneInputModule>();

            playerInput = new PlayerInput(InputDevice.Keyboard);
            //playerInput = new PlayerInput(InputDevice.Gamepad1);

            MainMenu();

            CheckSave();
        }

        public void StartNewGame()
        {
            GameManager.Instance.LoadLevel(levelNumber);

            // If a save file exists and the player presses the button, there
            // should be a confirmation asking whether the player truly wants to 
            // overwrite their save file (in the case that there is only one).
        }

        public void Continue()
        {
            GameManager.Instance.LoadLevel(GameManager.Instance.CurrentLevel.Number);
        }

        public void MainMenu()
        {
            mainMenuScreen.SetActive(true);
            settingsScreen.SetActive(false);

            sim.horizontalAxis = playerInput.HorizontalMenuKey;
            sim.verticalAxis = playerInput.VerticalMenuKey;
        }

        public void SettingsMenu()
        {
            mainMenuScreen.SetActive(false);
            settingsScreen.SetActive(true);
        }

        public void QuitGame()
        {
            Application.Quit();
            Debug.Log("You quit the game, dude!");
        }

        public void DeleteSaveData()
        {
            // Resets the save file. There should be a confirmation after pressing the button.
            GameManager.Instance.ResetSaveData();

            CheckSave();
        }

        public void CheckSave()
        {
            GameManager.Instance.LoadGame();

            if (GameManager.Instance.CurrentLevel.Number >= 1)
            {
                continueButton.interactable = true;
                deleteSaveDataButton.interactable = true;
            }
            else
            {
                continueButton.interactable = false;
                deleteSaveDataButton.interactable = false;
            }
        }
    }
}
