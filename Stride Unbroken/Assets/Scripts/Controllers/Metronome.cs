using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrideUnbroken
{
    public class Metronome : MonoBehaviour
    {
        #region Statics
        private static Metronome instance;
        public static Metronome Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<Metronome>();

                    if (instance == null)
                    {
                        Debug.LogError("A Metronome object could not be found in the scene.");
                    }
                }

                return instance;
            }
        }
        #endregion Statics

        [SerializeField, Range(0.1f, 2f)]
        private float _tempo;

        private float _tickRatio;
        private int _currentTick;
        private float _elapsedTime;
        private bool _paused;

        public delegate void TickAction();
        public event TickAction OnTick;

        /// <summary>
        /// The tempo.
        /// </summary>
        public static float Tempo
        {
            get { return Instance._tempo; }
        }

        /// <summary>
        /// The tick ratio.
        /// </summary>
        public static float TickRatio
        {
            get { return Instance._tickRatio; }
        }

        /// <summary>
        /// The current tick.
        /// </summary>
        public static int CurrentTick
        {
            get { return Instance._currentTick; }
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        void Awake()
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

            StartTick();
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (!_paused)
            {
                UpdateTick();
            }
        }

        /// <summary>
        /// Updates the current tick.
        /// </summary>
        private void UpdateTick()
        {
            _tickRatio = _elapsedTime / _tempo;
            _elapsedTime += Time.deltaTime;

            if (_tickRatio >= 1f)
            {
                _tickRatio = 1f;
                EndTick();
            }
        }

        private void StartTick()
        {
            _currentTick++;
            _elapsedTime = 0;
        }

        private void EndTick()
        {
            if (OnTick != null)
            {
                OnTick();
            }

            GameManager.Instance.TickEnded();

            StartTick();
        }

        public static void Pause(bool enabled)
        {
            instance._paused = enabled;
        }
    }
}
