using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PC.Events;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;

namespace PC
{
    public class PotionSpawner : MonoBehaviour
    {
        private class PotionSpawn
        {
            public CancellationTokenSource CTS = new();
            public AsyncOperationHandle<GameObject> OpHandle;
        }

        static PotionSpawner s_instance;

        [SerializeField] PotionSpawnerData _spawnerData;

        Dictionary<Potion, PotionSpawn> _potions = new();

        CancellationTokenSource _spawningCTS;

        #region Unity methods

        void Awake()
        {
            s_instance = this;

            GameEventSystem.OnGameStarted += OnGameStarted;
        }

        void OnDestroy()
        {
            StopSpawning();
            s_instance = null;
        }

        #endregion

        #region Static members

        public static void CollectPotion(Potion potion)
        {
            s_instance.Internal_CollectPotion(potion);
        }

        public static void DespawnPotion(Potion potionRef)
        {
            s_instance.Internal_DespawnPotion(potionRef);
        }

        #endregion

        void OnGameStarted(GameStartedArgs args)
        {
            GameEventSystem.OnGameStarted -= OnGameStarted;
            Internal_StartSpawning();
        }

        async void Internal_StartSpawning()
        {
            _spawningCTS = new CancellationTokenSource();
            while (true)
            {
                SpawnPotion();

                await Task.Delay(_spawnerData.SpawnTime, _spawningCTS.Token);
            }
        }

        void StopSpawning()
        {
            foreach (var ps in _potions)
            {
                ps.Value.CTS.Cancel();
                ps.Value.CTS.Dispose();
                Addressables.ReleaseInstance(ps.Value.OpHandle);
            }
            _potions.Clear();
            _spawningCTS.Cancel();
            _spawningCTS.Dispose();
            _spawningCTS = null;
        }

        async void SpawnPotion()
        {
            Vector3 position = new(
                Random.Range(-_spawnerData.SpawnArea.x, _spawnerData.SpawnArea.x),
                Random.Range(-_spawnerData.SpawnArea.y, _spawnerData.SpawnArea.y),
                0
            );

            PotionData potionData = _spawnerData.GetRandomData();

            PotionSpawn potionSpawn = new()
            {
                OpHandle = Addressables.InstantiateAsync(potionData.prefabReference,
                                                         position,
                                                         Quaternion.identity)
            };

            await potionSpawn.OpHandle.Task;

            if (potionSpawn.OpHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Failed to spawn potion: {potionData.potionName}");
                return;
            }

            Potion potion = potionSpawn.OpHandle.Result.GetComponent<Potion>();
            potion.Initialize(_spawnerData.Lifetime, potionData, potionSpawn.CTS.Token);

            GameEventSystem.DispatchPotionSpawned(potionData.type, position);

            _potions.Add(potion, potionSpawn);
        }

        void Internal_DespawnPotion(Potion potion)
        {
            potion.OnDespawn();

            DestroyPotion(potion);
        }

        void Internal_CollectPotion(Potion potion)
        {
            potion.OnCollect();

            GameEventSystem.DispatchPotionCollected(potion.PotionType, 1);

            DestroyPotion(potion);
        }

        void DestroyPotion(Potion potionRef)
        {
            if (!_potions.TryGetValue(potionRef, out PotionSpawn potionSpawn))
            {
                Debug.LogWarning("Trying to despawn a potion that isn't managed by the spawner.");
                return;
            }

            _potions.Remove(potionRef);

            potionSpawn.CTS.Cancel();
            potionSpawn.CTS.Dispose();
            Addressables.ReleaseInstance(potionSpawn.OpHandle);
        }
    }
}