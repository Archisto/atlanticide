using System;

namespace Atlanticide.Persistence
{
    [Serializable]
    public class PlayerData : ISaveData
    {
        public int ID { get; set; }
        public PlayerTool Tool;
    }
}
