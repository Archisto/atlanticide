using System;

namespace Atlanticide.Persistence
{
    public interface ISavable
    {
        /// <summary>
        /// The object's save ID.
        /// </summary>
        int ID { get; set; }

        /// <summary>
        /// Returns the object's save data.
        /// </summary>
        ISaveData GetSaveData();

        /// <summary>
        /// Sets the object's values from the save data.
        /// </summary>
        /// <param name="data">Save data</param>
        void SetData(ISaveData data);

        /// <summary>
        /// Returns the correct save data type.
        /// </summary>
        /// <returns>The save data type</returns>
        Type GetSaveDataType();
    }
}
