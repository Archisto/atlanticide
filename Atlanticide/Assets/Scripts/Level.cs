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
        /// The current puzzle number.
        /// </summary>
        public int CurrentPuzzleNumber { get; set; }

        /// <summary>
        /// The level's scene name. Each puzzle scene has
        /// this in its name along with the puzzle number.
        /// </summary>
        public string LevelSceneName { get; private set; }

        /// <summary>
        /// The number of puzzles the level contains.
        /// </summary>
        public int PuzzleCount { get; private set; }

        /// <summary>
        /// The names of the puzzles
        /// </summary>
        private string[] _puzzleNames;

        /// <summary>
        /// Creates the object.
        /// </summary>
        public Level(int number, string levelSceneName, int puzzleCount)
        {
            Number = number;
            LevelSceneName = levelSceneName;
            PuzzleCount = puzzleCount;
            _puzzleNames = new string[PuzzleCount];
            SetPuzzleNames();
        }

        public string GetPuzzleSceneName(int puzzleNum)
        {
            // Each puzzle scene's name should be in the format "Level1-1"
            // The names the players see can be more artistic.
            // Use SetPuzzleNames(params string[] names) for those.

            if (ValidPuzzleNumber(puzzleNum))
            {
                return LevelSceneName + "-" + puzzleNum;
            }
            else
            {
                return null;
            }
        }

        public string GetPuzzleName(int puzzleNum)
        {
            if (ValidPuzzleNumber(puzzleNum))
            {
                return _puzzleNames[puzzleNum - 1];
            }
            else
            {
                return null;
            }
        }

        public string GetCurrentPuzzleName()
        {
            if (ValidPuzzleNumber(CurrentPuzzleNumber))
            {
                return _puzzleNames[CurrentPuzzleNumber - 1];
            }
            else
            {
                return null;
            }
        }

        public void SetPuzzleNames(params string[] names)
        {
            if (names == null || names.Length == 0)
            {
                // Do nothing
            }
            else if (names.Length < _puzzleNames.Length)
            {
                Debug.LogError("There are not enough puzzle names.");
            }
            else if (names.Length > _puzzleNames.Length)
            {
                Debug.LogError("There are too many puzzle names.");
            }

            for (int i = 0; i < _puzzleNames.Length; i++)
            {
                if (i < names.Length)
                {
                    _puzzleNames[i] = names[i];
                }
                else
                {
                    _puzzleNames[i] = LevelSceneName + "-" + (i + 1);
                }
            }
        }

        /// <summary>
        /// Returns whether the given number is a valid puzzle number.
        /// </summary>
        /// <param name="puzzleNum">A puzzle number</param>
        /// <returns></returns>
        public bool ValidPuzzleNumber(int puzzleNum)
        {
            return (puzzleNum >= 1 && puzzleNum <= PuzzleCount);
        }
    }
}
