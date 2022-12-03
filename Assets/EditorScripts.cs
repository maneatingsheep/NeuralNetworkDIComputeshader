using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zenject;

public class EditorScripts : MonoBehaviour
{
    private InputDataManager _inputDataManager;
    private InputDisplayView _inputDisplayView;


    private static EditorScripts _instance;

    private void Awake() {
        _instance = this;
    }

    [Inject]
    public void Construct(InputDataManager inputDataManager, InputDisplayView inputDisplayView) {
        _inputDataManager = inputDataManager;
        _inputDisplayView = inputDisplayView;
    }


    [MenuItem("Cheats/Show Image")]
    static void CheatLoadData() {
        if (!_instance._inputDataManager.IsReady) {
            Debug.LogError("Data Not Ready");
            return;
        }
        _instance._inputDisplayView.ShowImage(_instance._inputDataManager.GetRandomImage(true));
    }
}
