using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace ThGold.Feel
{
    public class FeelAsyncLoadScene : MonoBehaviour
    {
        public string FeelAsyncScene = "FeelAsyncScene";
        private AsyncOperation asyncOperation => FeedBackManager.Instance.asyncLoadSceneOperation;
        public Slider loadingBar;

        void Start()
        {
            // 异步加载场景，但不会立即进入场景
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.E))
            {
                // 允许进入场景
                if (asyncOperation.progress < 0.9f)
                    return;
                asyncOperation.allowSceneActivation = true;
                FeedBackManager.Instance.UnloadScene(FeelAsyncScene);
            }

            // 更新进度条显示
            loadingBar.value = asyncOperation.progress;
        }
    }
}
