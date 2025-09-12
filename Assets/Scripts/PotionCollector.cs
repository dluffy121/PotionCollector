using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PC
{
    public class PotionCollector : MonoBehaviour
    {
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private LayerMask _potionLayer;

        public static event Action<int> OnCollectPotion;

        private int _score = 0;

        private void Awake()
        {
            if (_mainCamera == null)
                _mainCamera = Camera.main;
        }

        private void Update()
        {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            if (Touchscreen.current == null || !Touchscreen.current.primaryTouch.press.wasReleasedThisFrame)
                return;
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            CheckForPotion(touchPosition);
#else
            if (Mouse.current == null || !Mouse.current.leftButton.wasReleasedThisFrame)
                return;
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            CheckForPotion(mousePosition);
#endif
        }

        private void CheckForPotion(Vector2 inputPosition)
        {
            if (!Physics.Raycast(_mainCamera.ScreenPointToRay(inputPosition),
                                 out RaycastHit hit,
                                 100,
                                 _potionLayer))
                return;

            CollectPotion(hit.collider.gameObject);
        }

        private void CollectPotion(GameObject potionGO)
        {
            if (!potionGO.TryGetComponent(out Potion potion))
                return;

            _score++;

            potion.OnCollect();

            OnCollectPotion?.Invoke(_score);

            PotionSpawner.DespawnPotion(potion);
        }
    }
}