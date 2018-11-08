using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Atlanticide.UI;

namespace Atlanticide
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField]
        private Transform _player1SpawnPoint;

        [SerializeField]
        private Transform _player2SpawnPoint;

        private UIController _ui;
        private Level _currentLevel;

        // Orichalcum pickup prefab and pool.
        public OrichalcumPickup orichalcumPickupPrefab;
        public Pool<OrichalcumPickup> orichalcumPickupPool;

        // Destructible GameObject debris prefabs and pools.
        public Debris[] stoneDebrisPrefabArray;
        public Pool<Debris> stoneDebrisPool;
        public Debris[] woodDebrisPrefabArray;
        public Pool<Debris> woodDebrisPool;
        public Debris[] terracottaDebrisPrefabArray;
        public Pool<Debris> terracottaDebrisPool;

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

            _ui = GameManager.Instance.GetUI();
            _currentLevel = GameManager.Instance.CurrentLevel;
            string puzzleName = _currentLevel.GetCurrentPuzzleName();
            _ui.levelName.text = _currentLevel.LevelSceneName + (puzzleName != null ?
                " - " + _currentLevel.GetCurrentPuzzleName() : "");
            orichalcumPickupPool = new Pool<OrichalcumPickup>(64, true, orichalcumPickupPrefab);
            stoneDebrisPool = new Pool<Debris>(32, true, stoneDebrisPrefabArray);
            woodDebrisPool = new Pool<Debris>(32, true, woodDebrisPrefabArray);
            terracottaDebrisPool = new Pool<Debris>(32, true, terracottaDebrisPrefabArray);
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

            //switch (tool)
            //{
            //    case PlayerTool.EnergyCollector:
            //    {
            //        return _energyCollPlayerSpawnPoint.position;
            //    }
            //    case PlayerTool.Shield:
            //    {
            //        return _shieldPlayerSpawnPoint.position;
            //    }
            //    default:
            //    {
            //        return Vector3.zero;
            //    }
            //}
        }

        public void ResetLevel()
        {
        }
    }
}
