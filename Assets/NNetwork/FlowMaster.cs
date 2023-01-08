using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class FlowMaster : MonoBehaviour
{
    private Trainer _trainer;
    private SettingsConfig _settingsConfig;
    private IVerifier _verifier;
    private NeuralNetwork _neuralNetwork;

    private bool _doRun = false;

    [Inject]
    public void Construct(NeuralNetwork neuralNetwork,
        Trainer trainer,
        SettingsConfig settingsConfig,
        IVerifier verifier) {
        _neuralNetwork = neuralNetwork;
        _trainer = trainer;
        _settingsConfig = settingsConfig;
        _verifier = verifier;
    }

    
    void Start() {

        _verifier.Init();
        _neuralNetwork.Init(_verifier.GetInputSize, _verifier.GetOtputSize);
        _trainer.Init();

    }


    internal async void StartTrain() {
        if (_doRun) return;
        Debug.Log("Training Started");
        await Task.Yield();

        _doRun = true;
        while (_doRun) {
            await _trainer.TrainOneSupervisedRound();
            await ShowBestOutputAsync();
            await Task.Yield();
        }

        Debug.Log("Training Stopped");

    }

    internal void StopTrain() {
        _doRun = false;
    }

    internal void ShowCorrectOutput() {
        _verifier.VisualizeOutputReference();
    }

    internal async void ShowBestOutput() {
        await ShowBestOutputAsync();
    }

    private async Task ShowBestOutputAsync() {

        Model bestModel = _trainer.GetBestModel();
        _neuralNetwork.SetModel(bestModel);
        await _verifier.VisualizeSample();
    }

    private void OnDestroy() {
        _neuralNetwork.Dispose();
    }
}
