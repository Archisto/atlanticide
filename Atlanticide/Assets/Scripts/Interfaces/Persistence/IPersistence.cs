namespace Atlanticide.Persistence
{
    public interface IPersistence
    {
        /// <summary>
        /// The file extension of the save file
        /// </summary>
        string Extension { get; }

        /// <summary>
        /// The path of the save file on disk
        /// </summary>
        string FilePath { get; }

        /// <summary>
        /// A generic method for serializing the game data and storing it to the disk.
        /// </summary>
        /// <typeparam name="T">Any class that has data
        /// which should be saved</typeparam>
        /// <param name="data">The data to be saved</param>
        void Save<T>(T data);

        /// <summary>
        /// A method for reading serialized data and returning it.
        /// </summary>
        /// <typeparam name="T">Any class</typeparam>
        /// <returns>saved data</returns>
        T Load<T>();
    }
}
