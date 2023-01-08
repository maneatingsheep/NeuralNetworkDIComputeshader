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
        _neuralNetwork.InputLayer[0] = UnityEngine.Random.value;
        _neuralNetwork.InputLayer[1] = UnityEngine.Random.value;
        await _neuralNetwork.Run();
        float actual =_verifier.CalculateWantedRes(_neuralNetwork.InputLayer)[0];
        _neuralNetwork.OutputError[0] = actual - _neuralNetwork.OutputLayer[0];

        //calculate derivvatives to all but input
        for (int i = 1; i < _neuralNetwork.Layers.Length; i++) {
            ActivationDerivativeLayer(_neuralNetwork.Layers[i], _neuralNetwork.Derriivetives[i - 1]);
        }

        //set up output error signal
        for (int i = 0; i < _neuralNetwork.OutputError.Length; i++) {
            _neuralNetwork.OutputError[i] *= _neuralNetwork.Derriivetives[_neuralNetwork.Derriivetives.Length - 1][i];
        }

        //propogate error signals back to front
        for (int i = _neuralNetwork.Errors.Length - 1; i >= 1; i--) {
            PropagadeErrorSignal(bestModel.Weights[i], _neuralNetwork.Errors[i - 1], _neuralNetwork.Errors[i], _neuralNetwork.Derriivetives[i - 1]);
        }

        //adjust weights
        for (int i = 0; i < bestModel.Weights.Length; i++) {
            AdjustWeights(bestModel.Weights[i], _neuralNetwork.Errors[i], _neuralNetwork.Layers[i]);
        }

        
    }

    private void AdjustWeights(WeightMatrix weightMatrix, float[] outputErrorSignal, float[] inputLayer) {

        for (int i = 0; i < inputLayer.Length; i++) {
            for (int j = 0; j < outputErrorSignal.Length; j++) {

                float grad = inputLayer[i] * outputErrorSignal[j];


                float w = weightMatrix.GetValue(i, j);
                w += _settingsConfig.LearningRate * grad;
                weightMatrix.SetValue(w, i, j);
            }
        }

        for (int i = 0; i < outputErrorSignal.Length; i++) {
            float biasGrad = outputErrorSignal[i];
            weightMatrix.Biases[i] += _settingsConfig.LearningRate * biasGrad;

        }
    }

    private void PropagadeErrorSignal(WeightMatrix matrix, float[] inputError, float[] outputError, float[] derrivetives) {

        for (int i = 0; i < inputError.Length; i++) {
            inputError[i] = 0;

            for (int j = 0; j < outputError.Length; j++) {
                inputError[i] += outputError[j] * matrix.GetValue(i, j);
            }

            inputError[i] *= derrivetives[i];
        }
    }

    private void ActivationDerivativeLayer(float[] layer, float[] derrivetives) {
        for (int i = 0; i < layer.Length; i++) {
            derrivetives[i] = MathFunctions.ActivationFuncDerivative(layer[i], _settingsConfig.ActivationType);
        }
    }

}