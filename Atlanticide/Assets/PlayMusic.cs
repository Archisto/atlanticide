using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class PlayMusic : MonoBehaviour
    {
        [SerializeField]
        private int trackNum;

        // Use this for initialization
        void Start()
        {
            int currentTrack = MusicPlayer.Instance.currentTrack;
            if (!MusicPlayer.Instance.IsPlaying || currentTrack != trackNum)
            {
                MusicPlayer.Instance.Play(trackNum);
            }
        }
    }
}
