using System;
using UnityEngine;

public class DigitDetector : IVerifier {
    private InputDataManager _inputDataManager;
    InputDisplayView _inputDisplayView;
    private NetworkOutputDisaply _networkOutputDisaply;
    private SettingsConfig _settingsConfig;

    private int _imageSize;
    private int _outputCount = 10;
    private DataImage _image;
    private float[] _networkInput;

    public DigitDetector(InputDataManager inputDataManager,
        InputDisplayView inputDisplayView,
        NetworkOutputDisaply networkOutputDisaply,
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

    public float[] SetNewVerefication(bool isTraining) {
        _image = _inputDataManager.GetRandomImage(isTraining);

        for (int i = 0; i < _inputDataManager.ImageSize; i++) {
            for (int j = 0; j < _inputDataManager.ImageSize; j++) {
                _networkInput[i * _inputDataManager.ImageSize + j] = _image.Data[i, j];
            }
        }

        return _networkInput;
    }

    public float Verify(float[] networkResult, bool doVisualize) {

        float score = 0;
        int answer = _image.Label;

        float max = float.MinValue;
        int maxIndex = 0;

        for (int i = 0; i < networkResult.Length; i++) {
            if (i == answer) {
                score += networkResult[i];
            } else {
                score += 1f - networkResult[i];
            }

            if (networkResult[i] > max) {
                max = networkResult[i];
                maxIndex = i;
            }
        }


        if (doVisualize) {
            RenderVerefications(maxIndex);
        }

        return score;
    }

    private void RenderVerefications(int result) {

        _networkOutputDisaply.ShowOutput(result);
        _inputDisplayView.ShowImage(_image, _imageSize);
    }

    public void SetFitness(Model[] models) {

        float maxPossibleScore = (float)_settingsConfig.NumberOfAttemptsPerTrain * _outputCount;

        for (int i = 0; i < models.Length; i++) {
            models[i].Fitness = models[i].Score;
            models[i].Fitness -= maxPossibleScore / 2f;
            models[i].Fitness = Math.Max(0, models[i].Fitness);
        }

        float totalScore = 0;
        for (int i = 0; i < models.Length; i++) {
            totalScore += models[i].Fitness;
        }

        if (totalScore == 0) {
            for (int i = 0; i < models.Length; i++) {
                models[i].Fitness = 1f;
            }
        } else {
            for (int i = 0; i < models.Length; i++) {
                models[i].Fitness /= totalScore;
            }
        }
    }

    int IVerifier.GetInputSize => _imageSize * _imageSize;

    int IVerifier.GetOtputSize => _outputCount;

}
