using System;
using UnityEngine;

public class DigitDetectorVerifier : IVerifier {
    private InputDataManager _inputDataManager;
    DDNetInputView _inputDisplayView;
    private DDNetOutputView _networkOutputDisaply;
    private SettingsConfig _settingsConfig;

    private int _imageSize;
    private int _outputCount = 10;
    private DataImage _image;
    private float[] _networkInput;
    private int _resultIndex;

    public DigitDetectorVerifier(InputDataManager inputDataManager,
        DDNetInputView inputDisplayView,
        DDNetOutputView networkOutputDisaply,
        SettingsConfig settingsConfig) {
        _inputDataManager = inputDataManager;
        _inputDisplayView = inputDisplayView;
        _networkOutputDisaply = networkOutputDisaply;
        _settingsConfig = settingsConfig;
    }

    public void Init() {
        _inputDataManager.Init();
        _imageSize = _inputDataManager.ImageSize;
        _networkInput = new float[_imageSize * _imageSize];
    }

    public float[] SetNewVerefication(bool isTraining, int repetition, int attempt) {
        _image = _inputDataManager.GetRandomImage(isTraining);

        for (int i = 0; i < _inputDataManager.ImageSize; i++) {
            for (int j = 0; j < _inputDataManager.ImageSize; j++) {
                _networkInput[i * _inputDataManager.ImageSize + j] = _image.Data[i, j];
            }
        }

        return _networkInput;
    }

    public float Verify(float[] networkResult) {

        float score = 0;
        int answer = _image.Label;

        float max = float.MinValue;
        _resultIndex = 0;

        for (int i = 0; i < networkResult.Length; i++) {
            if (i == answer) {
                score += networkResult[i];
            } else {
                score += 1f - networkResult[i];
            }

            if (networkResult[i] > max) {
                max = networkResult[i];
                _resultIndex = i;
            }
        }


        return score;
    }

    private void RenderVerefications(int result) {
        (_networkOutputDisaply as DDNetOutputView).SetResult(result);
        _networkOutputDisaply.ShowOutput();
        _inputDisplayView.ShowImage(_image, _imageSize);
    }

    public void SetFitness(Model[] models) {

        float maxPossibleScore = (float)_settingsConfig.NumberOfAttemptsPerTrain * _outputCount;

        for (int i = 0; i < models.Length; i++) {
            models[i].Fitness = models[i].Score;
            models[i].Fitness -= maxPossibleScore / 2f;
            models[i].Fitness = Math.Max(0, models[i].Fitness);
        }

    }

    public void VisualizeResult() {
         RenderVerefications(_resultIndex);
    }

    int IVerifier.GetInputSize => _imageSize * _imageSize;

    int IVerifier.GetOtputSize => _outputCount;

    public int Repetitions => 1;
}
