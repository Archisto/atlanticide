using System.Collections.Generic;
using UnityEngine;

// README:
// Create a new game object with only an AudioSource component.
// Leave AudioClip empty and set all bools (from Mute to Loop) to false.
// Make it a prefab and give it to this in the editor. For each sound clip,
// there must be a corresponding name in the Sound enum in the correct order.
// The volume can be controlled with this script but it's not necessary.
// In our game, another Singleton object called GameManager handles
// audio settings. You need to adjust this script a bit to fit your game.

namespace Atlanticide
{
    /// <summary>
    /// The sound effects' names
    /// </summary>
    public enum Sound
    {
        // NOTE:
        // Sound clips must be assigned to SFXPlayer
        // in this specific order for the right sound
        // to be played at the right time

        Footsteps_Tile_1 = 0,
        Footsteps_Tile_2 = 1,
        Footsteps_Tile_3 = 2,
        Footsteps_Tile_4 = 3,
        Jump_1 = 4,
        Jump_2 = 5,
        Trampoline_Jump = 6,
        Player_Hurt_1 = 7,
        Player_Hurt_2 = 8,
        Player_Landing_Sound_1 = 9,
        Player_Landing_Sound_2 = 10,
        Player_Landing_Grunt_1 = 11,
        Player_Landing_Grunt_2 = 12,
        Player_Landing_Grunt_3 = 13,
        Climbing_Slower = 14,
        Climbing_Faster = 15,
        Item_Pickup = 16,
        Key_Use = 17,
        Door_Open = 18,
        Door_Close = 19,
        Pressure_Plate = 20,
        Lever_Pull = 21,
        Drawbridge_Lowering = 22,
        Drawbridge_Wheel_Turning = 23,
        Shield_Bash = 24,
        Shield_Deflect_1 = 25,
        Shield_Deflect_2 = 26,
        Lifting_Shield = 27,
        Success = 28,
        Failure = 29,
        Spaceship_Passing = 30,
        Mining_Orichalcum = 31,
        People_Screaming = 32,
        Cyclops_Exploding = 33,
        Cyclops_Hovering = 34,
        Cyclops_Laser_Start = 35,
        Cyclops_Laser_Beam = 36,
        Cyclops_Shortcircuit = 37,
        Cyclops_Waking_Up = 38,
        Distant_Explosions_1 = 39,
        Distant_Explosions_2 = 40,
        Explosion = 41,
        Explosion_With_Rubble_1 = 42,
        Explosion_With_Rubble_2 = 43,
        Rubble = 44,
        Rubble_And_Debris = 45,
        Short_Explosion = 46,
        Orichalcum_Before_Exploding = 47,
        Setting_Trap = 48,
        Springing_Trap = 49,
        Elevator = 50,
        Taking_Energy = 51,
        Using_Energy = 52,
        Using_Energy_On_Target = 53,
        Heavy_Falling_To_Ground = 54,
        Revive = 55,
        Ground_Collapse = 56,
        Dissolve = 57,
        Projectile_1 = 58,
        Clink = 59
    }

    public class SFXPlayer : MonoBehaviour
    {
        #region Statics
        private static SFXPlayer instance;

        /// <summary>
        /// Gets or sets the Singleton instance .
        /// </summary>
        public static SFXPlayer Instance
        {
            get
            {
                if (instance == null)
                {
                    // NOTE:
                    // There must be a Resources folder under Assets and
                    // SFXPlayer there for this to work. Not necessary if
                    // a SoundPlayer object is present in a scene from the
                    // get-go.

                    instance =
                        Instantiate(Resources.Load<SFXPlayer>("SFXPlayer"));
                    instance.Init();
                }

                return instance;
            }
        }
        #endregion Statics

        /// <summary>
        /// The sound list
        /// </summary>
        [SerializeField,
            Tooltip("The sound list")]
        private List<AudioClip> sounds;

        /// <summary>
        /// The AudioSource prefab
        /// </summary>
        [SerializeField,
            Tooltip("The AudioSource prefab")]
        private GameObject audioSrcPrefab;

        /// <summary>
        /// The SFX volume
        /// </summary>
        [SerializeField, Range(0, 1),
            Tooltip("The SFX volume")]
        private float volume = 1;

        /// <summary>
        /// How many individual sounds can play at the same time
        /// </summary>
        [SerializeField,
            Tooltip("How many individual sounds can play at the same time")]
        private int audioSrcPoolSize = 5;

        /// <summary>
        /// Can new AudioSources be created if there are no unused left.
        /// </summary>
        [SerializeField, Tooltip("Can new AudioSources be created " +
            "if there are no unused left")]
        private bool flexiblePoolSize;

        /// <summary>
        /// The AudioSource pool
        /// </summary>
        private List<AudioSource> audioSrcPool;

        private float pitch; 

        /// <summary>
        /// The object is initialized on start.
        /// </summary>
        private void Start()
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

            Init();
        }

        /// <summary>
        /// Initializes the SFX player.
        /// </summary>
        private void Init()
        {
            // Initializes the AudioSource pool
            InitPool();

            volume = GameManager.Instance.Settings.SFXVolume;
            pitch = 1;

            // Sets the SFX player to not be destroyed when changing scene
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Initializes the AudioSource pool.
        /// </summary>
        private void InitPool()
        {
            audioSrcPool = new List<AudioSource>();

            for (int i = 0; i < audioSrcPoolSize; i++)
            {
                CreateNewAudioSrc();
            }
        }

        /// <summary>
        /// Adds new AudioSources to the pool.
        /// </summary>
        /// <param name="increase">the number of new AudioSources</param>
        /// <returns>the last created AudioSource</returns>
        private AudioSource IncreasePoolSize(int increase)
        {
            AudioSource audioSrc = null;

            if (increase > 0)
            {
                audioSrcPoolSize += increase;

                for (int i = 0; i < increase; i++)
                {
                    audioSrc = CreateNewAudioSrc();
                }
            }

            return audioSrc;
        }

        /// <summary>
        /// Creates a new game object with an AudioSource
        /// component and adds it to the pool.
        /// </summary>
        /// <returns>an AudioSource</returns>
        private AudioSource CreateNewAudioSrc()
        {
            AudioSource audioSrc = null;

            if (audioSrcPrefab != null)
            {
                GameObject audioObj = Instantiate(audioSrcPrefab, transform);
                audioObj.transform.position = transform.position;
                audioSrc = audioObj.GetComponent<AudioSource>();
                audioSrcPool.Add(audioSrc);
            }

            return audioSrc;
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            // Returns any finished AudioSource to the pool to be used again
            ReturnFinishedAudioSrcsToPool();
        }

        /// <summary>
        /// Plays a sound clip which corresponds with the given name.
        /// </summary>
        /// <param name="sound">a sound's name</param>
        public AudioSource Play(Sound sound)
        {
            return Play((int) sound);
        }

        /// <summary>
        /// Plays a sound clip which corresponds with the given name
        /// and has the given pitch.
        /// </summary>
        /// <param name="sound">a sound's name</param>
        /// <param name="pitch">sound's pitch</param>
        public AudioSource Play(Sound sound, float pitch)
        {
            this.pitch = pitch;
            return Play((int) sound);
        }

        /// <summary>
        /// Plays a sound clip with the given number.
        /// </summary>
        /// <param name="soundNum">a sound clip's number</param>
        public AudioSource Play(int soundNum)
        {
            if (soundNum >= 0 &&
                soundNum < sounds.Count)
            {
                // Plays the sound
                return Play(sounds[soundNum]);
            }
            else
            {
                Debug.LogError("[SoundPlayer]: The requested sound " +
                               "clip cannot be played");
            }

            return null;
        }

        /// <summary>
        /// Plays a sound clip.
        /// </summary>
        /// <param name="clip">a sound clip</param>
        private AudioSource Play(AudioClip clip)
        {
            AudioSource audioSrc = GetAudioSrcFromPool();

            // If there are no unused AudioSources
            // and the pool's size is flexible, a
            // new AudioSource is created
            if (audioSrc == null && flexiblePoolSize)
            {
                audioSrc = IncreasePoolSize(1);
                audioSrc.enabled = true;
            }

            // Plays a sound
            if (audioSrc != null)
            {
                audioSrc.clip = clip;
                audioSrc.volume = volume;
                audioSrc.pitch = pitch;
                audioSrc.Play();
                //audioSrc.PlayOneShot(clip, volume);

                pitch = 1;
            }
            // Otherwise prints debug data
            //else
            //{
            //    Debug.Log("[SoundPlayer]: All AudioSources are being used " +
            //              "and a new one could not be created");
            //}

            return audioSrc;
        }

        public AudioSource PlayLooped(Sound sound)
        {
            return PlayLooped((int)sound);
        }

        public AudioSource PlayLooped(Sound sound, float pitch)
        {
            this.pitch = pitch;
            return PlayLooped((int)sound);
        }

        public AudioSource PlayLooped(int soundNum)
        {
            if (soundNum >= 0 &&
                soundNum < sounds.Count)
            {
                // Plays the sound
                return PlayLooped(sounds[soundNum]);
            }
            else
            {
                Debug.LogError("[SoundPlayer]: The requested sound " +
                               "clip cannot be played");
            }

            return null;
        }

        private AudioSource PlayLooped(AudioClip clip)
        {
            AudioSource audioSrc = GetAudioSrcFromPool();

            // If there are no unused AudioSources
            // and the pool's size is flexible, a
            // new AudioSource is created
            if (audioSrc == null && flexiblePoolSize)
            {
                audioSrc = IncreasePoolSize(1);
                audioSrc.enabled = true;
            }

            // Plays a sound
            if (audioSrc != null)
            {
                audioSrc.clip = clip;
                audioSrc.volume = volume;
                audioSrc.pitch = pitch;
                audioSrc.loop = true;
                audioSrc.Play();
                //audioSrc.PlayOneShot(clip, volume);

                pitch = 1;
            }
            // Otherwise prints debug data
            //else
            //{
            //    Debug.Log("[SoundPlayer]: All AudioSources are being used " +
            //              "and a new one could not be created");
            //}

            return audioSrc;
        }

        /// <summary>
        /// Gets an unused AudioSource from the pool.
        /// </summary>
        /// <returns>an unused AudioSource</returns>
        private AudioSource GetAudioSrcFromPool()
        {
            foreach (AudioSource audioSrc in audioSrcPool)
            {
                if (!audioSrc.enabled)
                {
                    audioSrc.enabled = true;
                    return audioSrc;
                }
            }

            //Debug.Log("[SoundPlayer]: All AudioSources are being used");
            return null;
        }

        /// <summary>
        /// Makes all finished sound effects usable again.
        /// </summary>
        private void ReturnFinishedAudioSrcsToPool()
        {
            foreach (AudioSource audioSrc in audioSrcPool)
            {
                if (audioSrc.enabled && !audioSrc.isPlaying)
                {
                    DeactivateAudioSrc(audioSrc);
                }
            }
        }

        /// <summary>
        /// Stops all sound effect playback.
        /// This is called when the scene changes.
        /// </summary>
        public void StopAllSFXPlayback()
        {
            foreach (AudioSource audioSrc in audioSrcPool)
            {
                audioSrc.Stop();
                DeactivateAudioSrc(audioSrc);
                audioSrc.loop = false;
            }
        }

        public void StopIndividualSFX(string clipName)
        {
            foreach (AudioSource audioSrc in audioSrcPool)
            {
                if (audioSrc.enabled && audioSrc.clip.name == clipName)
                {
                    Debug.Log("Stopped sound effect " + audioSrc.clip.name);
                    audioSrc.Stop();
                    audioSrc.loop = false;
                }
            }
        }

        private void DeactivateAudioSrc(AudioSource audioSrc)
        {
            //RemoveForbiddenDuplicate(audioSrc.clip.name);
            audioSrc.enabled = false;
        }

        /// <summary>
        /// Sets the AudioSources' volume.
        /// </summary>
        /// <param name="volume">volume level</param>
        public void SetVolume(float volume)
        {
            this.volume = volume;
        }
    }
}
