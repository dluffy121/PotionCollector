using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using PC.Events;
using UnityEngine;

namespace PC
{
    public partial class GameSaveManager : MonoBehaviour
    {
        static GameSaveManager s_instance;

        private GameSaveData _currentSessionSave;

        private const string SCORE_PATH = "Score";

        private DatabaseReference _databaseReference;

        private bool _isInitialized = false;

        #region Unity Methods

        private void Awake()
        {
            s_instance = this;
        }

        void OnEnable()
        {
            GameEventSystem.OnGameStarted += OnStartGame;
            GameEventSystem.OnGameEnded += OnEndGame;
            GameEventSystem.OnScoreUpdated += OnUpdateScore;
        }

        private void OnDisable()
        {
            GameEventSystem.OnGameStarted -= OnStartGame;
            GameEventSystem.OnGameEnded -= OnEndGame;
            GameEventSystem.OnScoreUpdated -= OnUpdateScore;
        }

        private void OnApplicationPause(bool isPaused)
        {
            if (!isPaused)
                return;

            // ensure session is saved if app is paused
            // user may never return, or app could be cleared from memory if not resumed
            // hence this as fallback
            EndSession(false);
        }

        private void OnApplicationQuit()
        {
            EndSession();
        }

        #endregion

        #region Static Members

        public static bool IsInitialized
            => s_instance._isInitialized;

        public static string CurrentPlayerName
            => s_instance._currentSessionSave.PlayerName;

        public static int CurrentScore
            => s_instance._currentSessionSave.Score;

        public static async Task Initialize()
        {
            await s_instance.InitializeFirebase();
            s_instance.ResetSave();
        }

        public static void SetPlayerName(string playerName)
            => s_instance._currentSessionSave.PlayerName = playerName;

        public static void FetchLeaderboard(CancellationToken token)
            => s_instance.Internal_FetchLeaderboard(token);

        #endregion

        async Task InitializeFirebase()
        {
            try
            {
                await FirebaseApp.CheckAndFixDependenciesAsync();
                _databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("Firebase initialized successfully");

                _isInitialized = true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Firebase initialization failed");
                Debug.LogException(ex);
            }
        }

        void OnStartGame(GameStartedArgs args)
        {
            _currentSessionSave ??= new();
            _currentSessionSave.StartTime = args.Timestamp;
            _currentSessionSave.Score = 0;
        }

        void OnEndGame(GameEndedArgs args)
        {
            EndSession();
        }

        public void OnUpdateScore(ScoreUpdatedArgs args)
        {
            UpdateScore(args.NewScore);
        }

        void EndSession(bool resetSave = true)
        {
            if (_currentSessionSave.StartTime == default)
                return;
            _currentSessionSave.EndTime = DateTime.UtcNow;
            SaveSessionData();
            if (resetSave)
                ResetSave();
        }

        void ResetSave()
            => _currentSessionSave = new();

        void UpdateScore(int newScore)
           => _currentSessionSave.Score = Mathf.Max(_currentSessionSave.Score, newScore);

        async void SaveSessionData()
        {
            if (_databaseReference == null) return;

            try
            {
                GameEventSystem.DispatchFirebaseSyncStarted(FirebaseOperationType.SaveScore);

                _currentSessionSave.StartTimeStr = _currentSessionSave.StartTime.ToString();
                _currentSessionSave.EndTimeStr = _currentSessionSave.EndTime.ToString();

                string json = JsonUtility.ToJson(_currentSessionSave);

                await _databaseReference.Child("scores")
                                        .Child(_currentSessionSave.PlayerName)
                                        .SetRawJsonValueAsync(json);

                Debug.Log($"Session data saved successfully. Player Name: {_currentSessionSave.PlayerName}, Score: {_currentSessionSave.Score}");

                GameEventSystem.DispatchFirebaseSyncCompleted(FirebaseOperationType.SaveScore, true);
            }
            catch (Exception ex)
            {
                GameEventSystem.DispatchFirebaseSyncCompleted(FirebaseOperationType.SaveScore, false);
                Debug.LogError($"Failed to save session data:");
                Debug.LogException(ex);
            }
        }

        async void Internal_FetchLeaderboard(CancellationToken token)
        {
            if (_databaseReference == null) return;

            try
            {
                GameEventSystem.DispatchFirebaseSyncStarted(FirebaseOperationType.LoadLeaderboard);

                Task<DataSnapshot> leaderboardTask = _databaseReference.Child("scores")
                                                                       .OrderByChild(SCORE_PATH)
                                                                       //    .LimitToLast(5)
                                                                       .GetValueAsync();
                await leaderboardTask;

                if (!leaderboardTask.Result.Exists
                    || token.IsCancellationRequested)
                {
                    GameEventSystem.DispatchFirebaseSyncCompleted(FirebaseOperationType.LoadLeaderboard, false);
                    return;
                }

                IEnumerable<DataSnapshot> children = leaderboardTask.Result.Children.Reverse()
                                                                                    .Take(5);

                List<LeaderboardEntry> entries = new(5);

                foreach (DataSnapshot child in children)
                {
                    string json = child.GetRawJsonValue();
                    GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);
                    entries.Add(new(data.PlayerName, data.Score, data.StartTimeStr, data.EndTimeStr));
                }

                GameEventSystem.DispatchFirebaseSyncCompleted(FirebaseOperationType.LoadLeaderboard, true);
                GameEventSystem.DispatchLeaderboardLoaded(entries.ToArray());
            }
            catch (Exception ex)
            {
                GameEventSystem.DispatchFirebaseSyncCompleted(FirebaseOperationType.LoadLeaderboard, false);
                Debug.LogError("Failed to fetch leaderboard data:");
                Debug.LogException(ex);
            }
        }
    }
}