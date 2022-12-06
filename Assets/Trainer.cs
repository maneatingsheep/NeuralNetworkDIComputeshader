using System;
using System.Threading.Tasks;
using UnityEngine;

public class Trainer {

    private const int SIZE_OF_GEN = 100;
    private const int NUM_OF_ATTEMPTS = 50; //consider increase for high scores

    private int _currentGen = 0;

    private ModelManager _modelManager;
    private InputDataManager _inputDataManager;
    private NeuralNetwork _neuralNetwork;

    private bool _halt;
    private int _bestModelIndex;

    public Trainer(ModelManager modelManager, InputDataManager inputDataManager, NeuralNetwork neuralNetwork) {
        _modelManager = modelManager;
        _inputDataManager = inputDataManager;
        _neuralNetwork = neuralNetwork;
    }

    internal async void Train() {

        _halt = false;
        _modelManager.Setup(SIZE_OF_GEN, _neuralNetwork);

        await StartTraining();
    }

    private async Task StartTraining() {
        while (!_halt) {
            if (_currentGen != 0) {
                _modelManager.BreedNextGen();
            }

            DataImage[] images = new DataImage[NUM_OF_ATTEMPTS];

            for (int i = 0; i < NUM_OF_ATTEMPTS; i++) {
                images[i] = _inputDataManager.GetRandomImage(true);
            }

            _bestModelIndex = -1;
            float avgScore = 0f;

            for (int m = 0; m < _modelManager.TrainModels.Length; m++) {
                _neuralNetwork.SetModel(_modelManager.TrainModels[m]);
                float score = PlayGame(ref images);
                
                _modelManager.TrainModels[m].Score = score;

                avgScore += _modelManager.TrainModels[m].Score;
                if (_bestModelIndex == -1 || _modelManager.TrainModels[m].Score > _modelManager.TrainModels[_bestModelIndex].Score) {
                    _bestModelIndex = m;
                }
            }

            avgScore /= _modelManager.TrainModels.Length;

            _currentGen++;
            Debug.Log($"completed gen {_currentGen} score: {_modelManager.TrainModels[_bestModelIndex].Score} avg score: {avgScore}");
            await Task.Yield();
        }
    }

    private float PlayGame(ref DataImage[] images) {
        int score = 0;

        for (int a = 0; a < NUM_OF_ATTEMPTS; a++) {
            _neuralNetwork.SetInput(images[a].Data);
            int answer = _neuralNetwork.Run();
            if (answer == (int)images[a].Label) {
                score++;
            }
        }

        return score;
    }

    public void StopTraining() {
        _halt = true;
    }

    internal Model GetBestModel() {
        return (_modelManager.TrainModels != null && _bestModelIndex > -1)?_modelManager.TrainModels[_bestModelIndex]: null; 
    }
}