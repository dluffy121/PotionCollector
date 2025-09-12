using System.Threading;

namespace PC
{
    internal interface IPotionHandle
    {
        void Initialize(int lifetime, CancellationToken token = default);
        void OnDespawn();
        void OnCollect();
    }
}