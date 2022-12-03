using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zenject;

public class EditorScripts : MonoBehaviour
{
    private InputDataManager _inputDataManager;

    private static EditorScripts _instance;

    private void Awake() {
        _instance = this;
    }

    [Inject]
    public void Construct(InputDataManager inputDataManager) {
        print(inputDataManager);
        _inputDataManager = inputDataManager;
    }

    void LoadData() {
        _inputDataManager.LoadData();
    }

    [MenuItem("Cheats/Load Data")]
    static void CheatLoadData() {
        _instance.LoadData();
    }
}
