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
            HiddenLayers[i] = new NetworkLayer(70);
        }
    }

    internal void SetModel(Model model) {
        _model = model;
    }

    internal void SetInput(float[,] data) {
        for (int i = 0; i < _imageSize; i++) {
            for (int j = 0; j < _imageSize; j++) {
                InputLayer._neurons[i * _imageSize + j] = data[i, j];
            }
        }
    }

    internal int Run() {
        CalculateLayer(_model.Weights[0], InputLayer, HiddenLayers[0]);
        for (int i = 0; i < NUM_OF_HIDDEN_LAYERS - 1; i++) {
            CalculateLayer(_model.Weights[i + 1], HiddenLayers[i], HiddenLayers[i + 1]);
        }

        CalculateLayer(_model.Weights[NUM_OF_HIDDEN_LAYERS], HiddenLayers[HiddenLayers.Length - 1], OutputLayer);

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

    private void CalculateLayer(NetworkWeightsSingle networkWeightsSingle, NetworkLayer inLayer, NetworkLayer outLayer) {
        int inLen = inLayer._neurons.Length;
        int outLen = outLayer._neurons.Length;

        for (int oIndex = 0; oIndex < outLen; oIndex++) {
            outLayer._neurons[oIndex] = 0f;

            float weightedSum = 0f;
            for (int iIndex = 0; iIndex < inLen; iIndex++) {
                weightedSum += inLayer._neurons[iIndex] * networkWeightsSingle.Weights[iIndex, oIndex];   
            }

            weightedSum /= inLen;

            weightedSum = ActivationFunc(weightedSum);
            outLayer._neurons[oIndex] = weightedSum;

        }
    }

    private float ActivationFunc(float weightedSum) {
        float input = Mathf.Clamp(weightedSum, 0f, 1f);
        float cos = Mathf.Cos(input * Mathf.PI);
        cos = 1 - ((cos * 0.5f) + 0.5f);
        return cos;
    }

    
}
