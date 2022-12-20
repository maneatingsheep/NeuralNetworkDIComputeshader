using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zenject;

public class EditorScripts : MonoBehaviour
{
    private static EditorScripts _instance;

    private void Awake() {
        _instance = this;
    }

    [Inject]
    public void Construct() { 

    }

}
