using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class World : MonoBehaviour
    {
        #region Statics
        private static World instance;
        public static World Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<World>();

                    if (instance == null)
                    {
                        Debug.LogError("A World object could not be found in the scene.");
                    }
                }

                return instance;
            }
        }
        #endregion Statics

        public float gravity = 1;

        [SerializeField, Range(0.1f, 5f)]
        public float telegrabRadius = 1;

        /// <summary>
        /// Initializes the singleton instance.
        /// </summary>
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
        }
    }
}
