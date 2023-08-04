using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Text;
using System.IO;

public class CheckPrefabPos : EditorWindow{

    [MenuItem("Tool/Check Same Pos")]
    static void Window(){
        EditorWindow.GetWindow(typeof(CheckPrefabPos)).Show();
    }

    private void OnGUI(){
        if (GUILayout.Button("Check")){
            CheckSamePos();          
        }
    }

    private List<Renderer> renders;
    private Dictionary<Vector3, List<Renderer>> objPosDic;
    void CheckSamePos(){
        renders = new List<Renderer>();
        var roots = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var root in roots){
            renders.AddRange(root.GetComponentsInChildren<Renderer>());
        }

        objPosDic = new Dictionary<Vector3, List<Renderer>>();
        foreach (var r in renders){
            var p = r.GetComponent<Transform>().position;
            if (objPosDic.ContainsKey(p)){
                if (objPosDic.TryGetValue(p, out List<Renderer> rs)){
                    rs.Add(r);
                }               
            }else{
                List<Renderer> rs = new List<Renderer>();
                rs.Add(r);
                objPosDic.Add(p, rs);
            }
            
        }

        var result = new StringBuilder();
        foreach (var posDic in objPosDic){
            if (posDic.Value.Count > 1){
                OutputResult(posDic, result);
            }
        }
        File.WriteAllText("D:\\CheckPos.txt", result.ToString(), Encoding.UTF8);
    }
    void OutputResult(KeyValuePair<Vector3, List<Renderer>> posDic, StringBuilder result){
        result.Append($"{posDic.Key}\n");
        for (int i = 0; i < posDic.Value.Count; i++){
            result.Append($"{posDic.Value[i].name}\n");
        }
        result.Append($"\n");
    }
}


