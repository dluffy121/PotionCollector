using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace PC
{
    public class Potion : MonoBehaviour
    {
        [SerializeField] Renderer _renderer;
        [SerializeField] AudioSource _audioSource;
        ParticleSystem _particleSystem;

        PotionData _data;

        public PotionType PotionType => _data.type;

        internal async void Initialize(int lifetime, PotionData data, CancellationToken token)
        {
            _data = data;

            if (data.collectEffect != null)
            {
                _particleSystem = Instantiate(data.collectEffect, transform);
                _particleSystem.Stop();
            }

            MaterialPropertyBlock propertyBlock = new();
            propertyBlock.SetColor("_BaseColor", data.potionColor);
            _renderer.SetPropertyBlock(propertyBlock);

            await Task.Delay(lifetime, token);

            Despawn();
        }

        void Despawn()
        {
            PotionSpawner.DespawnPotion(this);
        }

        internal void OnDespawn()
        {
        }

        internal void OnCollect()
        {
            if (_particleSystem != null)
            {
                _particleSystem.transform.parent = null;
                _particleSystem.Play();
                Destroy(_particleSystem.gameObject, _particleSystem.main.duration);
            }

            if (_data.collectSound != null)
                _audioSource.PlayOneShot(_data.collectSound);
        }
    }
}