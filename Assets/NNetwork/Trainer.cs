using System;
using System.Threading.Tasks;
using UnityEngine;

public class Trainer {

    private int _currentGen = 0;

    private ModelManager _modelManager;
    private NeuralNetwork _neuralNetwork;
    private SettingsConfig _settingsConfig;
    private IVerifier _verifier;

    private bool _initialized;
    private int _bestModelIndex;

    public Trainer(ModelManager modelManager, NeuralNetwork neuralNetwork, SettingsConfig settingsConfig, IVerifier verifier) {
        _modelManager = modelManager;
        _neuralNetwork = neuralNetwork;
        _settingsConfig = settingsConfig;
        _verifier = verifier;
    }

    public void Init() {

        _modelManager.Setup(_settingsConfig.GenerationSize, _neuralNetwork);
        _initialized = true;
        _currentGen = 0;
        Debug.Log("network setup complete");

    }

    public async Task TrainOneGen() {
        if (!_initialized) {
            Debug.Log("not initialized");
            return;
        }
        if (_currentGen != 0) {
            _modelManager.BreedNextGen();
        }

        await RunAllNetworks();

        _verifier.SetFitness(_modelManager.TrainModels);

        AnalyzeResults();
        _currentGen++;

    }

    private async Task RunAllNetworks() {
        for (int m = 0; m < _modelManager.TrainModels.Length; m++) {
            _modelManager.TrainModels[m].Reset();
        }

        for (int i = 0; i < _settingsConfig.NumberOfAttemptsPerTrain; i++) {

            var inputs = _verifier.SetNewVerefication(true);
            _neuralNetwork.SetInput(inputs);

            for (int m = 0; m < _modelManager.TrainModels.Length; m++) {

                _neuralNetwork.SetModel(_modelManager.TrainModels[m]);
                await _neuralNetwork.Run();

                float score = _verifier.Verify(_neuralNetwork.OutputLayer._neurons, false);

                _modelManager.TrainModels[m].Score += score;


            }

        }
    }

    private void AnalyzeResults() {
        _bestModelIndex = -1;
        float avgScore = 0f;

        for (int i = 0; i < _modelManager.TrainModels.Length; i++) {
            avgScore += _modelManager.TrainModels[i].Score;
            if (_bestModelIndex == -1 || _modelManager.TrainModels[i].Score > _modelManager.TrainModels[_bestModelIndex].Score) {
                _bestModelIndex = i;
            }
        }

        avgScore /= _modelManager.TrainModels.Length;

        Debug.Log($"completed gen {_currentGen} Best score: {_modelManager.TrainModels[_bestModelIndex].Score} Avg score: {avgScore}");

    }

    internal Model GetBestModel() {
        return _modelManager.TrainModels[_bestModelIndex];
    }

}