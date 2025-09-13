using System;
using UnityEngine;

namespace PC.Events
{
    #region Event Arguments

    public readonly struct GameStartedArgs
    {
        public DateTime Timestamp { get; }
        public string SessionId { get; }

        public GameStartedArgs(DateTime timestamp, string sessionId)
        {
            Timestamp = timestamp;
            SessionId = sessionId;
        }
    }

    public readonly struct GamePausedArgs
    {
        public DateTime Timestamp { get; }

        public GamePausedArgs(DateTime timestamp)
        {
            Timestamp = timestamp;
        }
    }

    public readonly struct GameResumedArgs
    {
        public DateTime Timestamp { get; }

        public GameResumedArgs(DateTime timestamp)
        {
            Timestamp = timestamp;
        }
    }

    public readonly struct GameEndedArgs
    {
        public DateTime Timestamp { get; }
        public int TotalScore { get; }

        public GameEndedArgs(DateTime timestamp, int totalScore)
        {
            Timestamp = timestamp;
            TotalScore = totalScore;
        }
    }

    public readonly struct PotionSpawnedArgs
    {
        public PotionType PotionType { get; }
        public Vector3 Position { get; }

        public PotionSpawnedArgs(PotionType potionType, Vector3 position)
        {
            PotionType = potionType;
            Position = position;
        }
    }

    public readonly struct PotionCollectedArgs
    {
        public PotionType PotionType { get; }
        public int Value { get; }
        public DateTime Timestamp { get; }

        public PotionCollectedArgs(PotionType potionType, int value, DateTime timestamp)
        {
            PotionType = potionType;
            Value = value;
            Timestamp = timestamp;
        }
    }

    public readonly struct ScoreUpdatedArgs
    {
        public int NewScore { get; }
        public int ScoreDelta { get; }

        public ScoreUpdatedArgs(int newScore, int scoreDelta)
        {
            NewScore = newScore;
            ScoreDelta = scoreDelta;
        }
    }

    public readonly struct LeaderboardLoadedArgs
    {
        public LeaderboardEntry[] TopScores { get; }

        public LeaderboardLoadedArgs(LeaderboardEntry[] topScores)
        {
            TopScores = topScores;
        }
    }

    public readonly struct FirebaseSyncArgs
    {
        public FirebaseOperationType OperationType { get; }
        public bool Success { get; }

        public FirebaseSyncArgs(FirebaseOperationType operationType, bool success = true)
        {
            OperationType = operationType;
            Success = success;
        }
    }
    #endregion

    #region Supporting Types

    public enum FirebaseOperationType
    {
        SaveScore,
        LoadLeaderboard,
    }

    public readonly struct LeaderboardEntry
    {
        public readonly string PlayerName;
        public readonly int Score;
        public readonly string StartTimeStr;
        public readonly string EndTimeStr;

        public LeaderboardEntry(string playerName, int score, string startTimeStr = "", string endTimeStr = "")
        {
            PlayerName = playerName;
            Score = score;
            StartTimeStr = startTimeStr;
            EndTimeStr = endTimeStr;
        }
    }

    #endregion
}