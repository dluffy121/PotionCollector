using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace PC
{
    public class Potion : MonoBehaviour, IPotionHandle
    {
        async void IPotionHandle.Initialize(int lifetime, CancellationToken token)
        {
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