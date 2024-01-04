using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using CustomEvent = ThGold.Event.CustomEvent;

public class launcher : MonoBehaviour {
    public GameObject EventSystem;

    private void Start() {
        LoadSingleton();
        LoadMenu();
    }


    void LoadSingleton() {
        GameManager gameManager = new GameObject("GameManager").AddComponent<GameManager>();
        ThGold.Event.EventHandler EventHandler = new GameObject("EventHandler").AddComponent<ThGold.Event.EventHandler>();
        GameObject inputManager = Instantiate(Resources.Load<GameObject>("Prefab/Object/InputManager"));
        SceneManager.sceneLoaded += UnloadFeel;
        //SceneManager.LoadScene("FeelCore", LoadSceneMode.Additive);
        DontDestroyOnLoad(gameManager);
        DontDestroyOnLoad(EventSystem);
        DontDestroyOnLoad(EventHandler);
        DontDestroyOnLoad(inputManager);
    }

    private void LoadMenu() {
        //加载核心,
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    private void UnloadFeel(Scene scene, LoadSceneMode arg1) {
        if (scene == SceneManager.GetSceneByName("FeelCore")) {
            SceneManager.UnloadSceneAsync("FeelCore");
        }

        //if (scene == SceneManager.GetSceneByName("ArticyTest"))
        //{
        //    ThGold.Event.EventHandler.Instance.EventDispatcher.DispatchEvent(CustomEvent.FirstLoadScene);
        //    string sceneName = "Launcher";
        //    Scene sceneToUnload = SceneManager.GetSceneByName(sceneName);
        //    if (sceneToUnload.IsValid())
        //    {
        //        SceneManager.UnloadSceneAsync(sceneName);
        //    }
        //}
    }
}