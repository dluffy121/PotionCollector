using System.Collections.Generic;
using UnityEngine;

namespace PC
{
    [CreateAssetMenu(fileName = "PotionSpawnerData", menuName = "FelicityAssgn/Potion Spawner Data")]
    public class PotionSpawnerData : ScriptableObject
    {
        [SerializeField] private List<PotionData> _potionDataList;
        [SerializeField] private int _spawnTimeMS;
        [SerializeField] private Vector2 _spawnArea;
        [SerializeField] private int _lifetimeMS;

        public int SpawnTime => _spawnTimeMS;
        public Vector2 SpawnArea => _spawnArea;
        public int Lifetime => _lifetimeMS;

        public PotionData GetRandomData()
            => _potionDataList[Random.Range(0, _potionDataList.Count)];

        public void UpdatePotionDataList(List<PotionData> potionDataList)
            => _potionDataList = potionDataList;
    }
}