using System;
using UnityEngine;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace PC
{
    public class GameManager : MonoBehaviour
    {
        static GameManager s_instance;

        [SerializeField] private int _mainMenuSceneIndex = 1;
        [SerializeField] private int _gameSceneIndex = 2;

        private PotionCollector _player;

        #region Unity Methods

        void Awake()
        {
            s_instance = this;
        }

        void Start()
        {
            PrepareGame();
        }

        void OnDestroy()
        {
            s_instance = null;
        }

        #endregion

        #region Static Members

        public static event Action OnGameStart;
        public static event Action OnGameEnd;
        public static event Action<int> OnScoreUpdate;

        public static void StartGame()
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(s_instance._gameSceneIndex);
            op.completed += static op =>
            {
                s_instance._player.OnCollectPotion += OnCollectPotion;
                GameSaveManager.StartSession();
            };
        }

        private static void OnCollectPotion(int score)
        {
            OnScoreUpdate?.Invoke(score);
        }

        public static void EndGame()
        {
            GameSaveManager.EndSession();
            s_instance._player = null;
            SceneManager.LoadSceneAsync(s_instance._mainMenuSceneIndex);
        }

        public static void RegisterPlayer(PotionCollector potionCollector)
        {
            s_instance._player = potionCollector;
        }

        #endregion

        private void PrepareGame()
        {
            if (GameSaveManager.IsInitialized)
                Initialize();
            else
                GameSaveManager.OnInitialized += Initialize;
        }

        private void Initialize()
        {
            SceneManager.LoadSceneAsync(_mainMenuSceneIndex);
        }
    }
}