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
            float score = await _verifier.EvaluateModel();
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


    internal async Task TrainOneSupervisedRound() {
        _bestModelIndex = 0;
        Model bestModel = _modelManager.TrainModels[_bestModelIndex];
        _neuralNetwork.SetModel(bestModel);
        _neuralNetwork.InputLayer[0] = Mathf.RoundToInt(UnityEngine.Random.value);
        _neuralNetwork.InputLayer[1] = Mathf.RoundToInt(UnityEngine.Random.value);
        await _neuralNetwork.Run();
        float actual =_verifier.CalculateWantedRes(_neuralNetwork.InputLayer)[0];
        _neuralNetwork.OutputError[0] = _neuralNetwork.OutputLayer[0] - actual;
        ActivationDerivativeLayer(_neuralNetwork.OutputError, _neuralNetwork.OutputLayer);

        for (int i = _neuralNetwork.Errors.Length - 1; i >= 1; i--) {
            PropagadeError(bestModel.Weights[i], _neuralNetwork.Errors[i - 1], _neuralNetwork.Errors[i]);
            ActivationDerivativeLayer(_neuralNetwork.Errors[i - 1], _neuralNetwork.Layers[i]);
        }

        for (int i = 0; i < bestModel.Weights.Length; i++) {
            
            for (int j = 0; j < _neuralNetwork.Layers[i].Length; j++) {
                
                for (int k = 0; k < _neuralNetwork.Layers[i + 1].Length; k++) {
                    float w = bestModel.Weights[i].GetValue(j, k);
                    w = w - _settingsConfig.LearningRate * _neuralNetwork.Errors[i][k] * _neuralNetwork.Layers[i][j];
                    bestModel.Weights[i].SetValue(w, j, k);
                }
            }
        }
    }

    private void PropagadeError(WeightMatrix matrix, float[] inputError, float[] outputError) {
        for (int i = 0; i < inputError.Length; i++) {
            inputError[i] = 0;

            for (int j = 0; j < outputError.Length; j++) {
                inputError[i] += outputError[j] * matrix.GetValue(i, j);
            }
        }
    }

    private void ActivationDerivativeLayer(float[] error, float[] layer) {
        for (int i = 0; i < error.Length; i++) {
            error[i] = error[i] * MathFunctions.ActivationFuncDerivative(layer[i], _settingsConfig.ActivationType);
        }
    }

}