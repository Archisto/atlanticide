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
        private Color _altColor;

        [SerializeField]
        private float _fadeOutTime = 1;

        [SerializeField]
        private float _fadeInTime = 1;

        private Image _screenCoverImage;
        private bool _fadeOut;
        private bool _useAltColor;
        private float _fadeProgress;
        private float _elapsedTime;

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
        private void Start()
        {
            CheckForErrors();
        }

        public void Init(Image image)
        {
            _screenCoverImage = image;
        }

        /// <summary>
        /// Checks for any missing references and logs the errors.
        /// </summary>
        private void CheckForErrors()
        {
            if (_screenCoverImage == null)
            {
                Debug.LogError(Utils.GetFieldNullString("Screen cover image"));
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
                StartFadeOut(false);
            }
            else
            {
                StartFadeIn();
            }
        }

        /// <summary>
        /// Starts fading out.
        /// </summary>
        public void StartFadeOut(bool useAltColor)
        {
            _fadeOut = true;
            _useAltColor = useAltColor;
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
            if (_screenCoverImage != null)
            {
                Color newColor = (_useAltColor ? _altColor : _color);

                if (_fadeOut)
                {
                    newColor.a = _fadeProgress;
                }
                else
                {
                    newColor.a = 1f - _fadeProgress;
                }

                _screenCoverImage.color = newColor;
            }
        }
    }
}
