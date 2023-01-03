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
    private float[] _fitness;
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
        _fitness = new float[_modelManager.TrainModels.Length];
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

        _modelManager.ResetScores();

        await RunAllModels();

        _verifier.SetFitness(_modelManager.TrainModels);
        DistributrFittness();

        AnalyzeResults();
        _currentGen++;

    }

    private async Task RunAllModels() {
        
        _verifier.SetNewVerefication(true);

        for (int m = 0; m < _modelManager.TrainModels.Length; m++) {
            _neuralNetwork.SetModel(_modelManager.TrainModels[m]);
            float score = await _verifier.Verify();
            _modelManager.TrainModels[m].Score = score;
        }
    }

    private void DistributrFittness() {

        for (int i = 0; i < _modelManager.TrainModels.Length; i++) {
            _fitness[i] = _modelManager.TrainModels[i].Fitness;
        }

        MathFunctions.NormlizeVector(_fitness);

        float totalFitt = 0;
        for (int i = 0; i < _fitness.Length; i++) {
            totalFitt += _fitness[i];
        }

        if (totalFitt == 0) {
            for (int i = 0; i < _modelManager.TrainModels.Length; i++) {
                _modelManager.TrainModels[i].Fitness = 1f;
            }

            return;
        }

        for (int i = 0; i < _modelManager.TrainModels.Length; i++) {
            _modelManager.TrainModels[i].Fitness = _fitness[i] / totalFitt;
        }

    }

    private void AnalyzeResults() {
        _bestModelIndex = -1;
        int worstModelIndex = -1;
        float avgScore = 0f;

        for (int i = 0; i < _modelManager.TrainModels.Length; i++) {
            avgScore += _modelManager.TrainModels[i].Score;
            if (_bestModelIndex == -1 || _modelManager.TrainModels[i].Score > _modelManager.TrainModels[_bestModelIndex].Score) {
                _bestModelIndex = i;
            }
            if (worstModelIndex == -1 || _modelManager.TrainModels[i].Score < _modelManager.TrainModels[worstModelIndex].Score) {
                worstModelIndex = i;
            }
        }

        avgScore /= _modelManager.TrainModels.Length;

        Debug.Log($"GEN: {_currentGen}. SCORES: {_modelManager.TrainModels[_bestModelIndex].Score}-{_modelManager.TrainModels[worstModelIndex].Score}. AVG: {avgScore}");

    }

    internal Model GetBestModel() {
        return _modelManager.TrainModels[_bestModelIndex];
    }

}