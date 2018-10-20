using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Atlanticide.UI;

namespace Atlanticide
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField]
        private Transform _energyCollPlayerSpawnPoint;

        [SerializeField]
        private Transform _shieldPlayerSpawnPoint;

        private UIController _ui;
        private Level _currentLevel;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            if (_energyCollPlayerSpawnPoint == null)
            {
                Debug.LogError(Utils.GetFieldNullString("Player 1 spawn point"));
            }
            if (_shieldPlayerSpawnPoint == null)
            {
                Debug.LogError(Utils.GetFieldNullString("Player 2 spawn point"));
            }

            _ui = GameManager.Instance.GetUI();
            _currentLevel = GameManager.Instance.CurrentLevel;
            string puzzleName = _currentLevel.GetCurrentPuzzleName();
            _ui.levelName.text = _currentLevel.LevelSceneName + (puzzleName != null ?
                " - " + _currentLevel.GetCurrentPuzzleName() : "");
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
        }

        public Vector3 GetSpawnPoint(PlayerTool tool)
        {
            switch (tool)
            {
                case PlayerTool.EnergyCollector:
                {
                    return _energyCollPlayerSpawnPoint.position;
                }
                case PlayerTool.Shield:
                {
                    return _shieldPlayerSpawnPoint.position;
                }
                default:
                {
                    return Vector3.zero;
                }
            }
        }

        public void ResetLevel()
        {
        }
    }
}
