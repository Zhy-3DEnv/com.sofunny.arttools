using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public class CamViewTransform : MonoBehaviour {
    [Range(1.0f, 300.0f)]
    public float cameraTime = 60.0f;
    public List<Matrix4x4> viewSpots;
    public int viewSpotIndex = 0;
    // Start is called before the first frame update
    void Start() {
    }
    // Update is called once per frame
    void Update() {
        ActivateCamera();
    }
    public void AddSpot() {
        if (IsCameraHasNewTransform(transform.localToWorldMatrix)) {
            viewSpots.Add(transform.localToWorldMatrix);
            guids.Add("");
            names.Add(gameObject.name + (viewSpots.Count - 1).ToString());
            if (viewSpots.Count == 0) {
                viewSpotIndex = 0;
            } else {
                viewSpotIndex = viewSpots.Count - 1;
            }
        } else {
            Debug.Log("Camera transform has no change.");
        }
    }

    public void CheckSameName() {
        for (int i = 0; i < names.Count; i++) {
            for (int j = 0; j < names.Count; j++) {
                if (names[j] == names[i] && i != j) {
                    Debug.LogError("存在相同的机位命名，请及时检查并修改");
            }
            }
        }
    }

    private bool IsCameraHasNewTransform(Matrix4x4 tranformMatrix) {
        bool status = true;
        foreach (Matrix4x4 mat in viewSpots) {
            if (mat == transform.localToWorldMatrix) {
                status = false;
                break;
            }
        }
        return status;
    }
    public void RemoveSpot() {
        // if (viewSpots.Count > 2) {
        //     viewSpots.RemoveAt(viewSpotIndex);
        //     guids.RemoveAt(viewSpotIndex);
        //     names.RemoveAt(viewSpotIndex);
        // }
        // if (viewSpots.Count == 2) {
        //     viewSpots.RemoveAt(viewSpotIndex);
        //     guids.RemoveAt(viewSpotIndex);
        //     names.RemoveAt(viewSpotIndex);
        //     viewSpotIndex = 0;
        // }
        // if(viewSpots.Count == 1){
        //     viewSpots.RemoveAt(viewSpotIndex);
        //     guids.RemoveAt(viewSpotIndex);
        //     names.RemoveAt(viewSpotIndex);
        //     viewSpotIndex = 0;
        // }
        viewSpots.RemoveAt(viewSpotIndex);
        guids.RemoveAt(viewSpotIndex);
        names.RemoveAt(viewSpotIndex);
        viewSpotIndex = 0;
    }

    public void CheckSpot() {

        if (viewSpots == null || viewSpots.Count == 0) {
            Debug.Log(" Camera is not ready ");
            return;
        }
        transform.position = viewSpots[viewSpotIndex].GetColumn(3);
        //Debug.Log(viewSpots[viewSpotIndex].rotation.eulerAngles);
        Quaternion targetRotation = viewSpots[viewSpotIndex].rotation;
        transform.rotation = targetRotation;
        viewSpotIndex++;
        if (viewSpotIndex > viewSpots.Count - 1) {
            viewSpotIndex = 0;
        }
    }
    public int GetSpotIndex() {
        return viewSpotIndex;
    }
    private void OnGUI() {
        GUILayout.Label("Camera Spot: " + viewSpotIndex);
    }
    private void ActivateCamera() {
        if (Input.GetMouseButtonDown(0)) {
            // StartCoroutine(TransformCamera(viewSpotIndex));
            TransformCamera(viewSpotIndex);
            viewSpotIndex++;
            if (viewSpotIndex > viewSpots.Count - 1) {
                viewSpotIndex = 0;
            }
        }
    }

    public void UpdateSpot() {
        if (viewSpots == null || viewSpots.Count == 0) {
            Debug.Log(" Camera is not ready ");
            return;
        }

        // int updateIndex = 0;
        // if(viewSpotIndex > 0) {
        //     updateIndex = viewSpotIndex - 1;
        // } else if(viewSpotIndex == 0) {
        //     updateIndex = viewSpots.Count - 1;
        // }

        Debug.Log(" View Spot: " + viewSpotIndex + " is updated. ");
        viewSpots[viewSpotIndex] = transform.localToWorldMatrix;
    }


    public void TransformCamera(int index) {
        Vector3 targetPos = viewSpots[index].GetColumn(3);
        // Vector3 moveStep = (targetPos - Camera.main.transform.position) / cameraTime;

        Vector3 targetEuler = viewSpots[index].rotation.eulerAngles;
        //Camera.main.transform.eulerAngles = targetEuler;
        // Quaternion rotationFrom = transform.rotation;

        // float rotateStep = 1.0f / cameraTime;
        // float j = rotateStep;
        //Debug.Log(targetPos);
        // for(int i = 0; i < cameraTime; i++, j += rotateStep) {
        // yield return new WaitForSeconds(0.01f);
        // GetComponent<Camera>().transform.position += moveStep;
        // GetComponent<Camera>().transform.rotation = Quaternion.Lerp(rotationFrom, Quaternion.Euler(targetEuler), j);
        // }
        GetComponent<Camera>().transform.position = targetPos;
        GetComponent<Camera>().transform.rotation = Quaternion.Euler(targetEuler);
    }

    [HideInInspector]
    public int width = 1920;
    [HideInInspector]
    public int height = 1080;
    [HideInInspector]
    public List<string> names = new List<string>();
    public List<string> guids = new List<string>();
    public void RenderATexture(string path) {
        Camera camera = GetComponent<Camera>();
        var hdr = camera.allowHDR && PlayerSettings.colorSpace == ColorSpace.Linear;
        bool transparency = camera.clearFlags == CameraClearFlags.Depth;

        RenderTexture rt = new RenderTexture(width, height, 16, hdr ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default, RenderTextureReadWrite.sRGB);
        Texture2D screenShot = new Texture2D(width, height, transparency ? TextureFormat.ARGB32 : TextureFormat.RGB24, false, false);
        RenderTexture.active = rt;
        camera.targetTexture = rt;
        camera.Render();
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);

        if(hdr){
            Color[] pixels = screenShot.GetPixels();
            for (int p = 0; p < pixels.Length; p++)
            {
                pixels[p] = pixels[p].gamma;
            }
            screenShot.SetPixels(pixels);

            screenShot.Apply();
        }

        camera.targetTexture = null;
        RenderTexture.active = null;
        rt.Release();

        byte[] bytes = transparency ? screenShot.EncodeToPNG() : screenShot.EncodeToJPG();
        string type = transparency ? ".png" : ".jpg";

        // CheckPictureExistence("Cam" + viewSpotIndex + "_", 0, bytes);
        Directory.CreateDirectory(path);
        File.WriteAllBytes(path + SceneManager.GetActiveScene().name + " " + names[viewSpotIndex] + type, bytes);
        guids[viewSpotIndex] = AssetDatabase.AssetPathToGUID(path + SceneManager.GetActiveScene().name + " " + names[viewSpotIndex] + type);
        AssetDatabase.Refresh();
    }

    // private void CheckPictureExistence(string path, int id, byte[] bytes){
    //     if(AssetDatabase.FindAssets(path + id.ToString()).Length == 0){
    //         File.WriteAllBytes(filePath + path + id + ".PNG", bytes);
    //         AssetDatabase.Refresh();
    //         Debug.Log("output" + "    "+ filePath + path + id + ".PNG");
    //     }else{
    //         id += 1;
    //         CheckPictureExistence(path, id, bytes);
    //     }
    // }
}
