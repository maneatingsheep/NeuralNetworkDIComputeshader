using System;
using UnityEngine;

public class NeuralNetwork {

    public NetworkLayer InputLayer;
    public NetworkLayer OutputLayer;
    public NetworkLayer[] HiddenLayers;

    public readonly int NUM_OF_HIDDEN_LAYERS = 2;
    public readonly int HIDDEN_LAYER_SIZE = 20;

    private Model _model;

    private int _imageSize = 26;

    private ComputeShader _computeShader;
    private int _kernelID;
    private bool _useGPU = false;

    private ComputeBuffer _inputBuffer;
    private ComputeBuffer _outputBuffer;
    private ComputeBuffer _flatMatrixBuffer;

    public NeuralNetwork(ComputeShader computeShader) {

        _computeShader = computeShader;

        _kernelID = _computeShader.FindKernel("CSMain");

        _inputBuffer = new ComputeBuffer(26*26, sizeof(float), ComputeBufferType.Default, ComputeBufferMode.Immutable);
        _outputBuffer = new ComputeBuffer(100, sizeof(float), ComputeBufferType.Default, ComputeBufferMode.Dynamic);
        _flatMatrixBuffer = new ComputeBuffer(26*26*100, sizeof(float), ComputeBufferType.Default, ComputeBufferMode.Immutable);

        _computeShader.SetBuffer(_kernelID, "_InputBuffer", _inputBuffer);
        _computeShader.SetBuffer(_kernelID, "_OutputBuffer", _outputBuffer);
        _computeShader.SetBuffer(_kernelID, "_FlatMatrix", _flatMatrixBuffer);


        InputLayer = new NetworkLayer(_imageSize * _imageSize);
        OutputLayer = new NetworkLayer(10);

        HiddenLayers = new NetworkLayer[NUM_OF_HIDDEN_LAYERS];
        for (int i = 0; i < NUM_OF_HIDDEN_LAYERS; i++) {
            HiddenLayers[i] = new NetworkLayer(HIDDEN_LAYER_SIZE);
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

        int maxIndex = GetMaxIndex();

        return maxIndex;

    }

    private int GetMaxIndex() {
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

    private void CalculateLayer(WeightMatrix weightMatrix, NetworkLayer inLayer, NetworkLayer outLayer) {

        float max = 0f;
        float min = float.MaxValue;
        int inLen = inLayer._neurons.Length;
        int outLen = outLayer._neurons.Length;

        if (_useGPU) {

            _inputBuffer.SetData(inLayer._neurons);
            _flatMatrixBuffer.SetData(weightMatrix.GetFlatMatrix());
            int batch = 10;
            _computeShader.SetInt("_InLen", inLen);
            _computeShader.SetInt("_OutLen", outLen);
            _computeShader.SetInt("_Batch", batch);

            _computeShader.Dispatch(_kernelID, Mathf.CeilToInt((outLen / (float)batch) / 64f), 1, 1);

            _outputBuffer.GetData(outLayer._neurons);
            
        } else {
            for (int oIndex = 0; oIndex < outLen; oIndex++) {
                float weightedSum = 0f;
                for (int iIndex = 0; iIndex < inLen; iIndex++) {
                    weightedSum += inLayer._neurons[iIndex] * weightMatrix.GetValue(iIndex, oIndex);
                }

                weightedSum /= inLen;

                weightedSum = ActivationFunc(weightedSum);
                weightedSum += weightMatrix.Bias;
                outLayer._neurons[oIndex] = weightedSum;
            }
        }

        //normlize
        /*for (int oIndex = 0; oIndex < outLen; oIndex++) {
            max = Mathf.Max(max, outLayer._neurons[oIndex]);
            min = Mathf.Min(min, outLayer._neurons[oIndex]);
        }

        float deltaSize = max - min;
        float offset = 1f - (max / deltaSize);

        for (int oIndex = 0; oIndex < outLen; oIndex++) {
            outLayer._neurons[oIndex] = (outLayer._neurons[oIndex] / deltaSize) + offset;
        }*/

    }

    private float ActivationFunc(float weightedSum) {
        //tanh
        return 2 * Sigmoid(2 * weightedSum) - 1f;
    }

    private float Sigmoid(float x) {
        return 1f / (1f + MathF.Pow(MathF.E, -x));
    }

    public void Dispose() {
        if (_inputBuffer != null) {
            _inputBuffer.Dispose();
            _outputBuffer.Dispose();
            _flatMatrixBuffer.Dispose();
        }
    }
}
