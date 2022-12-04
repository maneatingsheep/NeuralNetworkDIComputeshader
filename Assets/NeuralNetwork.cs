using System;

public class NeuralNetwork {

    public NetworkLayer InputLayer;
    public NetworkLayer OutputLayer;
    public NetworkLayer[] HiddenLayers;

    public readonly int NUM_OF_HIDDEN_LAYERS = 2;

    private Model _model;

    private int _imageSize = 26;
    public NeuralNetwork() {
        InputLayer = new NetworkLayer(_imageSize * _imageSize);
        OutputLayer = new NetworkLayer(10);

        HiddenLayers = new NetworkLayer[NUM_OF_HIDDEN_LAYERS];
        for (int i = 0; i < NUM_OF_HIDDEN_LAYERS; i++) {
            HiddenLayers[i] = new NetworkLayer(100);
        }
    }

    internal void SetInput(byte[,] data) {
        for (int i = 0; i < _imageSize; i++) {
            for (int j = 0; j < _imageSize; j++) {
                InputLayer._neurons[i * _imageSize + j] = data[i, j];
            }
        }
    }

    internal int Run() {
        return 1;
    }

    internal void SetModel(Model model) {
        _model = model;
    }
}
