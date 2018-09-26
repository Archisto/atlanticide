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

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            title.text = "Game paused";
        }
    }
}
