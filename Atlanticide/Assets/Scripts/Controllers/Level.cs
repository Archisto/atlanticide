using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class Level : MonoBehaviour
    {
        [SerializeField]
        private Transform _player1SpawnPoint;

        [SerializeField]
        private Transform _player2SpawnPoint;

        [SerializeField]
        private Transform _levelCenter;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            if (_player1SpawnPoint == null)
            {
                Debug.LogError(Utils.GetFieldNullString("Player 1 spawn point"));
            }
            if (_player2SpawnPoint == null)
            {
                Debug.LogError(Utils.GetFieldNullString("Player 2 spawn point"));
            }
            if (_levelCenter == null)
            {
                Debug.LogError(Utils.GetFieldNullString("Level center"));
            }
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
        }

        public Vector3 GetSpawnPoint(int playerNum)
        {
            switch (playerNum)
            {
                case 0:
                {
                    return _player1SpawnPoint.position;
                }
                case 1:
                {
                    return _player2SpawnPoint.position;
                }
                default:
                {
                    return Vector3.zero;
                }
            }
        }

        public Vector3 GetLevelCenter()
        {
            return _levelCenter.position;
        }

        public void ResetLevel()
        {
        }
    }
}
