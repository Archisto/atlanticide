using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField]
        private Transform _energyCollPlayerSpawnPoint;

        [SerializeField]
        private Transform _shieldPlayerSpawnPoint;

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
