using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;
using System;
using System.IO;
using System.Collections;

public class WindowsManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        float aspectRatio = 16f/9f;
        float height = 0.5f;
        float width = height * aspectRatio;
        WindowController whiteBoard = WindowController.New2DWindow(width, height);

        //WindowController clock = Instantiate(Resources.Load<WindowController>("Prefabs/Clock"));

        //WindowController chikorita = WindowController.New3DWindow();
        //AssetBundle chikoritaAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "chikorita"));
        //chikorita.SetModel(chikoritaAssetBundle);

        //WindowController table = WindowController.New3DWindow();
        //AssetBundle tableAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "table"));
        //table.SetModel(tableAssetBundle);
    }

    // Update is called once per frame
    void Update () {

	}
}
