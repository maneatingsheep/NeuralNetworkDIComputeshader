using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zenject;

public class EditorScripts : MonoBehaviour
{
    private InputDataManager _inputDataManager;
    private InputDisplayView _inputDisplayView;
    private NeuralNetwork _neuralNetwork;
    private NetworkOutputDisaply _networkOutputDisaply;
    private Trainer _trainer;
    private static EditorScripts _instance;

    private void Awake() {
        _instance = this;
    }

    [Inject]
    public void Construct(InputDataManager inputDataManager, 
        InputDisplayView inputDisplayView, 
        NetworkOutputDisaply networkOutputDisaply, 
        NeuralNetwork neuralNetwork,
        Trainer trainer) {
        _inputDataManager = inputDataManager;
        _inputDisplayView = inputDisplayView;
        _neuralNetwork = neuralNetwork;
        _networkOutputDisaply = networkOutputDisaply;
        _trainer = trainer;

    }

}
