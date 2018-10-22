using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{

    public class EnemyTactics : MonoBehaviour
    {

        #region Statics

        private static EnemyTactics instance = null;
        private static readonly object padlock = new object();

        public static EnemyTactics Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new EnemyTactics();
                    }
                    return instance;
                }
            }
        }

        #endregion

        /// <summary>
        /// List of enemies that are taking part of the tactics currently
        /// </summary>
        private List<EnemyBase> Enemies;

        // Use this for initialization
        void Start()
        {
            Enemies = new List<EnemyBase>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Draws tactics
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;

        }
    }
}