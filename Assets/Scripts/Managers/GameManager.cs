using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            await AddressablesHandler.Initialize();

            Initialize();
        }

        private void Initialize()
        {
            AsyncOperation mainMenuSceneLoadOP = SceneManager.LoadSceneAsync(_mainMenuSceneIndex);
            mainMenuSceneLoadOP.completed += static async op =>
            {
                await s_instance.LoadRemotePotionData();
                // if games starts before this completes,
                // it should work fine as the new spawns will update accordingly
                // can be handled later by blocking Start btn or add loading screen
            };
        }

        #region Potions remote updating

        [Header("Potions Remote Update")]
        [SerializeField] PotionSpawnerData _spawnerData;

        async Task LoadRemotePotionData()
        {
            try
            {
                List<PotionData> remotePotions = await AddressablesHandler.LoadPotionData();
                if (remotePotions.Count <= 0)
                {
                    Debug.LogWarning("No remote potions found, falling back to local potions");
                    return;
                }

                _spawnerData.UpdatePotionDataList(remotePotions);
                Debug.LogError($"Loaded {remotePotions.Count} potion types from remote catalog");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load remote potions: {e.Message}");
                Debug.LogWarning("Falling back to local potions");
            }
        }

        #endregion
    }
}