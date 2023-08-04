using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CamViewTransform))]
public class CamViewTransformEditor : Editor {
    CamViewTransform cvt;
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        // cvt = (CamViewTransform)target;
        // if(cvt.viewSpots == null) {
        //     cvt.viewSpots = new List<Matrix4x4>();
        // }
        // GUILayout.BeginHorizontal();
        // if(GUILayout.Button("Add ViewSpot")) {
        //     cvt.AddSpot();
        // }
        // if(GUILayout.Button("Del ViewSpot")) {
        //     cvt.RemoveSpot();
        // }
        // GUILayout.EndHorizontal();
        // GUILayout.BeginHorizontal();
        // if(GUILayout.Button("Check View")) {
        //     cvt.CheckSpot();
        // }
        // if(GUILayout.Button("Update View")) {
        //     cvt.UpdateSpot();
        // }
        // GUILayout.EndHorizontal();
        // //GUILayout.Label("Camera Spot: " + cvt.GetSpotIndex());
        // base.OnInspectorGUI();
        // GUILayout.Space(10);
        // GUILayout.Label("截图存储路径");
        // cvt.filePath = GUILayout.TextField(cvt.filePath);
        // GUILayout.Label("图片分辨率");
        // GUILayout.BeginHorizontal();
        // cvt.width = EditorGUILayout.IntField(cvt.width);
        // cvt.height = EditorGUILayout.IntField(cvt.height);
        // GUILayout.EndHorizontal();
        // if(GUILayout.Button("ScreenShot")){
        //     cvt.RenderATexture(cvt.filePath);
        // }
    }
}
