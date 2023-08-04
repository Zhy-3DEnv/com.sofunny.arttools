using UnityEngine;
using UnityEditor;

public class DeleteGameObjectsByNameTool : EditorWindow
{
    private string objectNameToDelete;

    [MenuItem("ArtTools/Delete Game Objects by Name")]
    private static void OpenWindow()
    {
        DeleteGameObjectsByNameTool window = GetWindow<DeleteGameObjectsByNameTool>();
        window.titleContent = new GUIContent("Delete Game Objects by Name");
        window.Show();
    }

    private void OnGUI()
    {
        objectNameToDelete = EditorGUILayout.TextField("Object Name", objectNameToDelete);

        if (GUILayout.Button("Delete"))
        {
            DeleteGameObjects();
        }
    }

    private void DeleteGameObjects()
    {
        GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject rootObject in rootObjects)
        {
            FindAndDeleteObjects(rootObject.transform);
        }

        Debug.Log($"Deleted game objects with name '{objectNameToDelete}' from Hierarchy.");
    }

    private void FindAndDeleteObjects(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);

            if (child.name == objectNameToDelete)
            {
                // 解包 Prefab 实例
                GameObject prefabInstance = PrefabUtility.GetOutermostPrefabInstanceRoot(child.gameObject);
                PrefabUtility.UnpackPrefabInstance(prefabInstance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

                // 删除子对象
                GameObject.DestroyImmediate(child.gameObject);
            }
            else
            {
                // 递归查找子对象
                FindAndDeleteObjects(child);
            }
        }
    }
}
