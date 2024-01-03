using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityToolbarExtender;

public static class WwiseEditor {
    private static string WwisePath = Application.streamingAssetsPath + "/Audio/GeneratedSoundBanks/Windows/SoundbanksInfo.xml";

    [MenuItem("CustomEditor/Wwise/CreateWwiseEvent")]
    static void ToEventName() {
        CreateWwiseEvent();
    }

    private static void CreateWwiseEvent() {
    }
}