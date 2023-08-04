using UnityEngine;
using UnityEditor;

public class ObjectCounterWindow : EditorWindow
{
    [MenuItem("ArtTools/Object Counter")]
    public static void ShowWindow()
    {
        ObjectCounterWindow window = EditorWindow.GetWindow<ObjectCounterWindow>();
        window.titleContent = new GUIContent("Object Counter");
        window.Show();
    }

    void OnGUI()
    {
        // 获取当前场景中选中的对象数量
        int count = Selection.objects.Length;

        // 显示选中的对象数量
        EditorGUILayout.LabelField("选中的对象数量: " + count);
    }
}
