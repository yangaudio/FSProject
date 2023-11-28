using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;

namespace ThGold.Common {
    public class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObject {
        private static T _instance;

        public static T Instance {
            get {
                return _instance;
            }
        }

        public static async UniTask CreateInstance() {
            if (_instance == null) {
                var key = $"Assets/_DynamicGroups/SO/{typeof(T)}.asset";
                if (YooAssets.CheckLocationValid(key)) {
                    var handle = YooAssets.LoadAssetAsync<T>(key);
                    await handle.ToUniTask();
                    _instance = Instantiate(handle.AssetObject as T);
                    handle.Release();
                }
            }

            if (_instance == null) {
                _instance = CreateInstance<T>();
            }
        }
    }
}