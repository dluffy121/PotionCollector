using System;
using System.Text;
using System.Threading;
using PC.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PC
{
    public class UIMainMenu : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _nameInputField;
        [SerializeField] private TextMeshProUGUI _leaderboardText;
        [SerializeField] private Button _startBtn;

        CancellationTokenSource _updateLeaderboardCTS;

        #region Unity methods

        void Start()
        {
            UpdateLeaderboard();
        }

        void OnDestroy()
        {
            _updateLeaderboardCTS?.Cancel();
            _updateLeaderboardCTS?.Dispose();
            _updateLeaderboardCTS = null;
        }

        void OnEnable()
        {
            GameEventSystem.OnLeaderboardLoaded += OnLeaderboardLoaded;
        }

        void OnDisable()
        {
            GameEventSystem.OnLeaderboardLoaded -= OnLeaderboardLoaded;
        }

        #endregion

        #region Interactions

        public void OnEndEdit_NameInputField(string name)
        {
            if (string.IsNullOrWhiteSpace(_nameInputField.text))
            {
                _startBtn.interactable = false;
                return;
            }

            _startBtn.interactable = true;
            GameSaveManager.SetPlayerName(name);
        }

        public void OnClick_StartBtn()
        {
            GameSaveManager.SetPlayerName(_nameInputField.text);
            GameManager.StartGameplay();
        }

        public void OnClick_RefreshLeaderboard()
        {
            UpdateLeaderboard();
        }

        #endregion

        void UpdateLeaderboard()
        {
            _updateLeaderboardCTS = new CancellationTokenSource();
            _leaderboardText.SetText("No leaderboard data found...");

            GameSaveManager.FetchLeaderboard(_updateLeaderboardCTS.Token);
        }

        void OnLeaderboardLoaded(LeaderboardLoadedArgs args)
        {
            StringBuilder sb = new();
            sb.AppendLine($"{"Name",15} {"Score",10} {"Start",25} {"End",25}");
            sb.AppendLine(new string('-', 100));

            foreach (LeaderboardEntry entry in args.TopScores)
            {
                sb.AppendLine(
                    $"{entry.PlayerName,15} " +
                    $"{entry.Score,10} " +
                    $"{entry.StartTimeStr,25} " +
                    $"{entry.EndTimeStr,25}"
                );
            }

            _leaderboardText.SetText(sb.ToString());
        }
    }
}