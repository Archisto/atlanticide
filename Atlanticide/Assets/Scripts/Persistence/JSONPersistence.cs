using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Atlanticide.Persistence
{
    /// <summary>
    /// Converts data to JSON for saving and loading it.
    /// </summary>
    [Serializable]
    public class JSONPersistence : IPersistence
    {
        public string Extension { get { return ".json"; } }

        public string FilePath { get; private set;}

        /// <summary>
        /// Initializes the JSONPersistence object.
        /// </summary>
        /// <param name="path">The path of the save file
        /// without the file extension</param>
        public JSONPersistence(string path)
        {
            FilePath = path + Extension;
        }

        /// <summary>
        /// Saves data.
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data to save</param>
        public void Save<T>(T data)
        {
            string jsonData = JsonUtility.ToJson(data);
            File.WriteAllText(FilePath, jsonData, Encoding.UTF8);
        }

        /// <summary>
        /// Loads data.
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <returns>loaded data of the given type</returns>
        public T Load<T>()
        {
            if (File.Exists(FilePath))
            {
                string jsonData = File.ReadAllText(FilePath, Encoding.UTF8);
                return JsonUtility.FromJson<T>(jsonData);
            }
            else
            {
                Debug.LogWarning("Could not find file to load.");
                return default(T);
            }
        }
    }
}
