using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace PC
{
    public class AddressablesHandler : MonoBehaviour
    {
        private const string REMOTE_CATALOG_URL = "https://FelicityAssign-PotionCollector.b-cdn.net/addressables/catalog_1.0.0.json";
        private const string POTIONS_DATA_GROUP_LABEL = "PotionsData";

        public static event Action<float> OnDownloadProgress;
        public static event Action<string> OnError;

        private static AddressablesHandler s_instance;
        private bool _isInitialized;

        void Awake()
        {
            s_instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void OnDestroy()
        {
            s_instance = null;
        }

        public async static Task Initialize()
        {
            try
            {
                await Addressables.CleanBundleCache().Task;

                await Addressables.InitializeAsync().Task;

                Addressables.ClearResourceLocators();

                AsyncOperationHandle<IResourceLocator> loadCatalogHandle = Addressables.LoadContentCatalogAsync(REMOTE_CATALOG_URL);
                await loadCatalogHandle.Task;

                if (loadCatalogHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log($"Loaded catalog from: {REMOTE_CATALOG_URL}");

                    foreach (var locator in Addressables.ResourceLocators)
                    {
                        Debug.Log($"Available locator: {locator.LocatorId}");

                        foreach (var key in locator.Keys)
                            Debug.Log($"Key in locator: {key}");
                    }
                }
                else
                {
                    Debug.LogError($"Failed to load catalog from: {REMOTE_CATALOG_URL}");
                    OnError?.Invoke("Failed to load remote catalog");
                    return;
                }

                Addressables.Release(loadCatalogHandle);
                s_instance._isInitialized = true;
                Debug.Log("Addressables initialization completed successfully");
            }
            catch (Exception e)
            {
                OnError?.Invoke($"Failed to initialize Addressables: {e.Message}");
                Debug.LogException(e);
                s_instance._isInitialized = false;
            }
        }

        public static async Task<List<PotionData>> LoadPotionData()
        {
            List<PotionData> potionData = new();

            if (!s_instance._isInitialized)
            {
                Debug.LogError("Addressables not initialized. Call Initialize() first.");
                return potionData;
            }

            try
            {
                AsyncOperationHandle<IList<IResourceLocation>> loadResourceLocationsHandle =
                    Addressables.LoadResourceLocationsAsync(new List<object> { POTIONS_DATA_GROUP_LABEL },
                                                            Addressables.MergeMode.UseFirst);

                await loadResourceLocationsHandle.Task;

                if (loadResourceLocationsHandle.Status != AsyncOperationStatus.Succeeded)
                {
                    OnError?.Invoke("Failed to load potion locations");
                    return potionData;
                }

                IList<IResourceLocation> locations = loadResourceLocationsHandle.Result;
                float totalAssets = locations.Count;
                float loadedAssets = 0;

                foreach (IResourceLocation location in locations)
                {
                    AsyncOperationHandle<PotionData> loadAssetHandle =
                        Addressables.LoadAssetAsync<PotionData>(location);
                    await loadAssetHandle.Task;

                    if (loadAssetHandle.Status == AsyncOperationStatus.Succeeded
                        && loadAssetHandle.Result != null)
                        potionData.Add(loadAssetHandle.Result);

                    loadedAssets++;
                    OnDownloadProgress?.Invoke(loadedAssets / totalAssets);
                }

                Addressables.Release(loadResourceLocationsHandle);
            }
            catch (Exception e)
            {
                OnError?.Invoke($"Error loading potion data:");
                Debug.LogException(e);
            }

            return potionData;
        }
    }
}