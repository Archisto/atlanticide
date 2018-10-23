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

            MainMenu();

            // If no save game is detected, the continue button should not be interactable.

            // if (SaveSlot != null)
            // continueButton.interactable = true;
            // else 
            continueButton.interactable = false;
        

            // If a save game exists, the delete save data button should be interactable.
            // Otherwise, it should not.

            // if (SaveSlot != null)
            // deleteSaveDataButton.interactable = true;
            // else
            deleteSaveDataButton.interactable = false;
        }

        public void StartNewGame()
        {
            GameManager.Instance.LoadLevelFromBeginning(levelNumber);

            // If a save file exists and the player presses the button, there
            // should be a confirmation asking whether the player truly wants to 
            // overwrite their save file (in the case that there is only one).
        }

        public void Continue()
        {
            // Takes the player to the puzzle they were last at according to the save file.
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
            // Removes the player's save file (if there is only one).
            // There should be a confirmation after pressing the button.
        }
    }
}
