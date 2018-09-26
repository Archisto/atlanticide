using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Atlanticide.UI
{
    public class MainMenuScreen : MonoBehaviour
    {
        [SerializeField]
        public Text title;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            title.text = "Main Menu";
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
