using System;
using System.Threading.Tasks;
using UnityEngine;

public class NeuralNetwork {

    public NetworkLayer InputLayer;
    public NetworkLayer OutputLayer;
    public NetworkLayer[] HiddenLayers;

    private Model _model;

    private ComputeShader _computeShader;
    private SettingsConfig _settingsConfig;

    private int _kernelID;
    private bool _useGPU = false;

    private ComputeBuffer _inputBuffer;
    private ComputeBuffer _outputBuffer;
    private ComputeBuffer _flatMatrixBuffer;

    public NeuralNetwork(ComputeShader computeShader, SettingsConfig settingsConfig) {

        _computeShader = computeShader;
        _settingsConfig = settingsConfig;
    }

    public void Init(int inputSize, int outputSize) {
        /*_kernelID = _computeShader.FindKernel("CSMain");

       _inputBuffer = new ComputeBuffer(_inputDataManager.ImageSize * _inputDataManager.ImageSize, sizeof(float), ComputeBufferType.Default, ComputeBufferMode.Immutable);
       _outputBuffer = new ComputeBuffer(100, sizeof(float), ComputeBufferType.Default, ComputeBufferMode.Dynamic);
       _flatMatrixBuffer = new ComputeBuffer(_inputDataManager.ImageSize * _inputDataManager.ImageSize * 100, sizeof(float), ComputeBufferType.Default, ComputeBufferMode.Immutable);

       _computeShader.SetBuffer(_kernelID, "_InputBuffer", _inputBuffer);
       _computeShader.SetBuffer(_kernelID, "_OutputBuffer", _outputBuffer);
       _computeShader.SetBuffer(_kernelID, "_FlatMatrix", _flatMatrixBuffer);
       */


        InputLayer = new NetworkLayer(inputSize);
        OutputLayer = new NetworkLayer(outputSize);

        HiddenLayers = new NetworkLayer[_settingsConfig.NumOfHiddenLayers];
        for (int i = 0; i < _settingsConfig.NumOfHiddenLayers; i++) {
            HiddenLayers[i] = new NetworkLayer(_settingsConfig.HiddenLayerSize);
        }
    }

    internal void SetModel(Model model) {
        _model = model;
    }

    internal void SetInput(float[] data) {
        for (int i = 0; i < data.Length; i++) {
            InputLayer._neurons[i] = data[i];
        }
    }

    internal async Task Run() {

        await Task.Run(() => {
            CalculateLayer(_model.Weights[0], InputLayer, HiddenLayers[0], false);

            for (int i = 0; i < _settingsConfig.NumOfHiddenLayers - 1; i++) {
                CalculateLayer(_model.Weights[i + 1], HiddenLayers[i], HiddenLayers[i + 1], false);
            }

            CalculateLayer(_model.Weights[_settingsConfig.NumOfHiddenLayers], HiddenLayers[_settingsConfig.NumOfHiddenLayers - 1], OutputLayer, true);
        });
       
    }

    public int GetResultIndex() {
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

    private void CalculateLayer(WeightMatrix weightMatrix, NetworkLayer inLayer, NetworkLayer outLayer, bool isLastLayer) {

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
                    var weight = weightMatrix.GetValue(iIndex, oIndex);
                    weightedSum += inLayer._neurons[iIndex] * weight;
                }

                //weightedSum /= inLen;

                if (!isLastLayer) {
                    weightedSum = ActivationFunc(weightedSum);
                }
                weightedSum += weightMatrix.Biases[oIndex];
                outLayer._neurons[oIndex] = weightedSum;
            }
        }

        if (isLastLayer) {
            NormlizeOutput(outLayer);
        }
    }

    private void NormlizeOutput(NetworkLayer outLayer) {
        float max = 0f;
        float min = float.MaxValue;
        //normlize
        for (int oIndex = 0; oIndex < outLayer._neurons.Length; oIndex++) {
            max = Mathf.Max(max, outLayer._neurons[oIndex]);
            min = Mathf.Min(min, outLayer._neurons[oIndex]);
        }

        float deltaSize = max - min;
        float offset = 1f - (max / deltaSize);

        for (int oIndex = 0; oIndex < outLayer._neurons.Length; oIndex++) {
            outLayer._neurons[oIndex] = (outLayer._neurons[oIndex] / deltaSize) + offset;
        }
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
