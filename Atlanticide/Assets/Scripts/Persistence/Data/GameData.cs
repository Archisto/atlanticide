using System;
using System.Collections.Generic;

namespace Atlanticide.Persistence
{
    [Serializable]
    public class GameData : ISaveData
    {
        public int ID { get; set; }
        public int LevelsUnlocked;
        public int LevelNum;
        public int Score;

        //public List<PlayerData> PlayerDataList = new List<PlayerData>();
        //public List<UnitData> EnemyDataList = new List<UnitData>();
    }
}
