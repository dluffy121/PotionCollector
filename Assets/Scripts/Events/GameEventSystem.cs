using System;
using UnityEngine;
using PC.Events;

namespace PC
{
    public static class GameEventSystem
    {
        #region Events

        public static event Action<GameStartedArgs> OnGameStarted;
        public static event Action<GamePausedArgs> OnGamePaused;
        public static event Action<GameResumedArgs> OnGameResumed;
        public static event Action<GameEndedArgs> OnGameEnded;
        public static event Action<PotionSpawnedArgs> OnPotionSpawned;
        public static event Action<PotionCollectedArgs> OnPotionCollected;
        public static event Action<ScoreUpdatedArgs> OnScoreUpdated;
        public static event Action<LeaderboardLoadedArgs> OnLeaderboardLoaded;
        public static event Action<FirebaseSyncArgs> OnFirebaseSyncStarted;
        public static event Action<FirebaseSyncArgs> OnFirebaseSyncCompleted;

        #endregion

        #region Dispatch Methods

        public static void DispatchGameStarted(string sessionId)
            => OnGameStarted?.Invoke(new(DateTime.UtcNow, sessionId));

        public static void DispatchGamePaused()
            => OnGamePaused?.Invoke(new(DateTime.UtcNow));

        public static void DispatchGameResumed()
            => OnGameResumed?.Invoke(new(DateTime.UtcNow));

        public static void DispatchGameEnded(int totalScore)
            => OnGameEnded?.Invoke(new(DateTime.UtcNow, totalScore));

        public static void DispatchPotionSpawned(PotionType potionType, Vector3 position)
            => OnPotionSpawned?.Invoke(new(potionType, position));

        public static void DispatchPotionCollected(PotionType potionType, int value)
            => OnPotionCollected?.Invoke(new(potionType, value, DateTime.UtcNow));

        public static void DispatchScoreUpdated(int newScore, int scoreDelta)
            => OnScoreUpdated?.Invoke(new(newScore, scoreDelta));

        public static void DispatchLeaderboardLoaded(LeaderboardEntry[] topScores)
            => OnLeaderboardLoaded?.Invoke(new(topScores));

        public static void DispatchFirebaseSyncStarted(FirebaseOperationType operationType)
            => OnFirebaseSyncStarted?.Invoke(new(operationType));

        public static void DispatchFirebaseSyncCompleted(FirebaseOperationType operationType, bool success)
            => OnFirebaseSyncCompleted?.Invoke(new(operationType, success));

        #endregion
    }
}