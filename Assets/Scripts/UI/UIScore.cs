using TMPro;
using UnityEngine;

namespace PC
{
    public class UIScore : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText;

        void OnEnable()
        {
            PotionCollector.OnCollectPotion += UpdateScore;
        }

        void OnDisable()
        {
            PotionCollector.OnCollectPotion -= UpdateScore;
        }

        public void UpdateScore(int score)
        {
            _scoreText.text = $"{score}";
        }
    }
}