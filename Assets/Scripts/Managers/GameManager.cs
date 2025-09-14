using System;
using PC.Events;
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

        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
                GameEventSystem.DispatchGamePaused();
            else
                GameEventSystem.DispatchGameResumed();
        }

        #endregion

        #region Static Members

        public static void StartGameplay()
        {
            SceneManager.LoadSceneAsync(s_instance._gameSceneIndex)
                        .completed += OnGameSceneLoaded;
        }

        public static void EndGameplay()
        {
            GameEventSystem.DispatchGameEnded(GameSaveManager.CurrentScore);
            s_instance._player = null;
            SceneManager.LoadSceneAsync(s_instance._mainMenuSceneIndex);
        }

        private static void OnGameSceneLoaded(AsyncOperation op)
        {
            GameEventSystem.DispatchGameStarted(GameSaveManager.CurrentPlayerName + "_" + DateTime.UtcNow.Ticks);
        }

        public static void RegisterPlayer(PotionCollector potionCollector)
        {
            s_instance._player = potionCollector;
        }

        #endregion

        private async void PrepareGame()
        {
            await GameSaveManager.Initialize();
                Initialize();

            Initialize();
        }

        private void Initialize()
        {
            AsyncOperation mainMenuSceneLoadOP = SceneManager.LoadSceneAsync(_mainMenuSceneIndex);

    }
}