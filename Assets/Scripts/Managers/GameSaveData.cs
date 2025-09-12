using System;

namespace PC
{
    public partial class GameSaveManager
    {
        [Serializable]
        private class GameSaveData
        {
            public string PlayerName;
            [NonSerialized]
            public DateTime StartTime;
            public string StartTimeStr;
            [NonSerialized]
            public DateTime EndTime;
            public string EndTimeStr;
            public int Score;
        }
    }
}