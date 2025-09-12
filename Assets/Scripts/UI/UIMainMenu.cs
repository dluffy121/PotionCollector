using System.Threading;
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
            GameManager.StartGame();
        }

        public void OnClick_RefreshLeaderboard()
        {
            UpdateLeaderboard();
        }

        #endregion

        async void UpdateLeaderboard()
        {
            _updateLeaderboardCTS = new();
            string leaderboardStr = await GameSaveManager.GetLeaderboardString(_updateLeaderboardCTS.Token);
            _leaderboardText.SetText(leaderboardStr);
            Debug.Log("loaded");
        }
    }
}