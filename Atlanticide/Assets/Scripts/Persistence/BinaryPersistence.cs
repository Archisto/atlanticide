using System;
using System.IO;
using System.Runtime.Serialization;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

namespace Atlanticide.Persistence
{
    [Serializable]
    public class BinaryPersistence : IPersistence
    {
        public string Extension { get { return ".bin"; } }

        public string FilePath { get; private set;}

        /// <summary>
        /// Initializes the BinaryPersistence object.
        /// </summary>
        /// <param name="path">The path of the save file
        /// without the file extension</param>
        public BinaryPersistence(string path)
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
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }

            using (FileStream stream = File.OpenWrite(FilePath))
            {
                BinaryFormatter bf = new BinaryFormatter();

                var surrogateSelector = new SurrogateSelector();
                Vector3Surrogate v3ss = new Vector3Surrogate();
                surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), v3ss);
                bf.SurrogateSelector = surrogateSelector;

                bf.Serialize(stream, data);

                // Calling the stream.Close() is not necessary when using
                // the 'using' statement. When the execution leaves the
                // stream's scope, the Dispose method is called automatically
                // by stream.Close().
                //stream.Close();
            }
        }

        /// <summary>
        /// Loads data.
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <returns>loaded data of the given type</returns>
        public T Load<T>()
        {
            T data = default(T);

            if (File.Exists(FilePath))
            {
                // If we are not using the 'using' statement we have to make
                // sure that the stream is correctly closed in case of an
                // Exception being thrown. The finally block makes sure that
                // the stream is closed correctly in every case.
                FileStream stream = File.OpenRead(FilePath);
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    data = (T) bf.Deserialize(stream);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                finally
                {
                    stream.Close();
                } 
            }

            return data;
        }
    }
}
