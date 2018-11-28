using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class Level
    {
        /// <summary>
        /// The level number.
        /// </summary>
        public int Number { get; private set; }

        /// <summary>
        /// The level's name that is shown to the player.
        /// </summary>
        public string LevelName { get; private set; }

        /// <summary>
        /// The level's scene name.
        /// </summary>
        public string LevelSceneName { get; private set; }

        /// <summary>
        /// Creates the object.
        /// </summary>
        public Level(int number, string levelSceneName, string levelName)
        {
            Number = number;
            LevelSceneName = levelSceneName;
            LevelName = levelName;
        }
    }
}
