using System.Threading.Tasks;
using UnityEngine;

namespace PC
{
    public class PotionSpawner : MonoBehaviour
    {
        static PotionSpawner s_instance;

        [SerializeField] PotionSpawnerData _spawnerData;

        #region Unity methods

        void Awake()
        {
            s_instance = this;
        }

        void OnDestroy()
        {
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

        public static void DespawnPotion(Potion potionRef)
        {
            s_instance.Internal_DespawnPotion(potionRef);
        }

        #endregion

        async void StartSpawning()
        {
            while (true)
            {
                SpawnPotion();

                await Task.Delay(_spawnerData.SpawnTime);
            }
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
            potion.transform.SetPositionAndRotation(position, rotation);
        }

        void Internal_DespawnPotion(Potion potionRef)
        {
            Destroy(potionRef.gameObject);
        }
    }
}