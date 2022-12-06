using System;
using UnityEngine;

public class NeuralNetwork {

    public NetworkLayer InputLayer;
    public NetworkLayer OutputLayer;
    public NetworkLayer[] HiddenLayers;

    public readonly int NUM_OF_HIDDEN_LAYERS = 2;

    private Model _model;

    private int _imageSize = 26;

    private ComputeShader _computeShader;
    private bool _useGPU = true;

    private ComputeBuffer _inputBuffer;
    private ComputeBuffer _outputBuffer;
    private ComputeBuffer _flatMatrixBuffer;

    public NeuralNetwork(ComputeShader computeShader) {

        _computeShader = computeShader;

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

    private void CalculateLayer(WeightMatrix networkWeightsSingle, NetworkLayer inLayer, NetworkLayer outLayer) {

        float max = 0f;
        float min = float.MaxValue;
        int inLen = inLayer._neurons.Length;
        int outLen = outLayer._neurons.Length;

        if (_useGPU) {
            int kernelID = _computeShader.FindKernel("CSMain");

            _inputBuffer = new ComputeBuffer(inLen, sizeof(float), ComputeBufferType.Default, ComputeBufferMode.Immutable);
            _outputBuffer = new ComputeBuffer(outLen, sizeof(float), ComputeBufferType.Default, ComputeBufferMode.Dynamic);

            var flatMatrix = networkWeightsSingle.GetFlatMatrix();
            _flatMatrixBuffer = new ComputeBuffer(flatMatrix.Length, sizeof(float), ComputeBufferType.Default, ComputeBufferMode.Immutable);

            _computeShader.SetBuffer(kernelID, "_InputBuffer", _inputBuffer);
            _computeShader.SetBuffer(kernelID, "_OutputBuffer", _outputBuffer);
            _computeShader.SetBuffer(kernelID, "_FlatMatrix", _flatMatrixBuffer);

            _inputBuffer.SetData(inLayer._neurons);
            for (int oIndex = 0; oIndex < outLen; oIndex++) {
                _computeShader.Dispatch(kernelID, oIndex, 1, 1);
            }

            _outputBuffer.GetData(outLayer._neurons);
        } else {
            for (int oIndex = 0; oIndex < outLen; oIndex++) {
                float weightedSum = 0f;
                for (int iIndex = 0; iIndex < inLen; iIndex++) {
                    weightedSum += inLayer._neurons[iIndex] * networkWeightsSingle.GetValue(iIndex, oIndex);
                }

                weightedSum /= inLen;

                weightedSum = ActivationFunc(weightedSum);
                outLayer._neurons[oIndex] = weightedSum;
            }
        }

        //normlize
        for (int oIndex = 0; oIndex < outLen; oIndex++) {
            max = Mathf.Max(max, outLayer._neurons[oIndex]);
            min = Mathf.Min(min, outLayer._neurons[oIndex]);
        }

        float deltaSize = max - min;
        float offset = 1f - (max / deltaSize);

        for (int oIndex = 0; oIndex < outLen; oIndex++) {
            outLayer._neurons[oIndex] = (outLayer._neurons[oIndex] / deltaSize) + offset;
        }

        Dispose();
    }

    private float ActivationFunc(float weightedSum) {
        float input = Mathf.Clamp(weightedSum, 0f, 1f);
        float cos = Mathf.Cos(input * Mathf.PI);
        cos = 1 - ((cos * 0.5f) + 0.5f);
        return cos;
    }

    public void Dispose() {
        _inputBuffer.Dispose();
        _outputBuffer.Dispose();
    }
}
