using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class FindObjectsByWorldPositionTool : EditorWindow
{
    private Dictionary<Vector3, List<GameObject>> objectsByPosition = new Dictionary<Vector3, List<GameObject>>();
    private string filterText = "";
    private Transform root;
    private Vector2 scrollPosition;

    [MenuItem("ArtTools/Find Objects by World Position Tool")]
    private static void OpenWindow()
    {
        FindObjectsByWorldPositionTool window = GetWindow<FindObjectsByWorldPositionTool>();
        window.titleContent = new GUIContent("Find Objects by World Position");
        window.Show();
    }

    private void OnGUI()
    {
        root = EditorGUILayout.ObjectField("Root", root, typeof(Transform), true) as Transform;

        if (GUILayout.Button("Find Objects by World Position"))
        {
            FindObjects();
        }

        EditorGUILayout.Space();

        filterText = EditorGUILayout.TextField("Filter by Name", filterText);

        if (objectsByPosition.Count > 0)
        {
            EditorGUILayout.LabelField("Objects by World Position:");
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            foreach (var kvp in objectsByPosition)
            {
                if (kvp.Value.Count >= 2)
                {
                    bool shouldDisplayGroup = string.IsNullOrEmpty(filterText);

                    if (!shouldDisplayGroup)
                    {
                        foreach (GameObject obj in kvp.Value)
                        {
                            if (obj.name.Contains(filterText))
                            {
                                shouldDisplayGroup = true;
                                break;
                            }
                        }
                    }

                    if (shouldDisplayGroup)
                    {
                        EditorGUILayout.BeginVertical(GUI.skin.box);

                        EditorGUILayout.LabelField("World Position: " + kvp.Key);

                        foreach (GameObject obj in kvp.Value)
                        {
                            if (string.IsNullOrEmpty(filterText) || obj.name.Contains(filterText))
                            {
                                if (GUILayout.Button(obj.name))
                                {
                                    Selection.activeObject = obj;
                                    EditorGUIUtility.PingObject(obj);
                                }
                            }
                        }

                        EditorGUILayout.EndVertical();
                    }
                }
            }

            EditorGUILayout.EndScrollView();
        }
    }

    private void FindObjects()
    {
        objectsByPosition.Clear();

        if (root == null)
        {
            Debug.Log("Root object is not specified.");
            return;
        }

        Transform[] allTransforms = root.GetComponentsInChildren<Transform>(true);
        GameObject[] allObjects = new GameObject[allTransforms.Length];

        for (int i = 0; i < allTransforms.Length; i++)
        {
            allObjects[i] = allTransforms[i].gameObject;
        }

        foreach (GameObject obj in allObjects)
        {
            Vector3 worldPosition = obj.transform.position;

            if (objectsByPosition.ContainsKey(worldPosition))
            {
                objectsByPosition[worldPosition].Add(obj);
            }
            else
            {
                List<GameObject> newList = new List<GameObject> { obj };
                objectsByPosition.Add(worldPosition, newList);
            }
        }

        // Remove positions with less than 2 objects
        var keysToRemove = new List<Vector3>();
        foreach (var kvp in objectsByPosition)
        {
            if (kvp.Value.Count < 2)
            {
                keysToRemove.Add(kvp.Key);
            }
        }
        foreach (var key in keysToRemove)
        {
            objectsByPosition.Remove(key);
        }

        Debug.Log("Found " + objectsByPosition.Count + " unique positions with multiple objects.");

        if (objectsByPosition.Count == 0)
        {
            Debug.Log("No objects found with the same world position.");
        }
    }
}
