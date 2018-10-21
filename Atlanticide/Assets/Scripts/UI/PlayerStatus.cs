using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Atlanticide.UI
{
    public class PlayerStatus : MonoBehaviour
    {
        [SerializeField]
        private Image _toolImage;

        [SerializeField]
        private Text _playerName;

        public void SetToolImage(Sprite sprite)
        {
            _toolImage.sprite = sprite;
        }

        public void SetPlayerName(string playerName)
        {
            _playerName.text = playerName;
        }
    }
}
