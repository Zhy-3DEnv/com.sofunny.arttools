using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SaveMesh : EditorWindow {
    [MenuItem("美术工具/Mesh 储存")]
    private static void ShowWindow() {
        var window = GetWindow<SaveMesh>();
        window.titleContent = new GUIContent("Mesh工具");
        window.Show();
    }

    private GameObject tarGo;
    private List<GameObject> meshs = new List<GameObject>();
    private SaveType type;

    private void OnGUI() {
        GUILayout.Label("保存类型");
        type = (SaveType)EditorGUILayout.EnumPopup(type);
        EditorGUILayout.Space(5);
        if (GUILayout.Button("Save Mesh")) {
            OnSaveMesh();
        }

        EditorGUILayout.Space(2);
        ShowSaveMeshName();
        EditorGUILayout.Space();
    }

    public enum SaveType {
        Mesh,
        Asset
    }

    private void ShowSaveMeshName() {
        meshs.Clear();
        if (Selection.gameObjects.Length > 0) {
            foreach (var m in Selection.gameObjects) {
                tarGo = (GameObject)EditorGUILayout.ObjectField(m, typeof(Mesh), true);
                meshs.Add(tarGo);
            }
        }
    }

    private void OnSaveMesh() {
        if (meshs.Count > 0) {
            var path = GetFinalPath("");
            string mtype = "mesh";
            switch (type) {
                case SaveType.Mesh:
                    mtype = "mesh";
                    break;
                case SaveType.Asset:
                    mtype = "asset";
                    break;
            }

            foreach (var m in meshs) {
                var tarMeshFilter = m.GetComponent<MeshFilter>();
                var tarRenderer = m.GetComponent<SkinnedMeshRenderer>();
                if (tarMeshFilter) {
                    var savePath = $"{path}/{tarMeshFilter.sharedMesh.name}.{mtype}";
                    AssetDatabase.CreateAsset(Instantiate(tarMeshFilter.sharedMesh), savePath);
                }

                if (tarRenderer) {
                    var savePath = $"{path}/{tarRenderer.sharedMesh.name}.{mtype}";
                    AssetDatabase.CreateAsset(Instantiate(tarRenderer.sharedMesh), savePath);
                }
            }
        }
    }

    private string GetFinalPath(string meshName) {
        var path = EditorUtility.SaveFolderPanel("Save Separate Mesh Asset", "Assets/", meshName);
        path = FileUtil.GetProjectRelativePath(path);
        return path;
    }
}