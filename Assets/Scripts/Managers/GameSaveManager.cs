using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
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
            InitializeFirebase();
            Internal_ResetSave();
        }

        void OnEnable()
        {
            GameManager.OnScoreUpdate += UpdateScore;
        }

        private void OnDisable()
        {
            GameManager.OnScoreUpdate -= UpdateScore;
        }

        private void OnApplicationPause(bool isPaused)
        {
            if (!isPaused)
                return;

            // ensure session is saved if app is paused
            // user may never return, or app could be cleared from memory if not resumed
            // hence this as fallback
            // EndSession(false);
        }

        private void OnApplicationQuit()
        {
            EndSession();
        }

        #endregion

        #region Static Members

        public static event Action OnInitialized;

        public static bool IsInitialized
            => s_instance._isInitialized;

        public static string CurrentPlayerName
            => s_instance._currentSessionSave.PlayerName;

        public static void StartSession()
            => s_instance.Internal_StartSession();

        public static void EndSession(bool resetSave = true)
        {
            if (s_instance._currentSessionSave == null
                || s_instance._currentSessionSave.StartTime == default)
                return;
            s_instance.Internal_EndSession();
            if (resetSave)
                s_instance.Internal_ResetSave();
        }

        public static int GetCurrentMaxScore()
            => s_instance._currentSessionSave.Score;

        public static void SetPlayerName(string playerName)
            => s_instance._currentSessionSave.PlayerName = playerName;

        public static void UpdateScore(int score)
            => s_instance.Internal_UpdateScore(score);

        public static Task<string> GetLeaderboardString(CancellationToken token = default)
            => s_instance.FetchLeaderboard(token);

        #endregion

        async void InitializeFirebase()
        {
            try
            {
                await FirebaseApp.CheckAndFixDependenciesAsync();
                _databaseReference = FirebaseDatabase.GetInstance("https://potioncollector-becde-default-rtdb.asia-southeast1.firebasedatabase.app/").RootReference;
                Debug.Log("Firebase initialized successfully");

                _isInitialized = true;

                OnInitialized?.Invoke();
                OnInitialized = null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Firebase initialization failed");
                Debug.LogException(ex);
            }
        }

        void Internal_StartSession()
        {
            _currentSessionSave ??= new();
            _currentSessionSave.StartTime = DateTime.UtcNow;
            _currentSessionSave.Score = 0;
        }

        void Internal_EndSession()
        {
            s_instance._currentSessionSave.EndTime = DateTime.UtcNow;
            s_instance.SaveSessionData();
        }

        void Internal_ResetSave()
        {
            s_instance._currentSessionSave = new();
        }

        void Internal_UpdateScore(int newScore)
           => _currentSessionSave.Score = Mathf.Max(_currentSessionSave.Score, newScore);

        async void SaveSessionData()
        {
            if (_databaseReference == null) return;

            try
            {
                _currentSessionSave.StartTimeStr = _currentSessionSave.StartTime.ToString();
                _currentSessionSave.EndTimeStr = _currentSessionSave.EndTime.ToString();

                string json = JsonUtility.ToJson(_currentSessionSave);

                await _databaseReference.Child("scores")
                                        .Child(_currentSessionSave.PlayerName)
                                        .SetRawJsonValueAsync(json);

                Debug.Log($"Session data saved successfully. Player Name: {_currentSessionSave.PlayerName}, Score: {_currentSessionSave.Score}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to save session data:");
                Debug.LogException(ex);
            }
        }

        async Task<string> FetchLeaderboard(CancellationToken token)
        {
            if (_databaseReference == null) return string.Empty;

            try
            {
                Task<DataSnapshot> leaderboardTask = _databaseReference.Child("scores")
                                                                       .OrderByChild(SCORE_PATH)
                                                                       //    .LimitToLast(5)
                                                                       .GetValueAsync();
                await leaderboardTask;

                if (!leaderboardTask.Result.Exists
                    || token.IsCancellationRequested)
                    return "No leaderboard data found...";

                StringBuilder sb = new();
                sb.AppendLine($"{"Name",15} {"Score",10} {"Start",25} {"End",25}");
                sb.AppendLine(new string('-', 100));

                IEnumerable<DataSnapshot> children = leaderboardTask.Result.Children.Reverse()
                                                                                    .Take(5);
                foreach (DataSnapshot child in children)
                {
                    string json = child.GetRawJsonValue();
                    GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);
                    sb.AppendLine(
                        $"{data.PlayerName,15} " +
                        $"{data.Score,10} " +
                        $"{data.StartTimeStr,25} " +
                        $"{data.EndTimeStr,25}"
                    );
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to fetch leaderboard data:");
                Debug.LogException(ex);
                return string.Empty;
            }
        }
    }
}