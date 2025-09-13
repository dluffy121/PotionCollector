using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace PC
{
    public enum PotionType
    {
        Red,
        Blue,
        Green,
        Golden
    }

    public class PotionSpawner : MonoBehaviour
    {
        private class PotionSpawn
        {
            public Potion PotionRef;
            public CancellationTokenSource CTS = new();
        }

        static PotionSpawner s_instance;

        [SerializeField] PotionSpawnerData _spawnerData;

        List<PotionSpawn> _potions = new();

        CancellationTokenSource _spawningCTS;

        #region Unity methods

        void Awake()
        {
            s_instance = this;
        }

        void OnDestroy()
        {
            StopSpawning();
            s_instance = null;
        }

        void Start()
        {
            StartSpawning();
        }

        #endregion

        #region Static members

        public static void SpawnPotion()
        {
            s_instance.Internal_SpawnPotion();
        }

        public static void CollectPotion(Potion potion)
        {
            s_instance.Internal_CollectPotion(potion);
        }

        public static void DespawnPotion(Potion potionRef)
        {
            s_instance.Internal_DespawnPotion(potionRef);
        }

        #endregion

        async void StartSpawning()
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
            _potions.ForEach(ps =>
            {
                ps.CTS.Cancel();
                ps.CTS.Dispose();
            });
            _potions.Clear();
            _spawningCTS.Cancel();
            _spawningCTS.Dispose();
            _spawningCTS = null;
        }

        void Internal_SpawnPotion()
        {
            Vector3 position = new(
                Random.Range(-_spawnerData.SpawnArea.x, _spawnerData.SpawnArea.x),
                Random.Range(-_spawnerData.SpawnArea.y, _spawnerData.SpawnArea.y),
                0
            );

            Quaternion rotation = Quaternion.Euler(
                0,
                Random.Range(0, 360),
                0
            );

            Potion potion = Instantiate(_spawnerData.PotionPrefab, transform);
            PotionSpawn potionSpawn = new() { PotionRef = potion };
            (potion as IPotionHandle).Initialize(_spawnerData.Lifetime, PotionType.Red, potionSpawn.CTS.Token);
            potion.transform.SetPositionAndRotation(position, rotation);

            _potions.Add(potionSpawn);
        }

        void Internal_DespawnPotion(Potion potion)
        {
            (potion as IPotionHandle).OnDespawn();

            DestroyPotion(potion);
        }

        void Internal_CollectPotion(Potion potion)
        {
            (potion as IPotionHandle).OnCollect();

            GameEventSystem.DispatchPotionCollected(potion.PotionType, 1);

            DestroyPotion(potion);
        }

        void DestroyPotion(Potion potionRef)
        {
            for (int i = _potions.Count - 1; i >= 0; i--)
            {
                if (_potions[i].PotionRef != potionRef)
                    continue;

                _potions[i].CTS.Cancel();
                _potions[i].CTS.Dispose();
                _potions.RemoveAt(i);
            }
            Destroy(potionRef.gameObject);
        }
    }
}