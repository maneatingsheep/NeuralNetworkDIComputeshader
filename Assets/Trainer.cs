using System;
using System.Threading.Tasks;

public class Trainer {

    private const int SIZE_OF_GEN = 100;
    private const int NUM_OF_ATTEMPTS = 30; //consider increase for high scores

    private int _currentGen = 0;

    private Model[] _models;
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
        _bestModelIndex = -1;

        _models = new Model[SIZE_OF_GEN];
        for (int i = 0; i < SIZE_OF_GEN; i++) {
            _models[i].Score = 0;
            _models[i] = _modelManager.EmptyModel(_neuralNetwork);
        }

        await StartTraining();
    }

    private async Task StartTraining() {
        while (!_halt) {
            if (_currentGen != 0) {
                _models = _modelManager.BreedNextGen(_models);
            }

            DataImage[] images = new DataImage[NUM_OF_ATTEMPTS];

            for (int i = 0; i < NUM_OF_ATTEMPTS; i++) {
                images[i] = _inputDataManager.GetRandomImage(true);
            }

            for (int m = 0; m < _models.Length; m++) {
                _neuralNetwork.SetModel(_models[m]);
                float score = PlayGame(images);
                
                _models[m].Score = score;
                if (_bestModelIndex == -1 || _models[m].Score > _models[_bestModelIndex].Score) {
                    _bestModelIndex = m;
                }
            }

            _currentGen++;
            await Task.Yield();
        }
    }

    private float PlayGame(DataImage[] images) {
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
}