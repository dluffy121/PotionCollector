using UnityEngine;

namespace PC
{
    /// <summary>
    /// Simple component to mark a GameObject to not be destroyed on scene load.
    /// </summary>
    [DisallowMultipleComponent]
    public class DontDestroyOnLoad : MonoBehaviour
    {
        void Awake() => DontDestroyOnLoad(gameObject);
    }
}