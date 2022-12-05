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
    private ModelManager _modelManager;
    private static EditorScripts _instance;

    private void Awake() {
        _instance = this;
    }

    [Inject]
    public void Construct(InputDataManager inputDataManager, 
        InputDisplayView inputDisplayView, 
        NetworkOutputDisaply networkOutputDisaply, 
        NeuralNetwork neuralNetwork,
        Trainer trainer,
        ModelManager modelManager) {
        _inputDataManager = inputDataManager;
        _inputDisplayView = inputDisplayView;
        _neuralNetwork = neuralNetwork;
        _networkOutputDisaply = networkOutputDisaply;
        _trainer = trainer;
        _modelManager = modelManager;
    }


    [MenuItem("Cheats/Show Image")]
    static void ShowImage() {
        if (!_instance._inputDataManager.IsReady) {
            Debug.LogError("Data Not Ready");
            return;
        }

        var data = _instance._inputDataManager.GetRandomImage(true);

        _instance._inputDisplayView.ShowImage(data);
        _instance._neuralNetwork.SetModel(_instance._trainer.GetBestModel());
        _instance._neuralNetwork.SetInput(data.Data);
        int result = _instance._neuralNetwork.Run();
        _instance._networkOutputDisaply.ShowOutput(result);
        

    }

    [MenuItem("Cheats/Train")]
    static void Train() {
        _instance._trainer.Train();
    }

    [MenuItem("Cheats/Stop Training")]
    static void StopTrainig() {
        _instance._trainer.StopTraining();
    }
}
