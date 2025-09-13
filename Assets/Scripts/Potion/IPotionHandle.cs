using System.Threading;

namespace PC
{
    internal interface IPotionHandle
    {
        void Initialize(int lifetime, PotionType potionType, CancellationToken token = default);
        void OnDespawn();
        void OnCollect();
    }
}