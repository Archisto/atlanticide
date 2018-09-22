using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Atlanticide
{
    public class FadeToColor : MonoBehaviour
    {
        [SerializeField]
        private Color _color;

        [SerializeField]
        private float _fadeOutTime = 1;

        [SerializeField]
        private float _fadeInTime = 1;

        public Image _uiImage;

        private bool _fadeOut;
        private float _fadeProgress;
        private float _elapsedTime;

        /// <summary>
        /// Is the same fade object still used
        /// even after the scene changes.
        /// </summary>
        public bool UsedInAllScenes { get; private set; }

        public bool Active { get; private set; }

        public bool FadedOut
        {
            get
            {
                return (_fadeOut && _fadeProgress == 1);
            }
        }

        public bool FadedIn
        {
            get
            {
                return (!_fadeOut && _fadeProgress == 1);
            }
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Awake()
        {
            InitUsageInAllScenes();
            CheckForErrors();

            // TODO: Disabled temporarily because this should not be used in the first scene when the game is started.
            // 
            //if (!UsedInAllScenes)
            //{
            //    InitAfterSceneChange();
            //}
        }

        public void Init(Image image)
        {
            _uiImage = image;
        }

        /// <summary>
        /// Checks if there's a FadeCatcher object in the original scene.
        /// If not, the fade will be transferred between scenes.
        /// </summary>
        private void InitUsageInAllScenes()
        {
            //FadeCatcher catcher = FindObjectOfType<FadeCatcher>();
            UsedInAllScenes = false;
            //if (catcher == null)
            //{
            //    UsedInAllScenes = true;
            //    DontDestroyOnLoad(gameObject);
            //}
            //else
            //{
            //    UsedInAllScenes = false;
            //}
        }

        /// <summary>
        /// After the scene has changed, initializes the fade.
        /// Starts fading in, continuing the fade-out before the scene change.
        /// </summary>
        public void InitAfterSceneChange()
        {
            StartFadeIn();
            //SetCompatibleSwitches();
        }

        //private void SetCompatibleSwitches()
        //{
        //    FadeActivator activator = GetComponent<FadeActivator>();

        //    if (activator != null)
        //    {
        //        activator.FindCompatibleSwitches();
        //    }
        //    //else
        //    //{
        //    //    Debug.LogError("FadeActivator component could " +
        //    //                   "not be found in the object.");
        //    //}
        //}

        /// <summary>
        /// Checks for any missing references and logs the errors.
        /// </summary>
        private void CheckForErrors()
        {
            if (UsedInAllScenes)
            {
                if (_uiImage == null)
                {
                    Debug.LogError("UI Image could not be " +
                                   "found in the scene.");
                }
            }
        }

        /// <summary>
        /// Starts fading in or out, opposite of whichever was done last.
        /// </summary>
        public void StartNextFade()
        {
            _fadeOut = !_fadeOut;

            if (_fadeOut)
            {
                StartFadeOut();
            }
            else
            {
                StartFadeIn();
            }
        }

        /// <summary>
        /// Starts fading out.
        /// </summary>
        public void StartFadeOut()
        {
            _fadeOut = true;
            StartFade();
        }

        /// <summary>
        /// Starts fading in.
        /// </summary>
        public void StartFadeIn()
        {
            _fadeOut = false;
            StartFade();
        }

        /// <summary>
        /// Starts the fading process.
        /// </summary>
        private void StartFade()
        {
            _fadeProgress = 0;
            _elapsedTime = 0;
            Active = true;
        }

        /// <summary>
        /// Finishes the fading process.
        /// </summary>
        private void FinishFade()
        {
            _fadeProgress = 1;
            Active = false;
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (UsedInAllScenes)
            {
                InitAfterSceneChange();
            }

            if (Active)
            {
                // Increases the elapsed time
                _elapsedTime += Time.deltaTime;

                // Updates the fade's progress
                UpdateFadeProgress();

                // Updates the fade object's transparency
                UpdateTransparency();
            }
        }

        /// <summary>
        /// Updates the fade's progress.
        /// </summary>
        private void UpdateFadeProgress()
        {
            if (_fadeOut)
            {
                if (_fadeOutTime <= 0)
                {
                    _fadeProgress = 1;
                }
                else
                {
                    _fadeProgress = _elapsedTime / _fadeOutTime;
                }
            }
            else
            {
                if (_fadeInTime <= 0)
                {
                    _fadeProgress = 1;
                }
                else
                {
                    _fadeProgress = _elapsedTime / _fadeInTime;
                }
            }

            if (_fadeProgress >= 1f)
            {
                FinishFade();
            }
        }

        /// <summary>
        /// Updates the UI image's transparency.
        /// </summary>
        private void UpdateTransparency()
        {
            if (_uiImage != null)
            {
                Color newColor = _color;

                if (_fadeOut)
                {
                    newColor.a = _fadeProgress;
                }
                else
                {
                    newColor.a = 1f - _fadeProgress;
                }

                _uiImage.color = newColor;
            }
        }
    }
}
