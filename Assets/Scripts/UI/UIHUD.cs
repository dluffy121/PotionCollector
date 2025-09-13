using PC.Events;
using TMPro;
using UnityEngine;

namespace PC
{
    public class UIHUD : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _scoreText;

        void OnEnable()
        {
            GameEventSystem.OnScoreUpdated += UpdateScore;
            _nameText.SetText(GameSaveManager.CurrentPlayerName);
        }

        void OnDisable()
        {
            GameEventSystem.OnScoreUpdated -= UpdateScore;
        }

        void UpdateScore(ScoreUpdatedArgs args)
        {
            _scoreText.text = $"{args.NewScore}";
        }

        public void OnClick_ExitBtn()
        {
            GameManager.EndGame();
        }
    }
}