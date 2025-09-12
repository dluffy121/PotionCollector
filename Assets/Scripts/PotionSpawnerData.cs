using UnityEngine;

namespace PC
{
    [CreateAssetMenu(menuName = "FelicityAssgn/PotionSpawnerData")]
    public class PotionSpawnerData : ScriptableObject
    {
        [SerializeField] private Potion _potionPrefab;
        [SerializeField] private int _spawnTimeMS;
        [SerializeField] private Vector2 _spawnArea;

        public Potion PotionPrefab => _potionPrefab;
        public int SpawnTime => _spawnTimeMS;
        public Vector2 SpawnArea => _spawnArea;
    }
}