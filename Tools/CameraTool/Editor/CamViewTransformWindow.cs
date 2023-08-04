using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CamViewTransformWindow : EditorWindow {
    [MenuItem("ArtTools/CamViewTool")]
    static void Init() {
        CamViewTransformWindow window = (CamViewTransformWindow)EditorWindow.GetWindow(typeof(CamViewTransformWindow));
        editorwindow = EditorWindow.GetWindow(typeof(CamViewTransformWindow));
        window.Show();
    }

    static EditorWindow editorwindow;
    public Camera activeCam = null;
    public string[] resolutions = new string[] { "1K (1920*1080)", "2K (2560*1440)", "4K (3840*2160)", "8K (7680*4320)", "CUSTOM" };
    public int resolutionIndex = 0;
    public string filePath = "Assets/Temp/CamviewScreenShots/";
    Vector2 scrollPos;
    Vector2 scrollPos1;
    bool autoRender = true;

    void OnGUI() {
        Repaint();
        if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Camera>() != null) {
            activeCam = Selection.activeGameObject.GetComponent<Camera>();
        }
        if (activeCam != null) {
            if (activeCam.GetComponent<CamViewTransform>() == null) {
                if (GUILayout.Button("添加脚本并开始")) {
                    activeCam.gameObject.AddComponent<CamViewTransform>();
                }
            } else {
                scrollPos1 = EditorGUILayout.BeginScrollView(scrollPos1);
                CamViewTransform cvt = activeCam.GetComponent<CamViewTransform>();
                if (cvt.viewSpots == null) {
                    cvt.viewSpots = new List<Matrix4x4>();
                }

                cvt.CheckSameName();

                // GUIStyle style = new GUIStyle();
                // style.
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("截图存储路径");
                filePath = GUILayout.TextField(filePath);
                GUILayout.Label("图片分辨率");
                GUILayout.BeginHorizontal();
                resolutionIndex = EditorGUILayout.Popup(resolutionIndex, resolutions, GUILayout.Width(120));
                if (resolutionIndex == 4) {
                    cvt.width = EditorGUILayout.IntField(cvt.width);
                    cvt.height = EditorGUILayout.IntField(cvt.height);
                } else {
                    switch (resolutionIndex) {
                        case 0:
                            cvt.width = 1920;
                            cvt.height = 1080;
                            break;
                        case 1:
                            cvt.width = 2560;
                            cvt.height = 1440;
                            break;
                        case 2:
                            cvt.width = 3840;
                            cvt.height = 2160;
                            break;
                        case 3:
                            cvt.width = 7680;
                            cvt.height = 4320;
                            break;
                    }
                }
                GUILayout.EndHorizontal();
                if (GUILayout.Button("ScreenShot", GUILayout.Height(50))) {
                    cvt.RenderATexture(filePath);
                    cvt.RenderATexture(filePath);
                }
                EditorGUILayout.Space(5);
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(20);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.BeginHorizontal();
                GUILayout.Label("ActiveCamera:");
                activeCam = (Camera)EditorGUILayout.ObjectField(activeCam, typeof(Camera));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Add ViewSpot")) {
                    cvt.AddSpot();
                    if (autoRender == true) {
                        cvt.RenderATexture(filePath);
                        cvt.RenderATexture(filePath);
                    }
                }
                if (GUILayout.Button("Del ViewSpot")) {
                    cvt.RemoveSpot();
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Check View")) {
                    cvt.CheckSpot();
                }
                if (GUILayout.Button("Update View")) {
                    cvt.UpdateSpot();
                    if (autoRender == true) {
                        cvt.RenderATexture(filePath);
                        cvt.RenderATexture(filePath);
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("AtuoRender", GUILayout.Width(70));
                autoRender = EditorGUILayout.Toggle(autoRender);
                GUILayout.EndHorizontal();

                int rowcount = (int)Mathf.Floor(editorwindow.position.width / 100);
                // scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(140));

                // cvt.viewSpotIndex = GUILayout.SelectionGrid(cvt.viewSpotIndex, cvt.names.ToArray(), 5);

                GUIStyle buttonStyle = new GUIStyle();
                buttonStyle.alignment = TextAnchor.MiddleLeft;
                buttonStyle.stretchWidth = false;
                EditorStyles.helpBox.alignment = TextAnchor.MiddleLeft;

                EditorStyles.helpBox.margin.left = 0;
                GUI.skin.button.onActive.textColor = Color.gray;

                for (int j = 0; j <= Mathf.Ceil(cvt.viewSpots.Count / rowcount); j++) {
                    EditorStyles.helpBox.alignment = TextAnchor.MiddleLeft;
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    // CamViewTransform camComnent = activeCam.GetComponent<CamViewTransform>();
                    for (int i = 0; i < rowcount; i++) {
                        if (i + j * rowcount >= cvt.viewSpots.Count) {
                            break;
                        }
                        Texture buttonTex = AssetDatabase.LoadAssetAtPath<Texture>(AssetDatabase.GUIDToAssetPath(cvt.guids[i + j * rowcount]));
                        EditorGUILayout.BeginVertical(GUILayout.Width(100));

                        Color bc = GUI.backgroundColor;
                        if (cvt.viewSpotIndex == i + j * rowcount) {
                            GUI.backgroundColor = Color.gray;
                        }
                        // GUI.skin.button.margin.left = 50;
                        EditorStyles.helpBox.padding.left = 0;

                        if (GUILayout.Button(buttonTex, GUILayout.Width(100), GUILayout.Height(100))) {
                            cvt.viewSpotIndex = i + j * rowcount;
                            cvt.TransformCamera(cvt.viewSpotIndex);
                        }

                        GUI.backgroundColor = bc;

                        cvt.names[i + j * rowcount] = EditorGUILayout.TextField(cvt.names[i + j * rowcount], GUILayout.Width(100));
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                // EditorGUILayout.EndScrollView();

                // EditorGUILayout.BeginHorizontal();
                // CamViewTransform camComnent = activeCam.GetComponent<CamViewTransform>();
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                if (cvt.guids.Count != 0) {
                    if (AssetDatabase.LoadAssetAtPath<Texture>(AssetDatabase.GUIDToAssetPath(cvt.guids[cvt.viewSpotIndex])) != null) {
                        GUILayout.Box(AssetDatabase.LoadAssetAtPath<Texture>(AssetDatabase.GUIDToAssetPath(cvt.guids[cvt.viewSpotIndex])), GUILayout.Height(500), GUILayout.Width(editorwindow.position.width - 50));
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
                if (GUILayout.Button("ClearAllMono", GUILayout.Height(50))) {
                    CamViewTransform[] allcammono = GameObject.FindObjectsOfType<CamViewTransform>();
                    for (int i = 0; i < allcammono.Length; i++) {
                        DestroyImmediate(allcammono[i]);
                    }
                }
                EditorGUILayout.EndScrollView();
            }
        }
    }
}
