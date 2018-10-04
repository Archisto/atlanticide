using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class SettingsManager
    {
        private const string SFXVolumeKey = "SFXVolume";
        private const string MusicVolumeKey = "MusicVolume";
        private const string MuteAudioKey = "MuteAudio";

        public float SFXVolume { get; set; }
        public float MusicVolume { get; set; }
        public bool MuteAudio { get; set; }

        /// <summary>
        /// Creates the settings manager.
        /// </summary>
        public SettingsManager()
        {
            // TODO: Unsaved settings should be reset to the saved ones
            // OR, settings should be saved after each setting change.

            LoadSettings();
        }

        /// <summary>
        /// Saves settings.
        /// </summary>
        public void SaveSettings()
        {
            PlayerPrefs.SetFloat(SFXVolume.ToString(), SFXVolume);
            PlayerPrefs.SetFloat(MusicVolume.ToString(), MusicVolume);
            Utils.SetBool(MuteAudio.ToString(), MuteAudio);
        }

        public void LoadSettings()
        {
            SFXVolume = PlayerPrefs.GetFloat(SFXVolumeKey, 0.5f);
            MusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 0.5f);
            MuteAudio = Utils.GetBool(MuteAudioKey, false);
        }
    }

    /*
    public class Settings : MonoBehaviour
    {
        #region Statics
        private static Settings instance;

        public static Settings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<Settings>();
                    if (instance == null)
                    {
                        // Prints an error message.
                        // A Settings object must be in the scene.
                        Debug.LogError("A Settings object has not " +
                                       "been added to the scene.");
                    }
                }

                return instance;
            }
        }
        #endregion Statics

        private const string MusicVolumeKey = "musicVolume";
        private const string EffectVolumeKey = "effectVolume";

        [SerializeField]
        private float _defaultMusicVolume = 0.1f;

        [SerializeField]
        private float _defaultEffectVolume = 0.1f;

        [SerializeField]
        private bool _autoplayMusic = true;

        //[SerializeField]
        //private Slider _musicVolumeSlider;

        //[SerializeField]
        //private Slider _effectVolumeSlider;

        //[SerializeField]
        //private Toggle _enableEventCamToggle;

        //[SerializeField]
        private bool _enableEventCamera = true;

        private bool _uiObjectsInitialized = false;
        private bool _audioInitialized = false;

        private float _musicVolume;
        private float _effectVolume;

        //private InputManager _input;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            //_input = FindObjectOfType<InputManager>();
            InitAudio();
            InitUIObjects();
            //EnableEventCamera = _enableEventCamera;
        }

        private void InitAudio()
        {
            if (!_autoplayMusic)
            {
                MusicPlayer.Instance.Stop();
            }

            _audioInitialized = true;
        }

        private void InitUIObjects()
        {
            //_musicVolumeSlider.value = MusicVolume;
            //_effectVolumeSlider.value = EffectVolume;
            _uiObjectsInitialized = true;
        }

        public float MusicVolume
        {
            get
            {
                return _musicVolume;
            }
            set
            {
                _musicVolume = Mathf.Clamp01(value);

                if (_audioInitialized)
                {
                    MusicPlayer.Instance.SetVolume(value);
                }
            }
        }

        public float EffectVolume
        {
            get
            {
                return _effectVolume;
            }
            set
            {
                _effectVolume = Mathf.Clamp01(value);

                if (_audioInitialized)
                {
                    //SFXPlayer.Instance.SetVolume(value);
                }
            }
        }

        public void OnMusicVolumeValueChanged()
        {
            if (_uiObjectsInitialized)
            {
                //MusicVolume = _musicVolumeSlider.value;
            }
        }

        public void OnEffectVolumeValueChanged()
        {
            if (_uiObjectsInitialized)
            {
                //EffectVolume = _effectVolumeSlider.value;
            }
        }

        public void EraseGameProgress(bool skipConfirmation)
        {
            //if (!skipConfirmation)
            //{
            //    _input.Confirm(ConfirmationType.EraseHighscores);
            //}
            //else
            //{
            //    GameManager.Instance.EraseLocalHighscores();

            //    // Highlight the default menu button if the mouse is not used
            //    if (!_input.HighlightMenuDefaultButton())
            //    {
            //        // Clears the menu button selection if the mouse is used
            //        EventSystem.current.SetSelectedGameObject(null);
            //    }
            //}
        }

        public void Save()
        {
            PlayerPrefs.SetFloat(MusicVolumeKey, _musicVolume);
            PlayerPrefs.SetFloat(EffectVolumeKey, _effectVolume);
        }

        public void Load()
        {
            MusicVolume = PlayerPrefs.GetFloat
                (MusicVolumeKey, _defaultMusicVolume);
            EffectVolume = PlayerPrefs.GetFloat
                (EffectVolumeKey, _defaultEffectVolume);
        }
    }*/
}
