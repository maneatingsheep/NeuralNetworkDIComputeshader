using System;
using UnityEngine;

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
            HiddenLayers[i] = new NetworkLayer(50);
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
        CalculateLayer(ref _model.AllWeights[0], ref InputLayer, HiddenLayers[0]);
        for (int i = 0; i < NUM_OF_HIDDEN_LAYERS - 1; i++) {
            CalculateLayer(ref _model.AllWeights[i + 1], ref HiddenLayers[i], HiddenLayers[i + 1]);
        }

        CalculateLayer(ref _model.AllWeights[NUM_OF_HIDDEN_LAYERS], ref HiddenLayers[HiddenLayers.Length - 1], OutputLayer);

        float max = -1f;
        int maxIndex = 0;
        for (int i = 0; i < OutputLayer._neurons.Length; i++) {
            if (max < OutputLayer._neurons[i]) {
                max = OutputLayer._neurons[i];
                maxIndex = i;
            }
        }

        return maxIndex;
    }

    private void CalculateLayer(ref NetworkWeightsSingle networkWeightsSingle, ref NetworkLayer inLayer, NetworkLayer outLayer) {
        int inLen = inLayer._neurons.Length;
        int outLen = outLayer._neurons.Length;

        for (int oIndex = 0; oIndex < outLen; oIndex++) {
            outLayer._neurons[oIndex] = 0f;

            for (int iIndex = 0; iIndex < inLen; iIndex++) {
                outLayer._neurons[oIndex] += inLayer._neurons[iIndex] * networkWeightsSingle.Weights[iIndex, oIndex];
            }
        }
    }

    internal void SetModel(Model model) {
        _model = model;
    }
}
