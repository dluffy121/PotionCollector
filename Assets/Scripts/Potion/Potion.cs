using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace PC
{
    public class Potion : MonoBehaviour, IPotionHandle
    {
        public PotionType PotionType { get; internal set; }

        async void IPotionHandle.Initialize(int lifetime, PotionType potionType, CancellationToken token)
        {
            PotionType = potionType;

            await Task.Delay(lifetime, token);

            Despawn();
        }

        void Despawn()
        {
            PotionSpawner.DespawnPotion(this);
        }

        void IPotionHandle.OnDespawn()
        {
        }

        void IPotionHandle.OnCollect()
        {
        }
    }
}