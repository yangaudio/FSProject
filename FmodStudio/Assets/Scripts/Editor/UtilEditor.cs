using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityToolbarExtender;

public class UtilEditor {
    [InitializeOnLoad]
    public class SceneSwitchLeftButton {
        static SceneSwitchLeftButton() {
            ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
        }

        public class EditorSceneChoice : OdinMenuEditorWindow {
            private string lastPath;

            public static void OpenWindow() {
                var windows = GetWindow<EditorSceneChoice>();
                windows.titleContent = new GUIContent("选择场景");
                windows.minSize = new Vector2(800, 800);
                windows.maxSize = new Vector2(800, 800);
                windows.maximized = false;
                windows.ResizableMenuWidth = false;
                windows.Show();
            }

            protected override OdinMenuTree BuildMenuTree() {
                var tree = new OdinMenuTree(false);
                var guids = AssetDatabase.FindAssets("t:scene");

                foreach (var guid in guids) {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    tree.Add(path, path);
                }

                tree.EnumerateTree().AddThumbnailIcons();
                tree.Config.DrawSearchToolbar = true;
                MenuWidth = 800;
                return tree;
            }

            protected override void OnGUI() {
                base.OnGUI();
                if (MenuTree?.Selection.SelectedValue == null) return;
                var selectionPath = MenuTree.Selection.SelectedValue.ToString();
                if (string.IsNullOrEmpty(lastPath)) {
                    lastPath = selectionPath;
                    return;
                }

                if (lastPath != selectionPath) {
                    lastPath = selectionPath;
                    EditorSceneManager.OpenScene(selectionPath);
                    Close();
                }
            }

            public void OnFocus() {
                ForceMenuTreeRebuild();
            }

        }
        static void OnToolbarGUI() {
            var style = new GUIStyle(EditorStyles.toolbarButton);
            style.fixedWidth = 35;
            if (GUILayout.Button(EditorGUIUtility.ObjectContent(null, typeof(SceneAsset)).image, style)) {
                EditorSceneChoice.OpenWindow();
            }

            style.fontStyle = FontStyle.Bold;
            GUILayout.FlexibleSpace();
        }
        
    }

}
