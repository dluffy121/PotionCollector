using UnityEngine;
using UnityEngine.AddressableAssets;

namespace PC
{
    [CreateAssetMenu(fileName = "PotionData", menuName = "FelicityAssgn/Potion Data")]
    public class PotionData : ScriptableObject
    {
        [Header("Basic Info")]
        public string potionName;
        public string description;
        [Range(1, 100)]
        public int potency = 10;
        public PotionType type;

        [Header("Visual Elements")]
        public Color potionColor = Color.white;
        public Sprite icon;

        [Header("Addressables")]
        public AssetReference prefabReference;
        public string[] labels;

        [Header("Effects")]
        public float duration = 5f;
        public ParticleSystem collectEffect;
        public AudioClip collectSound;

        [Header("Behavior")]
        public float rotationSpeed = 90f;
        public float bobAmplitude = 0.5f;
        public float bobFrequency = 1f;
    }
}