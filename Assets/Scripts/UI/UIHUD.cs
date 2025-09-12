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
            GameManager.OnScoreUpdate += UpdateScore;
            _nameText.SetText(GameSaveManager.CurrentPlayerName);
        }

        void OnDisable()
        {
            GameManager.OnScoreUpdate -= UpdateScore;
        }

        void UpdateScore(int score)
        {
            _scoreText.text = $"{score}";
        }

        public void OnClick_ExitBtn()
        {
            GameManager.EndGame();
        }
    }
}