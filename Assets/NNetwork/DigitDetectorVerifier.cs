using System;
using System.Threading.Tasks;
using UnityEngine;

public class DigitDetectorVerifier : IVerifier {
    private InputDataManager _inputDataManager;
    DDNetInputView _inputDisplayView;
    private DDNetOutputView _networkOutputDisaply;
    private SettingsConfig _settingsConfig;
    private NeuralNetwork _neuralNetwork;

    private int _imageSize;
    private int _outputCount = 10;
    private DataImage _image;
    private int _resultIndex;

    public DigitDetectorVerifier(InputDataManager inputDataManager,
        DDNetInputView inputDisplayView,
        DDNetOutputView networkOutputDisaply,
        SettingsConfig settingsConfig,
        NeuralNetwork neuralNetwork) {
        _inputDataManager = inputDataManager;
        _inputDisplayView = inputDisplayView;
        _networkOutputDisaply = networkOutputDisaply;
        _settingsConfig = settingsConfig;
        _neuralNetwork = neuralNetwork;
    }

    public void Init() {
        _inputDataManager.Init();
        _imageSize = _inputDataManager.ImageSize;
    }

    public void SetNewVerefication(bool isTraining) {
        _image = _inputDataManager.GetRandomImage(isTraining);

        for (int i = 0; i < _inputDataManager.ImageSize; i++) {
            for (int j = 0; j < _inputDataManager.ImageSize; j++) {
                _neuralNetwork.InputLayer[i * _inputDataManager.ImageSize + j] = _image.Data[i, j];
            }
        }
    }

    public async Task<float> Verify() {


        await _neuralNetwork.Run();

        float score = 0;
        int answer = _image.Label;

        for (int i = 0; i < _neuralNetwork.OutputLayer.Length; i++) {
            if (i == answer) {
                score += _neuralNetwork.OutputLayer[i];
            } else {
                score += 1f - _neuralNetwork.OutputLayer[i];
            }            
        }

        return score;
    }

   

    public void SetFitness(Model[] models) {

        for (int i = 0; i < models.Length; i++) {
            models[i].Fitness = models[i].Score;
            models[i].Fitness -= _outputCount / 2f;
            models[i].Fitness = Math.Max(0, models[i].Fitness);
        }
    }

    public async Task VisualizeSample() {
        SetNewVerefication(false);
        await _neuralNetwork.Run();

        float max = float.MinValue;
        _resultIndex = 0;

        for (int i = 0; i < _neuralNetwork.OutputLayer.Length; i++) {
            if (_neuralNetwork.OutputLayer[i] > max) {
                max = _neuralNetwork.OutputLayer[i];
                _resultIndex = i;
            }
        }

        (_networkOutputDisaply as DDNetOutputView).SetResult(_resultIndex);
        _networkOutputDisaply.ShowOutput();
        _inputDisplayView.ShowImage(_image, _imageSize);
    }
   

    public void VisualizeOutputReference() {
        throw new NotImplementedException();
    }


    int IVerifier.GetInputSize => _imageSize * _imageSize;

    int IVerifier.GetOtputSize => _outputCount;
}
