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
}
